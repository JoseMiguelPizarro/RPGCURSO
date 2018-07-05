using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework 
{
	public enum NodeSide { Left = 4, Top = 3, Right = 2, Bottom = 1 }
    [System.Serializable]
	public class NodeKnob : ScriptableObject
	{
		public Node body;
        protected virtual GUIStyle defaultLabelStyle { get { return GUI.skin.label; } }
		[System.NonSerialized]
		protected internal Texture2D knobTexture;
		
		protected virtual NodeSide defaultSide { get { return NodeSide.Right; } }
		public NodeSide side;
		public float sidePosition = 0;
		public float sideOffset = 0; 

		protected void InitBase (Node nodeBody, NodeSide nodeSide, float nodeSidePosition, string knobName) 
		{
			body = nodeBody;
			side = nodeSide;
			sidePosition = nodeSidePosition;
			name = knobName;
			nodeBody.nodeKnobs.Add (this);
			ReloadKnobTexture ();
		}

		public virtual void Delete () 
		{
			body.nodeKnobs.Remove (this);
			DestroyImmediate (this, true);
		}

		#region Knob Texture Loading

		internal void Check () 
		{
			if (side == 0)
				side = defaultSide;
			if (knobTexture == null)
				ReloadKnobTexture ();
		}

		protected void ReloadKnobTexture () 
		{
			ReloadTexture ();
			if (knobTexture == null)
				throw new UnityException ("Knob texture of " + name + " could not be loaded!");
			if (side != defaultSide) 
			{ 
				ResourceManager.SetDefaultResourcePath (NodeEditor.editorPath + "EditorResources/");
				int rotationSteps = getRotationStepsAntiCW (defaultSide, side);
				
				ResourceManager.MemoryTexture memoryTex = ResourceManager.FindInMemory (knobTexture);
				if (memoryTex != null)
				{ 
					string[] mods = new string[memoryTex.modifications.Length+1];
					memoryTex.modifications.CopyTo (mods, 0);
					mods[mods.Length-1] = "Rotation:" + rotationSteps;
					
					Texture2D knobTextureInMemory = ResourceManager.GetTexture (memoryTex.path, mods);
					if (knobTextureInMemory != null)
					{ 
						knobTexture = knobTextureInMemory;
					}
					else 
					{ 
						knobTexture = RTEditorGUI.RotateTextureCCW (knobTexture, rotationSteps);
						ResourceManager.AddTextureToMemory (memoryTex.path, knobTexture, mods.ToArray ());
					}
				}
				else
				{ 
					knobTexture = RTEditorGUI.RotateTextureCCW (knobTexture, rotationSteps);
				}
			}
      
		}

		protected virtual void ReloadTexture () 
		{
			knobTexture = RTEditorGUI.ColorToTex (1, Color.red);
		}

		#endregion

		#region Additional Serialization

		public virtual ScriptableObject[] GetScriptableObjects () { return new ScriptableObject[0]; }

		protected internal virtual void CopyScriptableObjects (System.Func<ScriptableObject, ScriptableObject> replaceSerializableObject) {}

		#endregion

		#region GUI drawing and Positioning

		public virtual void DrawKnob () 
		{
			Rect knobRect = GetGUIKnob ();

            if (knobTexture.name=="In_Knob(Clone)")
            {
                knobRect = new Rect(knobRect.x + 5, knobRect.y, knobRect.width, knobRect.height);

            }
            if (knobTexture.name == "Out_Knob(Clone)")
            {
                knobRect = new Rect(knobRect.x - 5, knobRect.y, knobRect.width, knobRect.height);
            }
            GUI.DrawTexture(knobRect, knobTexture);

        }

        public void DisplayLayout () 
		{
			DisplayLayout (new GUIContent (name), defaultLabelStyle);
		}

		public void DisplayLayout (GUIStyle style)
		{
			DisplayLayout (new GUIContent (name), style);
		}

		public void DisplayLayout (GUIContent content) 
		{
			DisplayLayout (content, defaultLabelStyle);
		}

		public void DisplayLayout (GUIContent content, GUIStyle style)
		{
			GUILayout.Label (content, style);
			if (Event.current.type == EventType.Repaint)
				SetPosition ();
		}
		
		public void SetPosition (float position, NodeSide nodeSide) 
		{
			if (side != nodeSide) 
			{
				side = nodeSide;
				ReloadKnobTexture ();
			}
			SetPosition (position);
		}
		
		public void SetPosition (float position) 
		{
			sidePosition = position;
		}
		
		public void SetPosition () 
		{
			Vector2 pos = GUILayoutUtility.GetLastRect ().center + body.contentOffset;
			sidePosition = side == NodeSide.Bottom || side == NodeSide.Top? pos.x : pos.y;
		}

		#endregion
		
		#region Position requests

		public Rect GetGUIKnob () 
		{
			Rect rect = GetCanvasSpaceKnob ();
			rect.position += NodeEditor.curEditorState.zoomPanAdjust + NodeEditor.curEditorState.panOffset;
			return rect;
		}
		
		public Rect GetCanvasSpaceKnob () 
		{
			Check ();
			Vector2 knobSize = new Vector2 ((knobTexture.width/knobTexture.height) * NodeEditorGUI.knobSize,
											(knobTexture.height/knobTexture.width) * NodeEditorGUI.knobSize);
			Vector2 knobCenter = GetKnobCenter (knobSize);
			return new Rect (knobCenter.x - knobSize.x/2, knobCenter.y - knobSize.y/2, knobSize.x, knobSize.y);
		}

		private Vector2 GetKnobCenter (Vector2 knobSize) 
		{
			if (side == NodeSide.Left)
				return body.rect.position + new Vector2 (-sideOffset-knobSize.x/2, sidePosition);
			else if (side == NodeSide.Right)
				return body.rect.position + new Vector2 ( sideOffset+knobSize.x/2 + body.rect.width, sidePosition);
			else if (side == NodeSide.Bottom)
				return body.rect.position + new Vector2 (sidePosition,  sideOffset+knobSize.y/2 + body.rect.height);
			else 
				return body.rect.position + new Vector2 (sidePosition, -sideOffset-knobSize.y/2);
		}

		public Vector2 GetDirection () 
		{
			return side == NodeSide.Right? 	Vector2.right : 
					(side == NodeSide.Bottom? Vector2.up : 
					(side == NodeSide.Top? 	Vector2.down : 
									Vector2.left));
		}

		private static int getRotationStepsAntiCW (NodeSide sideA, NodeSide sideB) 
		{
			return sideB - sideA + (sideA>sideB? 4 : 0);
		}

		#endregion

		#region Utility

		public virtual Node GetNodeAcrossConnection()
		{
			return null;
		}
		#endregion
	}
}
