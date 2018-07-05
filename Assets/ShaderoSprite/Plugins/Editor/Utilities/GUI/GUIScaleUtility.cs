using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace _ShaderoShaderEditorFramework.Utilities
{

    public static class GUIScaleUtility
    {
        private static bool compabilityMode;
        private static bool initiated;

        private static FieldInfo currentGUILayoutCache;
        private static FieldInfo currentTopLevelGroup;

        private static Func<Rect> GetTopRectDelegate;
        private static Func<Rect> topmostRectDelegate;

        public static Rect getTopRect { get { return (Rect)GetTopRectDelegate.Invoke(); } }
        public static Rect getTopRectScreenSpace { get { return (Rect)topmostRectDelegate.Invoke(); } }

        public static List<Rect> currentRectStack { get; private set; }
        private static List<List<Rect>> rectStackGroups;

        private static List<Matrix4x4> GUIMatrices;
        private static List<bool> adjustedGUILayout;

        #region Init

        public static void CheckInit()
        {
            if (!initiated)
                Init();
        }

        public static void Init()
        {
            Assembly UnityEngine = Assembly.GetAssembly(typeof(UnityEngine.GUI));
            Type GUIClipType = UnityEngine.GetType("UnityEngine.GUIClip", true);

            PropertyInfo topmostRect = GUIClipType.GetProperty("topmostRect", BindingFlags.Static | BindingFlags.Public);

            if (topmostRect == null)
            {

                MethodInfo topmostRectMethod = GUIClipType.GetMethod("get_topmostRect", BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo GetTopRect = GUIClipType.GetMethod("GetTopRect", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo ClipRect = GUIClipType.GetMethod("Clip", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new Type[] { typeof(Rect) }, new ParameterModifier[] { });

                if (GUIClipType == null || topmostRectMethod == null || GetTopRect == null || ClipRect == null)
                {
                    compabilityMode = true;
                    initiated = true;
                    return;
                }

                GetTopRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), GetTopRect);
                topmostRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), null, topmostRectMethod);
                if (GetTopRectDelegate == null || topmostRectDelegate == null)
                {
                    compabilityMode = true;
                    initiated = true;
                    return;
                }
            }
            else
            {
                MethodInfo GetTopRect = GUIClipType.GetMethod("GetTopRect", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo ClipRect = GUIClipType.GetMethod("Clip", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new Type[] { typeof(Rect) }, new ParameterModifier[] { });

                if (GUIClipType == null || topmostRect == null || GetTopRect == null || ClipRect == null)
                {
                    compabilityMode = true;
                    initiated = true;
                    return;
                }

                GetTopRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), GetTopRect);
                topmostRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), topmostRect.GetGetMethod());
                if (GetTopRectDelegate == null || topmostRectDelegate == null)
                {
                    compabilityMode = true;
                    initiated = true;
                    return;
                }
            }

            currentRectStack = new List<Rect>();
            rectStackGroups = new List<List<Rect>>();
            GUIMatrices = new List<Matrix4x4>();
            adjustedGUILayout = new List<bool>();

            initiated = true;
        }

        #endregion

        #region Scale Area

        public static Vector2 getCurrentScale { get { return new Vector2(1 / GUI.matrix.GetColumn(0).magnitude, 1 / GUI.matrix.GetColumn(1).magnitude); } }

        public static Vector2 BeginScale(ref Rect rect, Vector2 zoomPivot, float zoom, bool adjustGUILayout)
        {
            Rect screenRect;
            if (compabilityMode)
            {
                GUI.EndGroup();
                screenRect = rect;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    screenRect.y += 23;
#endif
            }
            else
            {
                GUIScaleUtility.BeginNoClip();
                screenRect = GUIScaleUtility.GUIToScaledSpace(rect);
            }

            rect = Scale(screenRect, screenRect.position + zoomPivot, new Vector2(zoom, zoom));


            GUI.BeginGroup(rect);
            rect.position = Vector2.zero;

            Vector2 zoomPosAdjust = rect.center - screenRect.size / 2 + zoomPivot;

            adjustedGUILayout.Add(adjustGUILayout);
            if (adjustGUILayout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(rect.center.x - screenRect.size.x + zoomPivot.x);
                GUILayout.BeginVertical();
                GUILayout.Space(rect.center.y - screenRect.size.y + zoomPivot.y);
            }

            GUIMatrices.Add(GUI.matrix);

            GUIUtility.ScaleAroundPivot(new Vector2(1 / zoom, 1 / zoom), zoomPosAdjust);

            return zoomPosAdjust;
        }
        public static void EndScale()
        {
            if (GUIMatrices.Count == 0 || adjustedGUILayout.Count == 0)
                throw new UnityException("GUIScaleUtility: You are ending more scale regions than you are beginning!");
            GUI.matrix = GUIMatrices[GUIMatrices.Count - 1];
            GUIMatrices.RemoveAt(GUIMatrices.Count - 1);

            if (adjustedGUILayout[adjustedGUILayout.Count - 1])
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            adjustedGUILayout.RemoveAt(adjustedGUILayout.Count - 1);

            GUI.EndGroup();

            if (compabilityMode)
            {
                if (!Application.isPlaying)
                    GUI.BeginClip(new Rect(0, 23, Screen.width, Screen.height - 23));
                else
                    GUI.BeginClip(new Rect(0, 0, Screen.width, Screen.height));
            }
            else
            {
                GUIScaleUtility.RestoreClips();
            }
        }

        #endregion

        #region Clips Hierarchy

        public static void BeginNoClip()
        {
            List<Rect> rectStackGroup = new List<Rect>();
            Rect topMostClip = getTopRect;
            while (topMostClip != new Rect(-10000, -10000, 40000, 40000))
            {
                rectStackGroup.Add(topMostClip);
                GUI.EndClip();
                topMostClip = getTopRect;
            }
            rectStackGroup.Reverse();
            rectStackGroups.Add(rectStackGroup);
            currentRectStack.AddRange(rectStackGroup);
        }

        public static void MoveClipsUp(int count)
        {
            List<Rect> rectStackGroup = new List<Rect>();
            Rect topMostClip = getTopRect;
            while (topMostClip != new Rect(-10000, -10000, 40000, 40000) && count > 0)
            {
                rectStackGroup.Add(topMostClip);
                GUI.EndClip();
                topMostClip = getTopRect;
                count--;
            }
            rectStackGroup.Reverse();
            rectStackGroups.Add(rectStackGroup);
            currentRectStack.AddRange(rectStackGroup);
        }

        public static void RestoreClips()
        {
            if (rectStackGroups.Count == 0)
            {
                return;
            }

            List<Rect> rectStackGroup = rectStackGroups[rectStackGroups.Count - 1];
            for (int clipCnt = 0; clipCnt < rectStackGroup.Count; clipCnt++)
            {
                GUI.BeginClip(rectStackGroup[clipCnt]);
                currentRectStack.RemoveAt(currentRectStack.Count - 1);
            }
            rectStackGroups.RemoveAt(rectStackGroups.Count - 1);
        }

        #endregion

        #region Layout & Matrix Ignores

        public static void BeginNewLayout()
        {
            if (compabilityMode)
                return;
            Rect topMostClip = getTopRect;
            if (topMostClip != new Rect(-10000, -10000, 40000, 40000))
                GUILayout.BeginArea(new Rect(0, 0, topMostClip.width, topMostClip.height));
            else
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        }

        public static void EndNewLayout()
        {
            if (!compabilityMode)
                GUILayout.EndArea();
        }

        public static void BeginIgnoreMatrix()
        {
            GUIMatrices.Add(GUI.matrix);
            GUI.matrix = Matrix4x4.identity;
        }

        public static void EndIgnoreMatrix()
        {
            if (GUIMatrices.Count == 0)
                throw new UnityException("GUIScaleutility: You are ending more ignoreMatrices than you are beginning!");
            GUI.matrix = GUIMatrices[GUIMatrices.Count - 1];
            GUIMatrices.RemoveAt(GUIMatrices.Count - 1);
        }

        #endregion

        #region Space Transformations

        public static Vector2 Scale(Vector2 pos, Vector2 pivot, Vector2 scale)
        {
            return Vector2.Scale(pos - pivot, scale) + pivot;
        }

        public static Rect Scale(Rect rect, Vector2 pivot, Vector2 scale)
        {
            rect.position = Vector2.Scale(rect.position - pivot, scale) + pivot;
            rect.size = Vector2.Scale(rect.size, scale);
            return rect;
        }

        public static Vector2 ScaledToGUISpace(Vector2 scaledPosition)
        {
            if (rectStackGroups == null || rectStackGroups.Count == 0)
                return scaledPosition;
            List<Rect> rectStackGroup = rectStackGroups[rectStackGroups.Count - 1];
            for (int clipCnt = 0; clipCnt < rectStackGroup.Count; clipCnt++)
                scaledPosition -= rectStackGroup[clipCnt].position;
            return scaledPosition;
        }
        public static Rect ScaledToGUISpace(Rect scaledRect)
        {
            if (rectStackGroups == null || rectStackGroups.Count == 0)
                return scaledRect;
            scaledRect.position = ScaledToGUISpace(scaledRect.position);
            return scaledRect;
        }

        public static Vector2 GUIToScaledSpace(Vector2 guiPosition)
        {
            if (rectStackGroups == null || rectStackGroups.Count == 0)
                return guiPosition;
            List<Rect> rectStackGroup = rectStackGroups[rectStackGroups.Count - 1];
            for (int clipCnt = 0; clipCnt < rectStackGroup.Count; clipCnt++)
                guiPosition += rectStackGroup[clipCnt].position;
            return guiPosition;
        }
        public static Rect GUIToScaledSpace(Rect guiRect)
        {
            if (rectStackGroups == null || rectStackGroups.Count == 0)
                return guiRect;
            guiRect.position = GUIToScaledSpace(guiRect.position);
            return guiRect;
        }

        public static Vector2 GUIToScreenSpace(Vector2 guiPosition)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return guiPosition + getTopRectScreenSpace.position - new Vector2(0, 22);
#endif
            return guiPosition + getTopRectScreenSpace.position;
        }

        public static Rect GUIToScreenSpace(Rect guiRect)
        {
            guiRect.position += getTopRectScreenSpace.position;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                guiRect.y -= 22;
#endif
            return guiRect;
        }

        #endregion
    }
}
