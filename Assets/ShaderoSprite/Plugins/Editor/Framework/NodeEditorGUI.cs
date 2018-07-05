using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework 
{
	public static class NodeEditorGUI 
	{
		public static int knobSize = 22;

		public static Color NE_LightColor = new Color (0.4f, 0.4f, 0.4f);
		public static Color NE_TextColor = new Color (0.7f, 0.7f, 0.7f);

		public static Texture2D Background;
		public static Texture2D AALineTex;
        public static Texture2D AALineTexUV;
        public static Texture2D AALineTexRGBA;
        public static Texture2D AALineTexSource;
         public static Texture2D GUIBox;
        public static Texture2D GUIBoxTitle;
        public static Texture2D GUIBoxTitleS;
        public static Texture2D GUIButton;
		public static Texture2D GUIBoxSelection;
        public static Texture2D GUIBoxLight;
        public static Texture2D GuiShadero;

        public static GUISkin nodeSkin;
        public static GUISkin nodeSkin2;
        public static GUISkin defaultSkin;

		public static GUIStyle nodeLabel;
		public static GUIStyle nodeLabelBold;
		public static GUIStyle nodeLabelSelected;

		public static GUIStyle nodeBox;
		public static GUIStyle nodeBoxBold;
        public static GUIStyle nodeBoxTitle;
        public static GUIStyle nodeBoxTitleBold;
        public static float offsetanim;
        public static bool Init ()
		{
			// Textures
			Background = ResourceManager.LoadTexture ("Textures/background.png");
            GUIBoxLight = ResourceManager.LoadTexture("Textures/NE_Box_Light.png");
            AALineTex = ResourceManager.LoadTexture ("Textures/AALine.png");
            AALineTexUV = ResourceManager.LoadTexture("Textures/AALineUV.png");
            AALineTexRGBA = ResourceManager.LoadTexture("Textures/AALineRGBA.png");
            AALineTexSource = ResourceManager.LoadTexture("Textures/AALineSource.png");
            GuiShadero = ResourceManager.LoadTexture("Textures/Shadero_Sprite.png");
            GUIBox = ResourceManager.LoadTexture ("Textures/NE_Box.png");
            GUIBoxTitle = ResourceManager.LoadTexture("Textures/NE_Box_2.png");
            GUIBoxTitleS = ResourceManager.LoadTexture("Textures/NE_Box_3.png");
            GUIButton = ResourceManager.LoadTexture ("Textures/NE_Button.png");
			GUIBoxSelection = ResourceManager.LoadTexture ("Textures/BoxSelection.png");
			
			if (!Background || !AALineTex || !GUIBox || !GUIButton)
				return false;

            // Skin & Styles
            nodeSkin = Object.Instantiate<GUISkin> (GUI.skin);
            nodeSkin2 = Object.Instantiate<GUISkin> (GUI.skin);

            // Label
            nodeSkin.label.fontSize = 14;
            nodeSkin.label.normal.textColor = NE_TextColor;
            nodeLabel = nodeSkin.label;
			// Box
			nodeSkin.box.normal.textColor = NE_TextColor;
			nodeSkin.box.normal.background = GUIBox;
			nodeBox = nodeSkin.box;
            nodeSkin2.box.normal.textColor = NE_TextColor;
            nodeSkin2.box.normal.background = GUIBoxTitle;
            nodeBoxTitle = nodeSkin2.box;
            // Button
            nodeSkin.button.fontSize = 14;
            nodeSkin.button.normal.textColor = NE_TextColor;
			nodeSkin.button.normal.background = GUIButton;
			// TextArea
			nodeSkin.textArea.normal.background = GUIBoxTitle;
			nodeSkin.textArea.active.background = GUIBoxTitleS;
			// Bold Label
			nodeLabelBold = new GUIStyle (nodeLabel);
			nodeLabelBold.fontStyle = FontStyle.Bold;
			// Selected Label
			nodeLabelSelected = new GUIStyle (nodeLabel);
			nodeLabelSelected.normal.background = RTEditorGUI.ColorToTex (1, NE_LightColor);

            nodeBoxTitle.fontSize = 16;
            nodeBoxTitle.alignment = TextAnchor.MiddleCenter;
            // Bold Box
            nodeBoxTitleBold = new GUIStyle(nodeBoxTitle);
            nodeBoxTitleBold.fontSize = 17;
            nodeBoxTitleBold.fontStyle = FontStyle.Bold;
            nodeBoxTitleBold.alignment = TextAnchor.MiddleCenter;
            nodeBoxBold = new GUIStyle (nodeBox);
			nodeBoxBold.fontStyle = FontStyle.Bold;

			return true;
		}

		public static void StartNodeGUI () 
		{
			NodeEditor.checkInit(true);

			defaultSkin = GUI.skin;
			if (nodeSkin != null)
				GUI.skin = nodeSkin;
			OverlayGUI.StartOverlayGUI ();
		}

		public static void EndNodeGUI () 
		{
			OverlayGUI.EndOverlayGUI ();
			GUI.skin = defaultSkin;
		}

		#region Connection Drawing

		public static void DrawConnection (Vector2 startPos, Vector2 endPos, Color col) 
		{
			Vector2 startVector = startPos.x <= endPos.x? Vector2.right : Vector2.left;
			DrawConnection (startPos, startVector, endPos, -startVector, col);
		}
		public static void DrawConnection (Vector2 startPos, Vector2 startDir, Vector2 endPos, Vector2 endDir, Color col) 
		{
			#if NODE_EDITOR_LINE_CONNECTION
			DrawConnection (startPos, startDir, endPos, endDir, ConnectionDrawMethod.StraightLine, col);
			#else
			DrawConnection (startPos, startDir, endPos, endDir, ConnectionDrawMethod.Bezier, col);
			#endif
		}
        public static void DrawConnection (Vector2 startPos, Vector2 startDir, Vector2 endPos, Vector2 endDir, ConnectionDrawMethod drawMethod, Color col) 
		{
			if (drawMethod == ConnectionDrawMethod.Bezier) 
			{
                float dirFactor = 80;
                RTEditorGUI.DrawBezier(startPos, endPos, startPos + startDir * dirFactor, endPos + endDir * dirFactor, col*Color.gray, null, 6);
                
            }
            else if (drawMethod == ConnectionDrawMethod.StraightLine)
				RTEditorGUI.DrawLine (startPos, endPos, col * Color.gray, null, 3);
		}

		internal static Vector2 GetSecondConnectionVector (Vector2 startPos, Vector2 endPos, Vector2 firstVector) 
		{
			if (firstVector.x != 0 && firstVector.y == 0)
				return startPos.x <= endPos.x? -firstVector : firstVector;
			else if (firstVector.y != 0 && firstVector.x == 0)
				return startPos.y <= endPos.y? -firstVector : firstVector;
			else
				return -firstVector;
		}

		#endregion
	}
}