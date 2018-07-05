using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;

namespace _ShaderoShaderEditorFramework
{
	public class NodeInput : NodeKnob
	{
		protected override NodeSide defaultSide { get { return NodeSide.Left; } }

		public NodeOutput connection;
		[FormerlySerializedAs("type")]
		public string typeID;
		private TypeData _typeData;
		internal TypeData typeData { get { CheckType (); return _typeData; } }

		#region General

		public static NodeInput Create (Node nodeBody, string inputName, string inputType)
		{
			return Create (nodeBody, inputName, inputType, NodeSide.Left, 20);
		}

		public static NodeInput Create (Node nodeBody, string inputName, string inputType, NodeSide nodeSide)
		{
			return Create (nodeBody, inputName, inputType, nodeSide, 20);
		}

		public static NodeInput Create (Node nodeBody, string inputName, string inputType, NodeSide nodeSide, float sidePosition)
		{
			NodeInput input = CreateInstance <NodeInput> ();
			input.typeID = inputType;
			input.InitBase (nodeBody, nodeSide, sidePosition, inputName);
			nodeBody.Inputs.Add (input);
			return input;
		}

		public override void Delete () 
		{
			RemoveConnection ();
			body.Inputs.Remove (this);
			base.Delete ();
		}

		#endregion

		#region Additional Serialization

		protected internal override void CopyScriptableObjects (System.Func<ScriptableObject, ScriptableObject> replaceSerializableObject) 
		{
			connection = replaceSerializableObject.Invoke (connection) as NodeOutput;
		}

		#endregion

		#region KnobType

		protected override void ReloadTexture () 
		{
			CheckType ();
			knobTexture = typeData.InKnobTex;
		}

		private void CheckType () 
		{
			if (_typeData == null || !_typeData.isValid ()) 
				_typeData = ConnectionTypes.GetTypeData (typeID);
			if (_typeData == null || !_typeData.isValid ()) 
			{
				ConnectionTypes.FetchTypes ();
				_typeData = ConnectionTypes.GetTypeData (typeID);
				if (_typeData == null || !_typeData.isValid ())
					throw new UnityException ("Could not find type " + typeID + "!");
			}
		}

		#endregion

		#region Value

		public bool IsValueNull { get { return connection != null? connection.IsValueNull : true; } }

		public object GetValue ()
		{
			return connection != null? connection.GetValue () : null;
		}

		public object GetValue (Type type)
		{
			return connection != null? connection.GetValue (type) : null;
		}

		public void SetValue (object value)
		{
			if (connection != null)
				connection.SetValue (value);
		}

		public T GetValue<T> ()
		{
			return connection != null? connection.GetValue<T> () : NodeOutput.GetDefault<T> ();
		}

		public void SetValue<T> (T value)
		{
			if (connection != null)
				connection.SetValue<T> (value);
		}

		#endregion

		#region Connecting Utility

		public bool TryApplyConnection (NodeOutput output)
		{
			if (CanApplyConnection (output)) 
			{
                ApplyConnection (output);
				return true;
			}
			return false;
		}

        public bool CanApplyConnection (NodeOutput output)
		{
			if (output == null || body == output.body || connection == output || !typeData.Type.IsAssignableFrom (output.typeData.Type)) 
			{
				return false;
			}

			if (output.body.isChildOf (body)) 
			{
                if (!output.body.allowsLoopRecursion (body))
				{
					Debug.LogWarning ("Cannot apply connection: Recursion detected!");
					return false;
				}
			}
			return true;
		}

		public void ApplyConnection (NodeOutput output)
		{
			if (output == null) 
				return;
			
			if (connection != null) 
			{
				NodeEditorCallbacks.IssueOnRemoveConnection (this);
				connection.connections.Remove (this);
			}
			connection = output;
			output.connections.Add (this);

			if (!output.body.calculated)
				NodeEditor.RecalculateFrom (output.body);
			else
				NodeEditor.RecalculateFrom (body);
			
			output.body.OnAddOutputConnection (output);
			body.OnAddInputConnection (this);
			NodeEditorCallbacks.IssueOnAddConnection (this);
		}

		public void RemoveConnection ()
		{
			if (connection == null)
				return;
			
			NodeEditorCallbacks.IssueOnRemoveConnection (this);
			connection.connections.Remove (this);
			connection = null;

			NodeEditor.RecalculateFrom (body);
		}


		#endregion

		#region Utility

		public override Node GetNodeAcrossConnection()
		{
			return connection != null ? connection.body : null;
		}

		#endregion
	}
}