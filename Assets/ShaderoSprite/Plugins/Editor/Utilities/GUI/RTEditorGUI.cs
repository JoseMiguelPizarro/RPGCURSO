using UnityEngine;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace _ShaderoShaderEditorFramework.Utilities 
{
	public static class RTEditorGUI 
	{

		#region GUI Proportioning Utilities

		public static float labelWidth = 150;
		public static float fieldWidth = 50;
		public static float indent = 0;
		private static float textFieldHeight { get { return GUI.skin.textField.CalcHeight (new GUIContent ("i"), 10); } }

		public static Rect PrefixLabel (Rect totalPos, GUIContent label, GUIStyle style)
		{
			if (label == GUIContent.none)
				return totalPos;

			Rect labelPos = new Rect (totalPos.x + indent, totalPos.y, Mathf.Min (getLabelWidth () - indent, totalPos.width/2), totalPos.height);
			GUI.Label (labelPos, label, style);

			return new Rect (totalPos.x + getLabelWidth (), totalPos.y, totalPos.width - getLabelWidth (), totalPos.height);
		}

		public static Rect PrefixLabel (Rect totalPos, float percentage, GUIContent label, GUIStyle style)
		{
			if (label == GUIContent.none)
				return totalPos;

			Rect labelPos = new Rect (totalPos.x + indent, totalPos.y, totalPos.width*percentage, totalPos.height);
			GUI.Label (labelPos, label, style);

			return new Rect (totalPos.x + totalPos.width*percentage, totalPos.y, totalPos.width*(1-percentage), totalPos.height);
		}

		private static Rect IndentedRect (Rect source)
		{
			return new Rect (source.x + indent, source.y, source.width - indent, source.height);
		}

		private static float getLabelWidth () 
		{
			#if UNITY_EDITOR
			return UnityEditor.EditorGUIUtility.labelWidth;
			
			#endif
		}

		private static float getFieldWidth () 
		{
			#if UNITY_EDITOR
			return UnityEditor.EditorGUIUtility.fieldWidth;
			
			#endif
		}

		private static Rect GetFieldRect (GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			float minLabelW = 0, maxLabelW = 0;
			if (label != GUIContent.none)
				style.CalcMinMaxWidth (label, out minLabelW, out maxLabelW);
			return GUILayoutUtility.GetRect (getFieldWidth() + minLabelW + 5, getFieldWidth() + maxLabelW + 5, textFieldHeight, textFieldHeight, options);
		}

		private static Rect GetSliderRect (GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			float minLabelW = 0, maxLabelW = 0;
			if (label != GUIContent.none)
				style.CalcMinMaxWidth (label, out minLabelW, out maxLabelW);
			return GUILayoutUtility.GetRect (getFieldWidth() + minLabelW + 5, getFieldWidth() + maxLabelW + 5 + 100, textFieldHeight, textFieldHeight, options);
		}

		private static Rect GetSliderRect (Rect sliderRect)
		{
			return new Rect (sliderRect.x, sliderRect.y, sliderRect.width - getFieldWidth() - 5, sliderRect.height);
		}

		private static Rect GetSliderFieldRect (Rect sliderRect)
		{
			return new Rect (sliderRect.x + sliderRect.width - getFieldWidth(), sliderRect.y, getFieldWidth(), sliderRect.height);
		}

		#endregion

		#region Seperator

		public static void Space ()
		{
			Space (6);
		}
		public static void Space (float pixels)
		{
			GUILayoutUtility.GetRect (pixels, pixels);
		}
        public static void Seperator () 
		{
			setupSeperator ();
			GUILayout.Box (GUIContent.none, seperator, new GUILayoutOption[] { GUILayout.Height (1) });
		}

		public static void Seperator (Rect rect) 
		{
			setupSeperator ();
			GUI.Box (new Rect (rect.x, rect.y, rect.width, 1), GUIContent.none, seperator);
		}

		private static GUIStyle seperator;
		private static void setupSeperator () 
		{
			if (seperator == null) 
			{
				seperator = new GUIStyle();
				seperator.normal.background = ColorToTex (1, new Color (0.6f, 0.6f, 0.6f));
				seperator.stretchWidth = true;
				seperator.margin = new RectOffset(0, 0, 7, 7);
			}
		}

		#endregion

		#region Change Check

		private static Stack<bool> changeStack = new Stack<bool> ();

		public static void BeginChangeCheck () 
		{
			changeStack.Push (GUI.changed);
			GUI.changed = false;
		}

		public static bool EndChangeCheck () 
		{
			bool changed = GUI.changed;
			if (changeStack.Count > 0)
			{
				GUI.changed = changeStack.Pop ();
				if (changed && changeStack.Count > 0 && !changeStack.Peek ())
				{
					changeStack.Pop ();
					changeStack.Push (changed);
				}
			}
			else
				Debug.LogWarning ("Requesting more EndChangeChecks than issuing BeginChangeChecks!");
			return changed;
		}

		#endregion


		#region Foldout and Toggle Wrappers

		public static bool Foldout (bool foldout, string content, params GUILayoutOption[] options)
		{
			return Foldout (foldout, new GUIContent (content), options);
		}

		public static bool Foldout (bool foldout, string content, GUIStyle style, params GUILayoutOption[] options)
		{
			return Foldout (foldout, new GUIContent (content), style, options);
		}

		public static bool Foldout (bool foldout, GUIContent content, params GUILayoutOption[] options)
		{
			#if UNITY_EDITOR 
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.Foldout (foldout, content);
			#endif
			return Foldout (foldout, content, GUI.skin.toggle, options);
		}

		public static bool Foldout (bool foldout, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			#if UNITY_EDITOR 
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.Foldout (foldout, content, style);
			#endif
			return GUILayout.Toggle (foldout, content, style, options);
		}


		public static bool Toggle (bool toggle, string content, params GUILayoutOption[] options)
		{
			return Toggle (toggle, new GUIContent (content), options);
		}

		public static bool Toggle (bool toggle, string content, GUIStyle style, params GUILayoutOption[] options)
		{
			return Toggle (toggle, new GUIContent (content), style, options);
		}

		public static bool Toggle (bool toggle, GUIContent content, params GUILayoutOption[] options)
		{
			#if UNITY_EDITOR 
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.ToggleLeft (content, toggle, options);
			#endif
			return Toggle (toggle, content, GUI.skin.toggle, options);
		}

		public static bool Toggle (bool toggle, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			#if UNITY_EDITOR 
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.ToggleLeft (content, toggle, style, options);
			#endif
			return GUILayout.Toggle (toggle, content, style, options);
		}

		#endregion

		#region Fields and Sliders

		#region Extra

		public static string TextField (GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.TextField (label, text);
			#endif
			if (style == null) style = GUI.skin.textField;
			Rect totalPos = GetFieldRect (label, style, options);
			Rect fieldPos = PrefixLabel (totalPos, 0.5f, label, style);
			text = GUI.TextField (fieldPos, text);
			return text;
		}


		public static int OptionSlider (GUIContent label, int selected, string[] selectableOptions, params GUILayoutOption[] options)
		{
			return OptionSlider (label, selected, selectableOptions, GUI.skin.label, options);
		}

        public static int OptionSlider (GUIContent label, int selected, string[] selectableOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			if (style == null) style = GUI.skin.textField;
			Rect totalPos = GetSliderRect (label, style, options);
			Rect sliderFieldPos = PrefixLabel (totalPos, 0.5f, label, style);

			selected = Mathf.RoundToInt (GUI.HorizontalSlider (GetSliderRect (sliderFieldPos), selected, 0, selectableOptions.Length-1));
			GUI.Label (GetSliderFieldRect (sliderFieldPos), selectableOptions[selected]);
			return selected;
		}

		public static int MathPowerSlider (GUIContent label, int baseValue, int value, int minPow, int maxPow, params GUILayoutOption[] options)
		{
			int power = (int)Math.Floor (Math.Log (value) / Math.Log (baseValue));
			power = MathPowerSliderRaw (label, baseValue, power, minPow, maxPow, options);
			return (int)Math.Pow (baseValue, power);
		}

        public static int MathPowerSliderRaw (GUIContent label, int baseValue, int power, int minPow, int maxPow, params GUILayoutOption[] options)
		{
			Rect totalPos = GetSliderRect (label, GUI.skin.label, options);
			Rect sliderFieldPos = PrefixLabel (totalPos, 0.5f, label, GUI.skin.label);

			power = Mathf.RoundToInt (GUI.HorizontalSlider (GetSliderRect (sliderFieldPos), power, minPow, maxPow));
			GUI.Label (GetSliderFieldRect (sliderFieldPos), Mathf.Pow (baseValue, power).ToString ());
			return power;
		}

		#endregion

		#region Int Fields and Slider Wrappers

		public static int IntSlider (string label, int value, int minValue, int maxValue, params GUILayoutOption[] options) 
		{
			return (int)Slider (new GUIContent (label), value, minValue, maxValue, options);
		}

		public static int IntSlider (GUIContent label, int value, int minValue, int maxValue, params GUILayoutOption[] options) 
		{
			return (int)Slider (label, value, minValue, maxValue, options);
		}

		public static int IntSlider (int value, int minValue, int maxValue, params GUILayoutOption[] options) 
		{
			return (int)Slider (GUIContent.none, value, minValue, maxValue, options);
		}

		public static int IntField (string label, int value, params GUILayoutOption[] options)
		{
			return (int)FloatField (new GUIContent (label), value, options);
		}

		public static int IntField (GUIContent label, int value, params GUILayoutOption[] options)
		{
			return (int)FloatField (label, value, options);
		}

		public static int IntField (int value, params GUILayoutOption[] options)
		{
			return (int)FloatField (value, options);
		}

		#endregion

		#region Float Slider

		public static float Slider (float value, float minValue, float maxValue, params GUILayoutOption[] options) 
		{
			return Slider (GUIContent.none, value, minValue, maxValue, options);
		}

			public static float Slider (string label, float value, float minValue, float maxValue, params GUILayoutOption[] options) 
		{
			return Slider (new GUIContent (label), value, minValue, maxValue, options);
		}

		public static float Slider (GUIContent label, float value, float minValue, float maxValue, params GUILayoutOption[] options) 
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.Slider (label, value, minValue, maxValue, options);
			#endif

			Rect totalPos = GetSliderRect (label, GUI.skin.label, options);
			Rect sliderFieldPos = PrefixLabel (totalPos, 0.5f, label, GUI.skin.label);

			value = GUI.HorizontalSlider (GetSliderRect (sliderFieldPos), value, minValue, maxValue);
			value = Mathf.Min (maxValue, Mathf.Max (minValue, FloatField (GetSliderFieldRect (sliderFieldPos), value, GUILayout.Width (60))));
			return value;
		}

		#endregion

		#region Float Field

		private static int activeFloatField = -1;
		private static float activeFloatFieldLastValue = 0;
		private static string activeFloatFieldString = "";

		public static float FloatField (string label, float value, params GUILayoutOption[] fieldOptions)
		{
			return FloatField (new GUIContent (label), value, fieldOptions);
		}

		public static float FloatField (GUIContent label, float value, params GUILayoutOption[] options)
		{
			Rect totalPos = GetFieldRect (label, GUI.skin.label, options);
			Rect fieldPos = PrefixLabel (totalPos, 0.5f, label, GUI.skin.label);
			return FloatField (fieldPos, value, options);
		}

		public static float FloatField (float value, params GUILayoutOption[] options)
		{
			Rect pos = GetFieldRect (GUIContent.none, null, options);
			return FloatField (pos, value, options);
		}

		public static float FloatField (Rect pos, float value, params GUILayoutOption[] options)
		{
			int floatFieldID = GUIUtility.GetControlID ("FloatField".GetHashCode (), FocusType.Keyboard, pos) + 1;
			if (floatFieldID == 0)
				return value;

			bool recorded = activeFloatField == floatFieldID;
			bool active = floatFieldID == GUIUtility.keyboardControl;

			if (active && recorded && activeFloatFieldLastValue != value)
			{
				activeFloatFieldLastValue = value;
				activeFloatFieldString = value.ToString ();
			}

			
			string str = recorded? activeFloatFieldString : value.ToString ();

			string strValue = GUI.TextField (pos, str);
			if (recorded)
				activeFloatFieldString = strValue;

			bool parsed = true;
			if (strValue == "")
				value = activeFloatFieldLastValue = 0;
			else if (strValue != value.ToString ())
			{
				float newValue;
				parsed = float.TryParse (strValue, out newValue);
				if (parsed)
					value = activeFloatFieldLastValue = newValue;
			}

			if (active && !recorded)
			{ 
				activeFloatField = floatFieldID;
				activeFloatFieldString = strValue;
				activeFloatFieldLastValue = value;
			}
			else if (!active && recorded) 
			{
				activeFloatField = -1;
				if (!parsed)
					value = strValue.ForceParse ();
			}

			return value;
		}

		public static float ForceParse (this string str) 
		{
			
			float value;
			if (float.TryParse (str, out value))
				return value;

			
			bool recordedDecimalPoint = false;
			List<char> strVal = new List<char> (str);
			for (int cnt = 0; cnt < strVal.Count; cnt++) 
			{
				UnicodeCategory type = CharUnicodeInfo.GetUnicodeCategory (str[cnt]);
				if (type != UnicodeCategory.DecimalDigitNumber)
				{
					strVal.RemoveRange (cnt, strVal.Count-cnt);
					break;
				}
				else if (str[cnt] == '.')
				{
					if (recordedDecimalPoint)
					{
						strVal.RemoveRange (cnt, strVal.Count-cnt);
						break;
					}
					recordedDecimalPoint = true;
				}
			}

		
			if (strVal.Count == 0)
				return 0;
			str = new string (strVal.ToArray ());
			if (!float.TryParse (str, out value))
				Debug.LogError ("Could not parse " + str);
			return value;
		}

		#endregion

		#endregion

		#region Object Field

		public static T ObjectField<T> (T obj, bool allowSceneObjects) where T : Object
		{
			return ObjectField<T> (GUIContent.none, obj, allowSceneObjects);
		}

		public static T ObjectField<T> (string label, T obj, bool allowSceneObjects) where T : Object
		{
			return ObjectField<T> (new GUIContent (label), obj, allowSceneObjects);
		}

		public static T ObjectField<T> (GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.ObjectField (label, obj, typeof (T), allowSceneObjects) as T;
			#endif
			bool open = false;
			if (obj.GetType () == typeof(Texture2D)) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label (label);
				open = GUILayout.Button (obj as Texture2D, new GUILayoutOption[] { GUILayout.MaxWidth (64), GUILayout.MaxHeight (64) });
				GUILayout.EndHorizontal ();
			}
			else
			{
				GUIStyle style = new GUIStyle (GUI.skin.box);
				open = GUILayout.Button (label, style);
			}
			if (open)
			{
			
			}
			return obj;
		}

		#endregion

		#region Popups

		public static System.Enum EnumPopup (System.Enum selected) 
		{
			return EnumPopup (GUIContent.none, selected);
		}

		public static System.Enum EnumPopup (string label, System.Enum selected) 
		{
			return EnumPopup (new GUIContent (label), selected);
		}

		public static System.Enum EnumPopup (GUIContent label, System.Enum selected) 
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.EnumPopup (label, selected);
			#endif
			label.text += ": " + selected.ToString ();
			GUILayout.Label (label);
			return selected;
		}

		public static int Popup (GUIContent label, int selected, string[] displayedOptions) 
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label (label);
				selected = UnityEditor.EditorGUILayout.Popup (selected, displayedOptions);
				GUILayout.EndHorizontal ();
				return selected;
			}
			#endif

			GUILayout.BeginHorizontal ();
			label.text += ": " + selected.ToString ();
			GUILayout.Label (label);
			GUILayout.EndHorizontal ();
			return selected;
		}

		public static int Popup (string label, int selected, string[] displayedOptions) 
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return UnityEditor.EditorGUILayout.Popup (label, selected, displayedOptions);
			#endif
			GUILayout.Label (label + ": " + selected.ToString ());
			return selected;
		}

		public static int Popup (int selected, string[] displayedOptions) 
		{
			return Popup ("", selected, displayedOptions);
		}

		#endregion

		#region Extended GUI Texture Drawing

		private static Material texVizMat;

		public static void DrawTexture (Texture texture, int texSize, GUIStyle style, params GUILayoutOption[] options) 
		{
			DrawTexture (texture, texSize, style, 1, 2, 3, 4, options);
		}

		public static void DrawTexture (Texture texture, int texSize, GUIStyle style, int shuffleRed, int shuffleGreen, int shuffleBlue, int shuffleAlpha, params GUILayoutOption[] options) 
		{
			if (texVizMat == null)
				texVizMat = new Material (Shader.Find ("Hidden/GUITextureClip_ChannelControl"));
			texVizMat.SetInt ("shuffleRed", shuffleRed);
			texVizMat.SetInt ("shuffleGreen", shuffleGreen);
			texVizMat.SetInt ("shuffleBlue", shuffleBlue);
			texVizMat.SetInt ("shuffleAlpha", shuffleAlpha);

			if (options == null || options.Length == 0)
				options = new GUILayoutOption[] { GUILayout.ExpandWidth (false) };
			Rect rect = style == null? GUILayoutUtility.GetRect (texSize, texSize, options) : GUILayoutUtility.GetRect (texSize, texSize, style, options);
			if (Event.current.type == EventType.Repaint)
				Graphics.DrawTexture (rect, texture, texVizMat);
		}

		#endregion



		#region Low-Level Drawing

		private static Material lineMaterial;
		private static Texture2D lineTexture;

		private static void SetupLineMat (Texture tex, Color col) 
		{
			if (lineMaterial == null)
				lineMaterial = new Material (Shader.Find ("Hidden/LineShader"));
			if (tex == null)
				tex = lineTexture != null? lineTexture : lineTexture = _ShaderoShaderEditorFramework.Utilities.ResourceManager.LoadTexture ("Textures/AALine.png");
			lineMaterial.SetTexture ("_LineTexture", tex);
			lineMaterial.SetColor ("_LineColor", col);
			lineMaterial.SetPass (0);
		}

		public static void DrawBezier (Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, float width = 1)
		{
			if (Event.current.type != EventType.Repaint)
				return;

			int segmentCount = CalculateBezierSegmentCount (startPos, endPos, startTan, endTan);
			DrawBezier (startPos, endPos, startTan, endTan, col, tex, segmentCount, width);
		}

		public static void DrawBezier (Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, int segmentCount, float width)
		{
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.KeyDown)
				return;

			Vector2[] bezierPoints = new Vector2[segmentCount+1];
			for (int pointCnt = 0; pointCnt <= segmentCount; pointCnt++) 
				bezierPoints[pointCnt] = GetBezierPoint ((float)pointCnt/segmentCount, startPos, endPos, startTan, endTan);
			DrawPolygonLine (bezierPoints, col, tex, width);
		}

		public static void DrawPolygonLine (Vector2[] points, Color col, Texture2D tex, float width = 1)
		{
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.KeyDown)
				return;

			if (points.Length == 1)
				return;
			else if (points.Length == 2)
				DrawLine (points[0], points[1], col, tex, width);

			SetupLineMat (tex, col);
			GL.Begin (GL.TRIANGLE_STRIP);
			GL.Color (Color.white);

			Rect clippingRect = GUIScaleUtility.getTopRect;
			clippingRect.x = clippingRect.y = 0;

			Vector2 curPoint = points[0], nextPoint, perpendicular;
			bool clippedP0, clippedP1;
			for (int pointCnt = 1; pointCnt < points.Length; pointCnt++) 
			{
				nextPoint = points[pointCnt];

				Vector2 curPointOriginal = curPoint, nextPointOriginal = nextPoint;
				if (SegmentRectIntersection (clippingRect, ref curPoint, ref nextPoint, out clippedP0, out clippedP1))
				{ 
					if (pointCnt < points.Length-1) 
						perpendicular = CalculatePointPerpendicular (curPointOriginal, nextPointOriginal, points[pointCnt+1]);
					else
						perpendicular = CalculateLinePerpendicular (curPointOriginal, nextPointOriginal);

					if (clippedP0)
					{ 
						GL.End ();
						GL.Begin (GL.TRIANGLE_STRIP);
						DrawLineSegment (curPoint, perpendicular * width/2);
					}

					if (pointCnt == 1)
						DrawLineSegment (curPoint, CalculateLinePerpendicular (curPoint, nextPoint) * width/2);
					DrawLineSegment (nextPoint, perpendicular * width/2);
				}
				else if (clippedP1)
				{
					GL.End ();
					GL.Begin (GL.TRIANGLE_STRIP);
				}

			
				curPoint = nextPointOriginal;
			}
			
			GL.End ();
		}

		private static int CalculateBezierSegmentCount (Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan) 
		{
			float straightFactor = Vector2.Angle (startTan-startPos, endPos-startPos) * Vector2.Angle (endTan-endPos, startPos-endPos) * (endTan.magnitude+startTan.magnitude);
			straightFactor = 2 + Mathf.Pow (straightFactor / 400, 0.125f); // 1/8
			float distanceFactor = 1 + (startPos-endPos).magnitude;
			distanceFactor = Mathf.Pow (distanceFactor, 0.25f); // 1/4
			return 4 + (int)(straightFactor * distanceFactor);
		}

			private static Vector2 CalculateLinePerpendicular (Vector2 startPos, Vector2 endPos) 
		{
			return new Vector2 (endPos.y-startPos.y, startPos.x-endPos.x).normalized;
		}

		private static Vector2 CalculatePointPerpendicular (Vector2 prevPos, Vector2 pointPos, Vector2 nextPos) 
		{
			return CalculateLinePerpendicular (pointPos, pointPos + (nextPos-prevPos));
		}

		private static Vector2 GetBezierPoint (float t, Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan) 
		{
			float rt = 1 - t;
			float rtt = rt * t;

			return startPos  * rt*rt*rt + 
				startTan * 3 * rt * rtt + 
				endTan   * 3 * rtt * t + 
				endPos   * t*t*t;
		}

		private static void DrawLineSegment (Vector2 point, Vector2 perpendicular) 
		{
			GL.TexCoord2 (0, 0);
			GL.Vertex (point - perpendicular);
			GL.TexCoord2 (0, 1);
			GL.Vertex (point + perpendicular);
		}

		public static void DrawLine (Vector2 startPos, Vector2 endPos, Color col, Texture2D tex, float width = 1)
		{
			if (Event.current.type != EventType.Repaint)
				return;

			
			SetupLineMat (tex, col);
			GL.Begin (GL.TRIANGLE_STRIP);
			GL.Color (Color.white);
			
			Rect clippingRect = GUIScaleUtility.getTopRect;
			clippingRect.x = clippingRect.y = 0;
			
			if (SegmentRectIntersection (clippingRect, ref startPos, ref endPos))
			{ 
				Vector2 perpWidthOffset = CalculateLinePerpendicular (startPos, endPos) * width / 2;
				DrawLineSegment (startPos, perpWidthOffset);
				DrawLineSegment (endPos, perpWidthOffset);
			}
		
			GL.End ();
		}

		private static bool SegmentRectIntersection(Rect bounds, ref Vector2 p0, ref Vector2 p1)
		{
			bool cP0, cP1;
			return SegmentRectIntersection (bounds, ref p0, ref p1, out cP0, out cP1);
		}


			private static bool SegmentRectIntersection (Rect bounds, ref Vector2 p0, ref Vector2 p1, out bool clippedP0, out bool clippedP1)
		{
			float t0 = 0.0f;
			float t1 = 1.0f;
			float dx = p1.x - p0.x;
			float dy = p1.y - p0.y;

			if (ClipTest (-dx, p0.x - bounds.xMin, ref t0, ref t1)) 
			{
				if (ClipTest (dx, bounds.xMax - p0.x, ref t0, ref t1)) 
				{
					if (ClipTest (-dy, p0.y - bounds.yMin, ref t0, ref t1)) 
					{
						if (ClipTest (dy, bounds.yMax - p0.y, ref t0, ref t1)) 
						{
							clippedP0 = t0 > 0;
							clippedP1 = t1 < 1;

							if (clippedP1)
							{
								p1.x = p0.x + t1 * dx;
								p1.y = p0.y + t1 * dy;
							}

							if (clippedP0)
							{
								p0.x = p0.x + t0 * dx;
								p0.y = p0.y + t0 * dy;
							}

							return true;
						}
					}
				}
			}

			clippedP1 = clippedP0 = true;
			return false;
		}

		private static bool ClipTest(float p, float q, ref float t0, ref float t1)
		{
			float u = q / p;

			if (p < 0.0f)
			{
				if (u > t1)
					return false;
				if (u > t0)
					t0 = u;
			}
			else if (p > 0.0f)
			{
				if (u < t0)
					return false;
				if (u < t1)
					t1 = u;
			}
			else if (q < 0.0f)
				return false;

			return true;
		}

		#endregion

		#region Texture Utilities

			public static Texture2D ColorToTex (int pxSize, Color col) 
		{

			Texture2D tex = new Texture2D (pxSize, pxSize);
			for (int x = 0; x < pxSize; x++) 
				for (int y = 0; y < pxSize; y++) 
					tex.SetPixel (x, y, col);
			tex.Apply ();
			return tex;
		}

		public static Texture2D Tint (Texture2D tex, Color color) 
		{
            return tex;
           
		}

		public static Texture2D RotateTextureCCW (Texture2D tex, int quarterSteps) 
		{
			if (tex == null)
				return null;
			tex = UnityEngine.Object.Instantiate (tex);
			int width = tex.width, height = tex.height;
			Color[] col = tex.GetPixels ();
			Color[] rotatedCol = new Color[width*height];
			for (int itCnt = 0; itCnt < quarterSteps; itCnt++) 
			{
				for (int x = 0; x < width; x++)
					for (int y = 0; y < height; y++)
						rotatedCol[x*width + y] = col[(width-y-1) * width + x];
				rotatedCol.CopyTo (col, 0); 
			}
			tex.SetPixels (col);
			tex.Apply ();
			return tex;
		}

		#endregion
	}
}