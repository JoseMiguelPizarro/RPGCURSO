using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;

namespace _ShaderoShaderEditorFramework 
{
	public class NodeOutput : NodeKnob
	{
		protected override NodeSide defaultSide { get { return NodeSide.Right; } }
		private static GUIStyle _defaultStyle;
		protected override GUIStyle defaultLabelStyle { get { if (_defaultStyle == null) { _defaultStyle = new GUIStyle (GUI.skin.label); _defaultStyle.alignment = TextAnchor.MiddleRight; } return _defaultStyle; } }
		public List<NodeInput> connections = new List<NodeInput> ();
		[FormerlySerializedAs("type")]
		public string typeID;
		private TypeData _typeData;
		internal TypeData typeData { get { CheckType (); return _typeData; } }
		[System.NonSerialized]
		private object value = null;
		public bool calculationBlockade = false;
		#region General
		public static NodeOutput Create (Node nodeBody, string outputName, string outputType) 
		{
			return Create (nodeBody, outputName, outputType, NodeSide.Right, 20);
		}
		public static NodeOutput Create (Node nodeBody, string outputName, string outputType, NodeSide nodeSide) 
		{
			return Create (nodeBody, outputName, outputType, nodeSide, 20);
		}
		public static NodeOutput Create (Node nodeBody, string outputName, string outputType, NodeSide nodeSide, float sidePosition) 
		{
			NodeOutput output = CreateInstance <NodeOutput> ();
			output.typeID = outputType;
			output.InitBase (nodeBody, nodeSide, sidePosition, outputName);
			nodeBody.Outputs.Add (output);
			return output;
		}

		public override void Delete () 
		{
			while (connections.Count > 0)
				connections[0].RemoveConnection ();
			body.Outputs.Remove (this);
			base.Delete ();
		}

		#endregion

		#region Additional Serialization

		protected internal override void CopyScriptableObjects (System.Func<ScriptableObject, ScriptableObject> replaceSerializableObject) 
		{
			for (int conCnt = 0; conCnt < connections.Count; conCnt++) 
				connections[conCnt] = replaceSerializableObject.Invoke (connections[conCnt]) as NodeInput;
		}

		#endregion

		#region KnobType

		protected override void ReloadTexture () 
		{
			CheckType ();
			knobTexture = typeData.OutKnobTex;
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
		
		public bool IsValueNull { get { return value == null; } }

		public object GetValue ()
		{
			return value;
		}

		public object GetValue (Type type)
		{
			if (type == null)
				throw new UnityException ("Trying to get value of " + name + " with null type!");
			CheckType ();
			if (type.IsAssignableFrom (typeData.Type))
				return value;
			Debug.LogError ("Trying to GetValue<" + type.FullName + "> for Output Type: " + typeData.Type.FullName);
			return null;
		}

		public void SetValue (object Value)
		{
			CheckType ();
			if (Value == null || typeData.Type.IsAssignableFrom (Value.GetType ()))
				value = Value;
			else
				Debug.LogError ("Trying to SetValue of type " + Value.GetType ().FullName + " for Output Type: " + typeData.Type.FullName);
		}

		public T GetValue<T> ()
		{
			CheckType ();
			if (typeof(T).IsAssignableFrom (typeData.Type))
				return (T)(value?? (value = GetDefault<T> ()));
			Debug.LogError ("Trying to GetValue<" + typeof(T).FullName + "> for Output Type: " + typeData.Type.FullName);
			return GetDefault<T> ();
		}
		
		public void SetValue<T> (T Value)
		{
			CheckType ();
			if (typeData.Type.IsAssignableFrom (typeof(T)))
				value = Value;
			else
				Debug.LogError ("Trying to SetValue<" + typeof(T).FullName + "> for Output Type: " + typeData.Type.FullName);
		}
		
		public void ResetValue () 
		{
			value = null;
		}
		
		public static T GetDefault<T> ()
		{
			if (typeof(T).GetConstructor (System.Type.EmptyTypes) != null)
				return System.Activator.CreateInstance<T> ();
			return default(T);
		}

		public static object GetDefault (Type type)
		{
			if (type.GetConstructor (System.Type.EmptyTypes) != null)
				return System.Activator.CreateInstance (type);
			return null;
		}

		#endregion

        #region Utility

        public override Node GetNodeAcrossConnection()
        {
            return connections.Count > 0 ? connections[0].body : null;
        }

	    #endregion
	}
}