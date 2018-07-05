using UnityEngine;
using System.Collections.Generic;

namespace _ShaderoShaderEditorFramework.Utilities 
{
	public static class OverlayGUI 
	{
		public static PopupMenu currentPopup;

		public static bool HasPopupControl () 
		{
			return currentPopup != null;
		}

		public static void StartOverlayGUI () 
		{
			if (currentPopup != null && Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
				currentPopup.Draw ();
		}

		public static void EndOverlayGUI () 
		{
			if (currentPopup != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
				currentPopup.Draw ();
		}
	}

	public class PopupMenu 
	{
		public delegate void MenuFunction ();
		public delegate void MenuFunctionData (object userData);
		
		public List<MenuItem> menuItems = new List<MenuItem> ();
		
		private Rect position;
		private string selectedPath;
		private MenuItem groupToDraw;
		private float currentItemHeight;
		private bool close;
		
		public static GUIStyle backgroundStyle;
		public static Texture2D expandRight;
		public static float itemHeight;
		public static GUIStyle selectedLabel;
       public static GUIStyle HelpLabel;
        public static GUIStyle HelpLabelDescription;
        public static GUIStyle HelpLabelDescriptionGroup;
        public static GUIStyle RepertoryLabel;

        public float minWidth;
        public static string CurrentSelected;
        public static Vector4 CurrentSelectedPos;
        public static Vector4 CurrentSelectedGroupPos;

        public PopupMenu () 
		{
			SetupGUI ();
		}
		
		public void SetupGUI () 
		{
			backgroundStyle = new GUIStyle (GUI.skin.box);
			backgroundStyle.contentOffset = new Vector2 (2, 2);
			expandRight = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture ("Textures/expandRight.png");
			itemHeight = GUI.skin.label.CalcHeight (new GUIContent ("text"), 100);
			
			selectedLabel = new GUIStyle (GUI.skin.label);
            selectedLabel.normal.background = _ShaderoShaderEditorFramework.Utilities.RTEditorGUI.ColorToTex (1, new Color (0.4f, 0.4f, 0.4f));

            RepertoryLabel = new GUIStyle(GUI.skin.label);

            HelpLabel = new GUIStyle(GUI.skin.label);
            HelpLabel.fontSize = 20;
            HelpLabel.normal.textColor = Color.black;
            HelpLabel.alignment = TextAnchor.MiddleCenter;
            HelpLabelDescription = new GUIStyle(GUI.skin.label);
            HelpLabelDescription.normal.textColor = Color.black;
            HelpLabelDescription.fontSize = 17;
            HelpLabelDescription.alignment = TextAnchor.MiddleCenter;

            HelpLabelDescriptionGroup = new GUIStyle(GUI.skin.label);
            HelpLabelDescriptionGroup.fontSize = 15;
            HelpLabelDescriptionGroup.normal.textColor = Color.black;
            HelpLabelDescriptionGroup.alignment = TextAnchor.MiddleCenter;
        }

        public void Show (Vector2 pos, float MinWidth = 200)
		{
			minWidth = MinWidth;
			position = calculateRect (pos, menuItems, minWidth);
			selectedPath = "";
			OverlayGUI.currentPopup = this;
		}

		public Vector2 Position { get { return position.position; } }

		#region Creation
		
		public void AddItem (GUIContent content, bool on, MenuFunctionData func, object userData)
		{
			string path;
			MenuItem parent = AddHierarchy (ref content, out path);
			if (parent != null)
				parent.subItems.Add (new MenuItem (path, content, func, userData));
			else
				menuItems.Add (new MenuItem (path, content, func, userData));
		}
		
		public void AddItem (GUIContent content, bool on, MenuFunction func)
		{
			string path;
			MenuItem parent = AddHierarchy (ref content, out path);
			if (parent != null)
				parent.subItems.Add (new MenuItem (path, content, func));
			else
				menuItems.Add (new MenuItem (path, content, func));
		}
		
		public void AddSeparator (string path)
		{
			GUIContent content = new GUIContent (path);
			MenuItem parent = AddHierarchy (ref content, out path);
			if (parent != null)
				parent.subItems.Add (new MenuItem ());
			else
				menuItems.Add (new MenuItem ());
		}
		
		private MenuItem AddHierarchy (ref GUIContent content, out string path) 
		{
			path = content.text;
			if (path.Contains ("/"))
			{ 	string[] subContents = path.Split ('/');
				string folderPath = subContents[0];
				
				MenuItem parent = menuItems.Find ((MenuItem item) => item.content != null && item.content.text == folderPath && item.group);
				if (parent == null)
					menuItems.Add (parent = new MenuItem (folderPath, new GUIContent (folderPath), true));
				for (int groupCnt = 1; groupCnt < subContents.Length-1; groupCnt++)
				{
					string folder = subContents[groupCnt];
					folderPath += "/" + folder;
					if (parent == null)
						Debug.LogError ("Parent is null!");
					else if (parent.subItems == null)
						Debug.LogError ("Subitems of " + parent.content.text + " is null!");
					MenuItem subGroup = parent.subItems.Find ((MenuItem item) => item.content != null && item.content.text == folder && item.group);
					if (subGroup == null)
						parent.subItems.Add (subGroup = new MenuItem (folderPath, new GUIContent (folder), true));
					parent = subGroup;
				}
				
				path = content.text;
				content = new GUIContent (subContents[subContents.Length-1], content.tooltip);
				return parent;
			}
			return null;
		}

        #endregion

        #region Drawing
         private string ResolveTextSize(string input, int lineLength)
        {
            string[] words = input.Split(" "[0]);
            string result = "";
            string line = "";
            foreach (string s in words)
            {
                string temp = line + " " + s;
                if (temp.Length > lineLength)
                {
                    result += line + "\n";
                    line = s;
                }
                else
                {
                    line = temp;
                }
            }
            result += line;
            return result.Substring(1, result.Length - 1);
        }

        public void ShowHelp(string texturename, string name, string description)
        {
            Texture2D HelpShadow = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture("Help/help_shadow.png");
            Texture2D HelpBack = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture("Help/help_back.png");
            Texture2D txt2 = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture(texturename);

            Vector2 pos = new Vector2(position.x - 270, position.y-6);
            if (position.x < 260)
            {
                pos.x = CurrentSelectedPos.x + CurrentSelectedPos.z;
          
            }
            if (position.y > Screen.height - 260) pos.y -= 200;
            GUI.DrawTexture(new Rect(pos.x-20, pos.y-20, 320, 320), HelpShadow);
            GUI.DrawTexture(new Rect(pos.x, pos.y, 256, 256), HelpBack);
            GUI.DrawTexture(new Rect(pos.x+1, pos.y+40, 254, 150), txt2);
            GUI.Label(new Rect(pos.x, pos.y, 256, 40), name, HelpLabel);
            int fs = HelpLabelDescription.fontSize;
            int ft = 25;
            if (description.Length > 60) { ft = 36; HelpLabelDescription.fontSize = 13; }
            GUI.Label(new Rect(pos.x, pos.y + 185, 256, 70), ResolveTextSize(description, ft), HelpLabelDescription);
            HelpLabelDescription.fontSize = fs;
        }

        public void ShowHelpGroup(string texturename, string name, string description)
        {
            Texture2D HelpShadow = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture("Help/help_shadow.png");
            Texture2D HelpBackRepertory = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture("Help/help_backrepertory.png");
            Texture2D txt2 = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture(texturename);
            float Zoom = 1.5f;
            Vector2 pos = new Vector2(position.x - 270 * Zoom, position.y - 6 * Zoom);
            if (position.x < 260 * Zoom)
            {
                pos.x = -50 + CurrentSelectedPos.x + CurrentSelectedPos.z;

            }

            if (position.y > Screen.height - 260 * Zoom) pos.y -= 200;

            if (position.x < 260 * Zoom)
            {
                pos.x = Screen.width - 300 * Zoom;
                pos.y = Screen.height - 300 * Zoom;
            }
  
            GUI.DrawTexture(new Rect(pos.x - 20, pos.y - 20* Zoom, 320* Zoom, 320* Zoom), HelpShadow);
            GUI.DrawTexture(new Rect(pos.x, pos.y, 256* Zoom, 256* Zoom), HelpBackRepertory);
            GUI.DrawTexture(new Rect(pos.x + 1, pos.y + 40* Zoom, 254* Zoom, 150* Zoom), txt2);
            GUI.Label(new Rect(pos.x, pos.y, 256* Zoom, 40* Zoom), name, HelpLabel);
            GUI.Label(new Rect(pos.x, pos.y + 185*Zoom, 256* Zoom, 70* Zoom), ResolveTextSize(description,45), HelpLabelDescriptionGroup);
        }
        public void Draw () 
		{

            Texture2D txt = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture("Help/help_background.png");
            GUI.DrawTexture(new Rect(0, 0,Screen.width, Screen.height), txt);

            bool inRect = DrawGroup (position, menuItems);

            while (groupToDraw != null && !close)
			{
				MenuItem group = groupToDraw;
				groupToDraw = null;
				if (group.group)
				{
					if (DrawGroup (group.groupPos, group.subItems))
					inRect = true;
				}
			}
			
			if (!inRect || close) 
			{
				OverlayGUI.currentPopup = null;
			}
            if (Screen.width>900)
            {

            if (CurrentSelected == "Channels 2 RGBA") ShowHelp("Help/id_Channels2RGBA.jpg", "Channels 2 RGBA", "Join 4 Channels (R,G,B and Alpha) to form an RGBA");
            if (CurrentSelected == "RGBA 2 Channels") ShowHelp("Help/id_RGBA2Channels.jpg", "RGBA 2 Channels", "Split RGBA to 4 channels (R,G,B and Alpha)");
            if (CurrentSelected == "Fill Color") ShowHelp("Help/id_fillcolor.jpg", "Fill Color", "Turn the RGB to an unique color Keep the original Alpha");
            if (CurrentSelected == "Preview Texture") ShowHelp("Help/id_PreviewTexture.jpg", "Preview Texture", "Preview the RGBA output an in Real Time");
            if (CurrentSelected == "Tint Color") ShowHelp("Help/id_TintColor.jpg", "Tint Color", "Turn the RGB input into a selected color Keep the original Alpha");
            if (CurrentSelected == "GRP-UV") ShowHelpGroup("Help/id_grp_uv.jpg", "UV Groups", "UV coordinates. UV are used for determining which pixel from the texture being used is shown at any given points on the surface of the model. UV is mainly used for Position, Distortion, Zoom and Rotation.");
            if (CurrentSelected == "GRP-Generate") ShowHelpGroup("Help/id_grp_generate_procedural.jpg", "Generate / Procedural", "Generate from computer algorithm shape and texture");

            if (CurrentSelected == "GRP-Tools") ShowHelpGroup("Help/id_grp_tools.jpg", "Tools", "Great extra tools to make the best optimized shader ever!");
            

            if (CurrentSelected == "GRP-RGBA") ShowHelpGroup("Help/id_grp_rgba.jpg", "RGBA Manipulation", "Manipulate any RGBA Input and/or Output. Mainly for Special FX and color Manipulation");
            if (CurrentSelected == "GRP-Mask") ShowHelpGroup("Help/id_grp_mask.jpg", "Mask Group", "Set of Mask Node to hide a specific part of the image");
            if (CurrentSelected == "GRP-Initialize") ShowHelpGroup("Help/id_grp_initialize.jpg", "Initialize Texture", "The source of the texture by RGBA or by Source. Source is required for some effects such as Blur and others FX");
            if (CurrentSelected == "GRP-FX") ShowHelpGroup("Help/id_grp_fx.jpg", "RGBA Special FX", "Add special FX to the RGBA with powerfull premade and optimised FX");
            if (CurrentSelected == "GRP-Color") ShowHelpGroup("Help/id_grp_color.jpg", "RGBA Color Manipulation", "Generate or Manipute the RGBA colors");
            if (CurrentSelected == "GRP-Clipping") ShowHelpGroup("Help/id_grp_clipping.jpg", "RGBA Clipping", "Remove part of the RGBA image by direction");
            if (CurrentSelected == "GRP-Gradient") ShowHelpGroup("Help/id_grp_gradient.jpg", "RGBA Gradient", "Generate Gradient or manipulate the color by gradient");
            if (CurrentSelected == "GRP-Generate") ShowHelpGroup("Help/id_grp_generate_procedural.jpg", "Generate / Procedural", "Generate from computer algorithm shape and texture");
            if (CurrentSelected == "GRP-Effects") ShowHelpGroup("Help/id_grp_generate_effects.jpg", "Generate Textures", "Generate Procedural texture from computer algorithm");
            if (CurrentSelected == "GRP-Shape") ShowHelpGroup("Help/id_grp_generate_shape.jpg", "Generate / Shape", "Generate Shapes from computer algorithm");

            if (CurrentSelected == "GRP-Channels") ShowHelpGroup("Help/id_grp_channels.jpg", "Split Channels", "Generate Shapes from computer algorithm");
            if (CurrentSelected == "GRP-UV (RGBA)") ShowHelpGroup("Help/id_grp_uvrgba.jpg", "UV convertor", "Use RGBA value Red and Green to manipulate the UV coordinate. Red = X and Green = Y");
            if (CurrentSelected == "GRP-Alpha") ShowHelpGroup("Help/id_grp_alpha.jpg", "Alpha Manipulation", "Manipulate the Alpha of a RGBA");
            if (CurrentSelected == "GRP-Source") ShowHelpGroup("Help/id_grp_source.jpg", "Source", "Convert a source to a RGBA output");
            if (CurrentSelected == "GRP-With 2 RGBA") ShowHelpGroup("Help/id_grp_with2rgba.jpg", "Manipulate 2 RGBA", "Manipulate 2 RGBA easily, for example if you need to Blend 2 Sprites at once, or to fade 2 RGBA");
            if (CurrentSelected == "GRP-Math") ShowHelpGroup("Help/id_grp_math.jpg", "Mathematical Manipulation", "Add mathematical value to the RGBA for example double the intensity of a particular channels");
            if (CurrentSelected == "GRP-Animated") ShowHelpGroup("Help/id_grp_animated.jpg", "Animated UV", "Add UV animation to your Shader, UV animation use a Shader Timer, great for automatic mouvement");
            if (CurrentSelected == "GRP-FX (UV)") ShowHelpGroup("Help/id_grp_fx_uv.jpg", "Visual FX for UV", "Add Rotation, distortion, twist distortion, and more");
            if (CurrentSelected == "GRP-Tilt") ShowHelpGroup("Help/id_grp_tilt.jpg", "Tilt UV", "Tilt the UV from any side");
            if (CurrentSelected == "GRP-Math (UV)") ShowHelpGroup("Help/id_grp_math_uv.jpg", "Mathematical UV Manipulation", "Add mathematical value to the UV");

            if (CurrentSelected == "HologramFX") ShowHelp("Help/id_hologram.jpg", "HologramFX", "Add Holographic Special FX");
            if (CurrentSelected == "Outline Empty") ShowHelp("Help/id_outlineEmpty.jpg", "Outline Empty", "Add an outline without the Sprite. Remember to add extra empty pixel to your Sprite");
            if (CurrentSelected == "Outline") ShowHelp("Help/id_outline.jpg", "Outline", "Add an outline to the Sprite. Remember to add extra empty pixel to your Sprite");
            if (CurrentSelected == "Plasma FX") ShowHelp("Help/id_Plasma.jpg", "Plasma FX", "Turn the Sprite to a Plasma Rainbow Shield");
            if (CurrentSelected == "Circle") ShowHelp("Help/id_shape_circle.jpg", "Circle", "Generate a circle with parameters");
            if (CurrentSelected == "Pyramid") ShowHelp("Help/id_shape_pyramid.jpg", "Pyramid", "Generate a pyramid with parameters");
            if (CurrentSelected == "Shape Side") ShowHelp("Help/id_shape_shape.jpg", "Shape Side", "Generate a shape by the number of sides with parameters");
            if (CurrentSelected == "Threshold") ShowHelp("Help/id_Threshold.jpg", "Threshold", "Threshold the color from a RGBA");
            if (CurrentSelected == "Threshold Smooth") ShowHelp("Help/id_ThresholdSmooth.jpg", "Threshold Smooth", "Threshold the color with smooth effect from a RGBA");
            if (CurrentSelected == "Tint Color") ShowHelp("Help/id_TintColor.jpg", "Tint Color", "Tint the RGBA by a specific Color");
            if (CurrentSelected == "Fill Color") ShowHelp("Help/id_fillcolor.jpg", "Fill Color", "Fill the color to the selected Color and keep the Alpha");
            if (CurrentSelected == "Ghost FX") ShowHelp("Help/id_ghostfx.jpg", "Ghost FX", "This is a better version of the clipping with smooth");

            if (CurrentSelected == "Super Gray Scale") ShowHelp("Help/id_supergrayscale.jpg", "Super Gray", "Add a filter gray scale with different posibilities");
            if (CurrentSelected == "Color Filters") ShowHelp("Help/id_colorfilters.jpg", "Color Filters", "Add a filter with a preset color");
            if (CurrentSelected == "Generate a Sprite") ShowHelp("Help/id_generate_a_sprite.jpg", "Generate a Sprite", "Save the result as a RGBA PNG with transparence");
            if (CurrentSelected == "Lighting Support") ShowHelp("Help/id_lightingsupport.jpg", "Add Light Support", "Use the Light Support. Now you can use the light from the Unity scene, just like 3D!");
            if (CurrentSelected == "Blur") ShowHelp("Help/id_blur.jpg", "Blur", "Add a simple Blur FX ( Ultra Fast )");
            if (CurrentSelected == "Blur HQ") ShowHelp("Help/id_blur_hq.jpg", "Blur HQ", "Add a simple Blur FX HQ ( Fast )");

            if (CurrentSelected == "Burn FX") ShowHelp("Help/id_burn_fx.jpg", "Burn FX", "Add a Burn FX you can change the seed");
            if (CurrentSelected == "Circle Fade") ShowHelp("Help/id_circlefade.jpg", "Circle Fade", "Add a Circle Fade to the RGBA, similar to a Mask with Circle");
            if (CurrentSelected == "Compression FX") ShowHelp("Help/id_compressionfx.jpg", "Compression FX", "Add a compression glitch FX");
            if (CurrentSelected == "Desintegration") ShowHelp("Help/id_desintegration.jpg", "Desintegration", "Add a Desintegration FX with parameters");
            if (CurrentSelected == "DestroyerFX") ShowHelp("Help/id_destroyer.jpg", "DestroyerFX", "Add a Desintegration FX with parameters");
         

            if (CurrentSelected == "Brightness") ShowHelp("Help/id_brightness.jpg", "Brightness", "Change the Brightness of the sprite");
            if (CurrentSelected == "HSV") ShowHelp("Help/id_hsv.jpg", "HSV", "Change the HSV of the sprite ( HUE, Saturation and Value)");
            if (CurrentSelected == "*RGBA") ShowHelp("Help/id_mul_rgba.jpg", "*RGBA", "Multiply the RGBA with a specified Color");
            if (CurrentSelected == "Darkness") ShowHelp("Help/id_darkness.jpg", "Darkness", "Change the Darkness of the sprite");
            if (CurrentSelected == "4 Gradients") ShowHelp("Help/id_4_gradient.jpg", "4 Gradients", "Turn the Sprite into a 4 Corner gradients");
            if (CurrentSelected == "Gray Scale") ShowHelp("Help/id_grayscale.jpg", "Gray Scale", "Turn the Sprite to a grayscale color");
            if (CurrentSelected == "InverseColor") ShowHelp("Help/id_inverse.jpg", "InverseColor", "Inverse the RGBA color");
            if (CurrentSelected == "Down") ShowHelp("Help/id_clipping_down.jpg", "Clipping Down", "Cut a part of the sprite from the bottom");
            if (CurrentSelected == "Left") ShowHelp("Help/id_clipping_left.jpg", "Clipping Left", "Cut a part of the sprite from the left");
            if (CurrentSelected == "Right") ShowHelp("Help/id_clipping_right.jpg", "Clipping Right", "Cut a part of the sprite from the right");
            if (CurrentSelected == "Up") ShowHelp("Help/id_clipping_up.jpg", "Clipping Up", "Cut a part of the sprite from the top");
            if (CurrentSelected == "Color Gradients") ShowHelp("Help/id_color_gradient.jpg", "Color Gradients", "Change RGBA with a manual gradients");
            if (CurrentSelected == "Premade Gradients") ShowHelp("Help/id_gradient_premade.jpg", "Premade Gradients", "Change the RGBA with special premade gradients");
            if (CurrentSelected == "Checker") ShowHelp("Help/id_checker.jpg", "Checker", "Generate a Checker texture");
            if (CurrentSelected == "Fire") ShowHelp("Help/id_fire.jpg", "Fire", "Generate an animated black and white fire texture");
            if (CurrentSelected == "Voronoi") ShowHelp("Help/id_voronoi.jpg", "Voronoi", "Generate a Voronoi Texture");
            if (CurrentSelected == "RGBA to UV") ShowHelp("Help/id_rgba2uv.jpg", "RGBA to UV", "Turn the Red and Green from the RGBA to UV");
            if (CurrentSelected == "UV to RGBA") ShowHelp("Help/id_uv2rgba.jpg", "UV to RGBA", "Turn the UV to RGBA ( Red = uv.x, Green = uv.y)");
            if (CurrentSelected == "Fade to Alpha") ShowHelp("Help/id_fadetoalpha.jpg", "Fade to Alpha", "Fade RGBA Alpha only");
            if (CurrentSelected == "Force RGBA Alpha from RGBA") ShowHelp("Help/id_forcergbafromrgba.jpg", "RGBA Alpha from RGBA", "Force to use an Alpha from another RGBA");
            if (CurrentSelected == "Force RGBA Alpha from Alpha") ShowHelp("Help/id_forcergbafromalpha.jpg", "RGBA Alpha from Alpha", "Force to use an Alpha from another Alpha Channel");
            if (CurrentSelected == "to RGBA") ShowHelp("Help/id_source.jpg", "Source to RGBA", "Convert a Source format to an RGBA. Use it to keep to avoid many source");
            if (CurrentSelected == "Addition") ShowHelp("Help/id_w2r_add.jpg", "Addition", "Addition of 2 RGBA and return the result in RGBA");
            if (CurrentSelected == "Blend") ShowHelp("Help/id_w2r_blend.jpg", "Blend", "Blend 2 RGBA at once.");
            if (CurrentSelected == "Division") ShowHelp("Help/id_w2r_division.jpg", "Division", "Divide 2 RGBA and return the result in RGBA");
            if (CurrentSelected == "Lerp") ShowHelp("Help/id_w2r_lerp.jpg", "Lerp", "Fade 2 RGBA");
            if (CurrentSelected == "Multiplication") ShowHelp("Help/id_wr2_mul.jpg", "Multiplication", "Multiply 2 RGBA and return the result in RGBA");
            if (CurrentSelected == "Subtraction") ShowHelp("Help/id_wr2_sub.jpg", "Subtraction", "Subtraction of 2 RGBA and return the result in RGBA");

            if (CurrentSelected == "RGBA + value") ShowHelp("Help/id_add.jpg", "RGBA + value", "Add mathematical value to the UV");
            if (CurrentSelected == "RGBA div value") ShowHelp("Help/id_div.jpg", "RGBA div value", "Add mathematical value to the UV");
            if (CurrentSelected == "RGBA * value") ShowHelp("Help/id_mul.jpg", "RGBA * value", "Add mathematical value to the UV");
            if (CurrentSelected == "RGBA - value") ShowHelp("Help/id_sub.jpg", "RGBA - value", "Add mathematical value to the UV");
            if (CurrentSelected == "Mask with Channel") ShowHelp("Help/id_mask_channel.jpg", "Mask with Channel", "Mask a RGBA with a channel");
            if (CurrentSelected == "Mask with RGBA") ShowHelp("Help/id_mask_rgba.jpg", "Mask with RGBA", "Mask a RGBA with another RGBA");
            if (CurrentSelected == "Mask 2 RGBA") ShowHelp("Help/id_mask_2rgba.jpg", "Mask 2 RGBA", "Mask 2 RGBA with another RGBA");

            if (CurrentSelected == "Animated Offset UV") ShowHelp("Help/id_anm_offset.jpg", "Animated Offset UV", "Move automaticaly the offset of a UV");
            if (CurrentSelected == "Ping Pong Offset UV") ShowHelp("Help/id_anm_pingpong.jpg", "Ping Pong Offset UV", "Move automaticaly with a ping pong FX");
            if (CurrentSelected == "Animated Twist UV") ShowHelp("Help/id_anm_twist.jpg", "Twist UV", "Twist FX with mouvement");
            if (CurrentSelected == "Displacement UV") ShowHelp("Help/id_uv_displacement.jpg", "Displacement UV", "Displacement with a RGBA from a Source input");
            if (CurrentSelected == "Distortion UV") ShowHelp("Help/id_uv_distortion.jpg", "Distortion UV", "Distortion FX");
            if (CurrentSelected == "Offset UV") ShowHelp("Help/id_uv_offset.jpg", "Offset UV", "Move the position of the Sprite");
            if (CurrentSelected == "Pixel UV") ShowHelp("Help/id_uv_pixel.jpg", "Pixel UV", "Add a Pixel FX");
            if (CurrentSelected == "Pixel XY UV") ShowHelp("Help/id_uv_pixel.jpg", "Pixel XY UV", "Add a Pixel XY FX");
            if (CurrentSelected == "Rotation UV") ShowHelp("Help/id_uv_rotation.jpg", "Rotation UV", "Rotation a Sprite");
            if (CurrentSelected == "Twist UV") ShowHelp("Help/id_uv_twist.jpg", "Twist UV", "Add a twist FX");
            if (CurrentSelected == "Zoom UV") ShowHelp("Help/id_uv_zoom.jpg", "Zoom UV", "Zoom in out a Sprite with positions");
            if (CurrentSelected == "Tilt Down") ShowHelp("Help/id_tilt_down.jpg", "Tilt Down", "Tilt the UV from the bottom");
            if (CurrentSelected == "Tilt Left") ShowHelp("Help/id_tilt_left.jpg", "Tilt Left", "Tilt the UV from the left");
            if (CurrentSelected == "Tilt Right") ShowHelp("Help/id_tilt_right.jpg", "Tilt Right", "Tilt the UV from the right");
            if (CurrentSelected == "Tilt Up") ShowHelp("Help/id_tilt_up.jpg", "Tilt Up", "Tilt the UV from the top");
            if (CurrentSelected == "Saturate UV") ShowHelp("Help/id_saturate_uv.jpg", "Saturate UV", "Saturate the UV value to from 0 to 1");
            if (CurrentSelected == "Position UV") ShowHelp("Help/id_position_uv.jpg", "Position UV", "Change the Position");
            if (CurrentSelected == "Resize UV") ShowHelp("Help/id_resize_uv.jpg", "Resize UV", "Resize the UV to create Zoom In or Zoom Out");

            if (CurrentSelected == "Grab Pass") ShowHelp("Help/id_grabpass.jpg", "Grab Pass", "Use the Screen Background as a Texture");
            if (CurrentSelected == "GrabPass UV") ShowHelp("Help/id_uv_grabpass.jpg", "GrabPass UV", "Use this UV if you are using Grab Pass");
            if (CurrentSelected == "Main Texture") ShowHelp("Help/id_maintex.jpg", "Main Texture", "Sprite from the SpriteRenderer. Shadero use a preview sprite because the editor doesn't know what sprite the SpriteRenderer will be used");

            if (CurrentSelected == "New Texture") ShowHelp("Help/id_newtexture.jpg", "New Texture", "Use a personalised Texture");
            if (CurrentSelected == "Original UV") ShowHelp("Help/id_maintex_uv.jpg", "Original UV", "Use the original UV");
            if (CurrentSelected == "Source Grab Pass") ShowHelp("Help/id_source_grabpass.jpg", "Source Grab Pass", "Use the Screen Background as a Source. Some FX required a Source.");
            if (CurrentSelected == "Source Main Texture") ShowHelp("Help/id_source_maintex.jpg", "Source Main Texture", "Use the Sprite Texture as a Source. Some FX required a Source.");
            if (CurrentSelected == "Source New Texture") ShowHelp("Help/id_source_newtexture.jpg", "Source New Texture", "Use a personalised Texture as a Source. Some FX required a Source.");

            if (CurrentSelected == "Hdr Control Value") ShowHelp("Help/id_Hdr_Control_Value.jpg", "HDR Control Value", "Control the intensity of the HDR values.");
            if (CurrentSelected == "Hdr Create") ShowHelp("Help/id_Hdr_Create.jpg", "HDR Create", "Create HDR from the color intensity. Brightness color will be set as HDR");
            if (CurrentSelected == "Turn Transparent") ShowHelp("Help/id_TurnTransparent.jpg", "Turn to Transparent", "Create a sort of shield transparent effects.");
            if (CurrentSelected == "Turn Gold") ShowHelp("Help/id_TurnGold.jpg", "Turn to Gold", "Turn the source to a Gold aspect with HDR support.");
            if (CurrentSelected == "Turn Metal") ShowHelp("Help/id_TurnSilver.jpg", "Turn to Metal", "Turn the source to a Metal aspect with HDR support.");
            if (CurrentSelected == "Sprite Sheet Animation UV") ShowHelp("Help/id_sprite_sheet_animation.jpg", "Sprite Sheet Animation UV", "Create animation from a Sprite Sheet Texture");
            if (CurrentSelected == "Sprite Sheet Frame UV") ShowHelp("Help/id_sprite_sheet_frame.jpg", "Sprite Sheet Frame UV", "Use a specific frame from a Sprite Sheet Texture");
            if (CurrentSelected == "Displacement Plus UV") ShowHelp("Help/id_displacement_plus_uv.jpg", "Displacement Plus UV", "Displacement of pixel with a RGBA from a Source input with mask and position");
            if (CurrentSelected == "Fish Eye UV") ShowHelp("Help/id_fish_eye.jpg", "Fish Eye UV", "Use a specific frame from a Sprite Sheet Texture");
            if (CurrentSelected == "Kaleidoscope UV") ShowHelp("Help/id_kaleidoscope.jpg", "Kaleidoscope UV", "Use a specific frame from a Sprite Sheet Texture");
            if (CurrentSelected == "Inner Glow") ShowHelp("Help/id_innerglow.jpg", "Inner Glow FX", "Add an inner glow effect on the sprite. a Source is required");
                
            if (CurrentSelected == "Pinch UV") ShowHelp("Help/id_pinch.jpg", "Pinch UV", "Use a specific frame from a Sprite Sheet Texture");
            if (CurrentSelected == "Saturate") ShowHelp("Help/id_Math_Saturate.jpg", "Saturate", "Saturate the RGBA value to from 0 to 1");
            if (CurrentSelected == "Flip Horizontal UV") ShowHelp("Help/id_flip_horizontal.jpg", "Flip Horizontal UV", "Flip horizontally");
            if (CurrentSelected == "Flip Vertical UV") ShowHelp("Help/id_flip_vertical.jpg", "Flip Vertical UV", "Flip vertically");

            if (CurrentSelected == "Donuts") ShowHelp("Help/id_generate_donut.jpg", "Generate Donuts", "Generate a donuts with parameters");
            if (CurrentSelected == "Lines") ShowHelp("Help/id_generate_line.jpg", "Generate Line", "Generate lines with parameters");
            if (CurrentSelected == "Noise") ShowHelp("Help/id_generate_noise.jpg", "Generate Noise", "Generate noise");
            if (CurrentSelected == "Lightning FX") ShowHelp("Help/id_generate_lightning_storm.jpg", "Generate Lightning FX", "Generate lightning effects with parameters");

            if (CurrentSelected == "Turn Alpha To Black") ShowHelp("Help/id_TurnAlphaToBlack.jpg", "Turn Alpha to Black", "Turn the alpha to a black color (useful for masking)");
            if (CurrentSelected == "Turn Black To Alpha") ShowHelp("Help/id_TurnBlackToAlpha.jpg", "Turn Black to Alpha", "Turn the black color to an alpha");
            if (CurrentSelected == "Pattern Movement") ShowHelp("Help/id_pattern_mouvement.jpg", "Generate Lightning FX", "Move the Pattern offset");
            if (CurrentSelected == "Pattern Movement Mask") ShowHelp("Help/id_pattern_mouvement_mask.jpg", "Generate Lightning FX", "Move the Pattern offset with mask");
            if (CurrentSelected == "Blend with Mask") ShowHelp("Help/id_Blend_Mask.jpg", "Generate Lightning FX", "Mask and Blend");
            if (CurrentSelected == "Animated Mouvement UV") ShowHelp("Help/id_animation_movement.jpg", "Generate Lightning FX", "Move the animated pattern offset with mask");
            if (CurrentSelected == "Render Texture") ShowHelp("Help/id_renderTexture.jpg", "Generate Lightning FX", "Use a Render Texture");
            if (CurrentSelected == "Source Render Texture") ShowHelp("Help/id_sourceRenderTexture.jpg", "Source Render Texture FX", "Use the Render Texture as a Source. Some FX required a Source.");
            if (CurrentSelected == "Xray") ShowHelp("Help/id_Xray.jpg", "X Ray", "Generate an X-Ray light with parameters");


            }
            _ShaderoShaderEditorFramework.NodeEditor.RepaintClients ();
            CurrentSelected = "";
        }

       

        private bool DrawGroup (Rect pos, List<MenuItem> menuItems) 
		{
            CurrentSelectedGroupPos = new Vector4(pos.x, pos.y, pos.width, pos.height);
            Rect rect = calculateRect (pos.position, menuItems, minWidth);
            CurrentSelectedPos = new Vector4(rect.x, rect.y, rect.width, rect.height);


            Rect clickRect = new Rect (rect);
			clickRect.xMax += 20;
			clickRect.xMin -= 20;
			clickRect.yMax += 20;
			clickRect.yMin -= 20;
			bool inRect = clickRect.Contains (Event.current.mousePosition);

			currentItemHeight = backgroundStyle.contentOffset.y;
			GUI.BeginGroup (extendRect (rect, backgroundStyle.contentOffset), GUIContent.none, backgroundStyle);
			for (int itemCnt = 0; itemCnt < menuItems.Count; itemCnt++)
			{
				DrawItem (menuItems[itemCnt], rect);
				if (close) break;
			}
			GUI.EndGroup ();
			
			return inRect;
		}
		  private void DrawItem (MenuItem item, Rect groupRect) 
		{
			if (item.separator) 
			{
				if (Event.current.type == EventType.Repaint)
					RTEditorGUI.Seperator (new Rect (backgroundStyle.contentOffset.x+16, currentItemHeight+1, groupRect.width-2, 1));
				currentItemHeight += 3;
			}
			else 
			{
				Rect labelRect = new Rect (backgroundStyle.contentOffset.x, currentItemHeight, groupRect.width, itemHeight);

				if (labelRect.Contains (Event.current.mousePosition))
					selectedPath = item.path;

				bool selected = selectedPath == item.path || selectedPath.Contains (item.path + "/");

                if (item.group)
                {
                    Texture2D txt = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture("Textures/repertory.png");
                    GUI.DrawTexture(new Rect(labelRect.x+1, labelRect.y+4, 15, 13), txt);
                    if (selected) CurrentSelected = "GRP-"+item.content.text;


                    GUI.Label(new Rect(labelRect.x + 16, labelRect.y, labelRect.width, labelRect.height), item.content, selected ? selectedLabel : RepertoryLabel);
                }
                else
                {
                    if (selected) CurrentSelected = item.content.text;
 
                    GUI.Label(new Rect(labelRect.x + 16, labelRect.y, labelRect.width, labelRect.height), item.content, selected ? selectedLabel : GUI.skin.label);
                }

                if (item.group) 
				{
					GUI.DrawTexture (new Rect (labelRect.x+labelRect.width-12, labelRect.y+(labelRect.height-12)/2, 12, 12), expandRight);
					if (selected)
					{
						item.groupPos = new Rect (groupRect.x+groupRect.width+4, groupRect.y+currentItemHeight-2, 0, 0);
						groupToDraw = item;
                    }
				}
				else if (selected && (Event.current.type == EventType.MouseDown || (Event.current.button != 1 && Event.current.type == EventType.MouseUp)))
				{
					item.Execute ();
					close = true;
					Event.current.Use ();
				}
				
				currentItemHeight += itemHeight;
			}
		}
		
		private static Rect extendRect (Rect rect, Vector2 extendValue) 
		{
			rect.x -= extendValue.x;
			rect.y -= extendValue.y;
			rect.width += extendValue.x+extendValue.x;
			rect.height += extendValue.y+extendValue.y;
			return rect;
		}
		
		private static Rect calculateRect (Vector2 position, List<MenuItem> menuItems, float minWidth) 
		{
			Vector2 size;
			float width = minWidth, height = 0;
			
			for (int itemCnt = 0; itemCnt < menuItems.Count; itemCnt++)
			{
				MenuItem item = menuItems[itemCnt];
				if (item.separator)
					height += 3;
				else
				{
					width = Mathf.Max (width, GUI.skin.label.CalcSize (item.content).x + (item.group? 22 : 10));
					height += itemHeight;
				}
			}
			
			size = new Vector2 (width + 16, height);
			bool down = (position.y + size.y) <= Screen.height;
            bool sdown = size.y >= Screen.height-60;
            Rect trec = new Rect(position.x, position.y - (down ? 0 : size.y/2), size.x, size.y);
            if (sdown) trec = new Rect(position.x, 5, size.x, size.y);

            return trec;
		}
		
		#endregion
		
		#region Nested MenuItem
		
		public class MenuItem
		{
			public string path;
			// -!Separator
			public GUIContent content;
			// -Executable Item
			public MenuFunction func;
			public MenuFunctionData funcData;
			public object userData;
			// -Non-executables
			public bool separator = false;
			// --Group
			public bool group = false;
			public Rect groupPos;
			public List<MenuItem> subItems;
			
			public MenuItem ()
			{
				separator = true;
			}
			
			public MenuItem (string _path, GUIContent _content, bool _group)
			{
				path = _path;
				content = _content;
				group = _group;
				
				if (group)
					subItems = new List<MenuItem> ();
			}
			
			public MenuItem (string _path, GUIContent _content, MenuFunction _func)
			{
				path = _path;
				content = _content;
				func = _func;
			}
			
			public MenuItem (string _path, GUIContent _content, MenuFunctionData _func, object _userData)
			{
				path = _path;
				content = _content;
				funcData = _func;
				userData = _userData;
			}
			
			public void Execute ()
			{
				if (funcData != null)
					funcData (userData);
				else if (func != null)
					func ();
			}
		}
		
		#endregion
	}

		public class GenericMenu
	{
		private static PopupMenu popup;

		public Vector2 Position { get { return popup.Position; } }
		
		public GenericMenu () 
		{
			popup = new PopupMenu ();
		}
		
		public void ShowAsContext ()
		{
			popup.Show (GUIScaleUtility.GUIToScreenSpace (Event.current.mousePosition));
		}

		public void Show (Vector2 pos, float MinWidth = 40)
		{
			popup.Show (GUIScaleUtility.GUIToScreenSpace (pos), MinWidth);
		}
		
		public void AddItem (GUIContent content, bool on, PopupMenu.MenuFunctionData func, object userData)
		{
			popup.AddItem (content, on, func, userData);
		}
		
		public void AddItem (GUIContent content, bool on, PopupMenu.MenuFunction func)
		{
			popup.AddItem (content, on, func);
		}
		
		public void AddSeparator (string path)
		{
			popup.AddSeparator (path);
		}
	}
}