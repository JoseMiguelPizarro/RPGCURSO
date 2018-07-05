using System;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;

using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;


namespace _ShaderoShaderEditorFramework.Standard
{
    public class NodeEditorWindow : EditorWindow
    {
         private static NodeEditorWindow _editor;
        public static NodeEditorWindow editor { get { AssureEditor(); return _editor; } }
        public static void AssureEditor() { if (_editor == null) OpenNodeEditor(); }

        public static NodeEditorUserCache canvasCache;

        private Rect loadSceneUIPos;
        private Rect createCanvasUIPos;
        private int sideWindowWidth = 400;

        public Rect sideWindowRect { get { return new Rect(position.width - sideWindowWidth, 0, sideWindowWidth, position.height); } }
        public Rect canvasWindowRect { get { return new Rect(0, 0, position.width - sideWindowWidth, position.height); } }

        public bool PreviewLiveAnimation = true;

        public bool UseOwnMaterial = false;

        public bool ShowTutorial = false;
        public int ShowTutorialCurrent = 0;

        private string MaterialChange;
        #region General 

          [MenuItem("Window/Shadero Sprite Shader Editor")]
        public static NodeEditorWindow OpenNodeEditor()
        {
            _editor = GetWindow<NodeEditorWindow>();
        
            NodeEditor.ReInit(false);

            Texture iconTexture = ResourceManager.LoadTexture(EditorGUIUtility.isProSkin ? "Textures/Icon_Dark.png" : "Textures/Icon_Light.png");
            _editor.titleContent = new GUIContent("Shadero Sprite Shader Editor", iconTexture);

            return _editor;
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        private static bool AutoOpenCanvas(int instanceID, int line)
        {
            if (Selection.activeObject != null && Selection.activeObject is NodeCanvas)
            {
                string NodeCanvasPath = AssetDatabase.GetAssetPath(instanceID);
                NodeEditorWindow.OpenNodeEditor();
                canvasCache.LoadNodeCanvas(NodeCanvasPath);
                return true;
            }
            return false;
        }

        private void OnEnable()
        {

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            PreviewLiveAnimation = true;
            _editor = this;
            NodeEditor.checkInit(false);

            NodeEditor.ClientRepaints -= Repaint;
            NodeEditor.ClientRepaints += Repaint;

            EditorLoadingControl.justLeftPlayMode -= NormalReInit;
            EditorLoadingControl.justLeftPlayMode += NormalReInit;
            EditorLoadingControl.justOpenedNewScene -= NormalReInit;
            EditorLoadingControl.justOpenedNewScene += NormalReInit;

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;

            // Fix for .Net 4
            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
            path = path.Replace("\\", "/");
            canvasCache = new NodeEditorUserCache(path);
            canvasCache.SetupCacheEvents();
        }

        private void NormalReInit()
        {
            NodeEditor.ReInit(false);
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(canvasCache.nodeCanvas);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            NodeEditor.ClientRepaints -= Repaint;

            EditorLoadingControl.justLeftPlayMode -= NormalReInit;
            EditorLoadingControl.justOpenedNewScene -= NormalReInit;

            SceneView.onSceneGUIDelegate -= OnSceneGUI;

            canvasCache.ClearCacheEvents();
        }

        #endregion

        #region GUI

        private void OnSceneGUI(SceneView sceneview)
        {
            DrawSceneGUI();
        }

        private void DrawSceneGUI()
        {
            if (canvasCache.editorState.selectedNode != null)
                canvasCache.editorState.selectedNode.OnSceneGUI();
            SceneView.lastActiveSceneView.Repaint();
        }
        private int bframe = 0;
        private float bframecount = 0;
        private void OnGUI()
        {
            GUI.enabled = !ShowTutorial;
            NodeEditor.NoBuildShader = false;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (NodeEditor.AutoUpdate) NodeEditor.RecalculateFlag = true;
            }

            NodeEditor.checkInit(true);
            if (NodeEditor.InitiationError)
            {
                GUILayout.Label("Shadero Sprite Shader Editor Initiation failed! Check console for more information!");
                return;
            }
            AssureEditor();
            canvasCache.AssureCanvas();

             canvasCache.editorState.canvasRect = canvasWindowRect;

            NodeEditorGUI.StartNodeGUI();

            try
            {
                NodeEditor.DrawCanvas(canvasCache.nodeCanvas, canvasCache.editorState);
            }
            catch (UnityException e)
            {    canvasCache.NewNodeCanvas();
                NodeEditor.ReInit(true);
                Debug.LogError("Unloaded Canvas due to an exception during the drawing phase!");
                Debug.LogException(e);
            }


        

            if (NodeEditor.NoBuildShader)
            {
                if (!NodeEditor.FlagIsSaved)
                {
                    float x = canvasCache.editorState.panOffset.x + NodeEditor.BuildShaderPosX;
                    float y = canvasCache.editorState.panOffset.y + NodeEditor.BuildShaderPosY;

                    x /= canvasCache.editorState.zoom;
                    y /= canvasCache.editorState.zoom;
                    x -= 110;
                    y -= 20;
                    x += position.width * 0.4f;
                    y += position.height * 0.4f;
                    y += (90 / canvasCache.editorState.zoom);

                    Texture2D preview = ResourceManager.LoadTexture("Help/arrow.png");
                    GUI.DrawTexture(new Rect(x, y, 128, 100), preview);

                    bframecount += Time.fixedDeltaTime * 0.25f;
                    if (bframecount > 1) { bframe++; bframecount = 0; }
                    if (bframe >= 16) bframe = 0;


                    preview = ResourceManager.LoadTexture("Help/help_light.png");

                    x -= 420;
                    y -= 100;

                    GUI.DrawTexture(new Rect(x-24, y-24, 410+48, 363+48), preview);


                    if (bframe == 0) preview = ResourceManager.LoadTexture("Help/Init_build_anm_1.jpg");
                    if (bframe == 1) preview = ResourceManager.LoadTexture("Help/Init_build_anm_2.jpg");
                    if (bframe == 2) preview = ResourceManager.LoadTexture("Help/Init_build_anm_3.jpg");
                    if (bframe == 3) preview = ResourceManager.LoadTexture("Help/Init_build_anm_4.jpg");
                    if (bframe == 4) preview = ResourceManager.LoadTexture("Help/Init_build_anm_5.jpg");
                    if (bframe == 5) preview = ResourceManager.LoadTexture("Help/Init_build_anm_6.jpg");
                    if (bframe == 6) preview = ResourceManager.LoadTexture("Help/Init_build_anm_7.jpg");
                    if (bframe == 7) preview = ResourceManager.LoadTexture("Help/Init_build_anm_8.jpg");
                    if (bframe == 8) preview = ResourceManager.LoadTexture("Help/Init_build_anm_9.jpg");
                    if (bframe == 9) preview = ResourceManager.LoadTexture("Help/Init_build_anm_10.jpg");
                    if (bframe == 10) preview = ResourceManager.LoadTexture("Help/Init_build_anm_11.jpg");
                    if (bframe == 11) preview = ResourceManager.LoadTexture("Help/Init_build_anm_12.jpg");
                    if (bframe == 12) preview = ResourceManager.LoadTexture("Help/Init_build_anm_13.jpg");
                    if (bframe >= 13) preview = ResourceManager.LoadTexture("Help/Init_build_anm_14.jpg");
                   
                
                  
                    GUI.DrawTexture(new Rect(x, y, 410, 363), preview);
                }
            }

             sideWindowWidth = Math.Min(205, Math.Max(205, (int)(position.width / 5)));
            GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
            DrawSideWindow();
            GUILayout.EndArea();



            if (GUI.Button(new Rect(10,75,200,20),new GUIContent("Help Tutorial")))
            {
                ShowTutorial = !ShowTutorial;
            }
           
            if (ShowTutorial)
            {
                GUI.enabled = ShowTutorial;
                Texture2D preview = ResourceManager.LoadTexture("Help/tut_back.png");
                float x = position.width;
                float y = position.height;
                float sx = 926;
                float sy = 490;
                Rect r = new Rect(x / 2 - sx / 2, y / 2 - sy / 2, sx, sy);
                GUI.DrawTexture(r, preview);
                float Bsize = 127;
                float Bpos = 60;
                float Bheight = 35;
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22, (y / 2 + sy / 2) - Bpos, Bsize-5, Bheight), new GUIContent("1. Basic part 1")))
                {
                    ShowTutorialCurrent = 0;
                }
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22 + Bsize, (y / 2 + sy / 2) - Bpos, Bsize-5, Bheight), new GUIContent("2. Basic part 2")))
                {
                    ShowTutorialCurrent = 1;
                }
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22 + Bsize * 2, (y / 2 + sy / 2) - Bpos, Bsize-5, Bheight), new GUIContent("3. The UV")))
                {
                    ShowTutorialCurrent = 2;
                }
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22 + Bsize * 3, (y / 2 + sy / 2) - Bpos, Bsize-5, Bheight), new GUIContent("4. Use Shader")))
                {
                    ShowTutorialCurrent = 3;
                }
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22 + Bsize * 4, (y / 2 + sy / 2) - Bpos, Bsize-5, Bheight), new GUIContent("5. Parameters")))
                {
                    ShowTutorialCurrent = 4;
                }
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22 + Bsize * 5, (y / 2 + sy / 2) - Bpos, Bsize - 5, Bheight), new GUIContent("6. HDR")))
                {
                    ShowTutorialCurrent = 5;
                }
                if (GUI.Button(new Rect((x / 2 - sx / 2) + 22 + Bsize * 6, (y / 2 + sy / 2) - Bpos, Bsize - 5, Bheight), new GUIContent("OK")))
                {
                    ShowTutorial = !ShowTutorial;
                }
                if (GUI.Button(new Rect(10, 75, 200, 20), new GUIContent("Help Tutorial")))
                {
                    ShowTutorial = !ShowTutorial;
                }

                if (ShowTutorialCurrent == 0) preview = ResourceManager.LoadTexture("Help/tut_img1.jpg");
                if (ShowTutorialCurrent == 1) preview = ResourceManager.LoadTexture("Help/tut_img2.jpg");
                if (ShowTutorialCurrent == 2) preview = ResourceManager.LoadTexture("Help/tut_img3.jpg");
                if (ShowTutorialCurrent == 3) preview = ResourceManager.LoadTexture("Help/tut_img4.jpg");
                if (ShowTutorialCurrent == 4) preview = ResourceManager.LoadTexture("Help/tut_img5.jpg");
                if (ShowTutorialCurrent == 5) preview = ResourceManager.LoadTexture("Help/tut_img6.jpg");
                r.x += 23;
                r.y += 53;
                r.width = 875;
                r.height = 372;
                GUI.DrawTexture(r, preview);
            }

            NodeEditorGUI.EndNodeGUI();
            NodeEditor.NoBuildShaderContext = NodeEditor.NoBuildShader;
        }
        float FullScreenSet;
     
        private void DrawSideWindow()
        {
            GUILayout.Label("Shadero Sprite v1.9.0", NodeEditorGUI.nodeLabelBold);

            if (NodeEditor._Shadero_Material == null)
            {
                Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Preview_no_material.jpg");
                GUI.DrawTexture(new Rect(5, 30, 196, 196), preview);
            }
            else
            {
           
                if (!NodeEditor.NoBuildShader)
                 {

                    Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Preview_no_buildshader.jpg");
                    GUI.DrawTexture(new Rect(5, 30, 196, 196), preview);

                }
                else
                {
                    if (!NodeEditor.FlagIsSaved)
                     {
                        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Preview_no_shader.jpg");
                         GUI.DrawTexture(new Rect(5, 30, 196, 196), preview);

                    }
                    else
                    {
                        if (!NodeEditor.RGBAonBuildShader)
                        {
                            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Preview_no_rgba_buildshader.jpg");
                            GUI.DrawTexture(new Rect(5, 30, 196, 196), preview);

                        }
                        else
                        {
                            NodeEditor._Shadero_Material.shader = Shader.Find("Shadero Customs/" + Node.ShaderNameX);
                            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Preview_Shader.jpg");
                            GUI.DrawTexture(new Rect(5, 30, 196, 196), preview);

                            Texture tex = NodeEditor._Shadero_Material.mainTexture;
                            if (tex == null) tex = ResourceManager.LoadTexture("Textures/previews/Preview_Shader.jpg");
                            float x = Event.current.mousePosition.x - 8;
                            float y = Event.current.mousePosition.y - 33;
                            x = x / 190;
                            y = y / 190;

                            if ((x > 0 && x < 1) && (y > 0 && y < 1))
                            {
                                EditorGUI.DrawPreviewTexture(new Rect(8, 33, 190, 190), tex, NodeEditor._Shadero_Material);

                                if (FullScreenSet > 3)
                                {
                                    EditorGUI.DrawPreviewTexture(new Rect(-(Screen.width / 2) - Screen.height / 4, 10, Screen.height, Screen.height), tex, NodeEditor._Shadero_Material);
                                }

                                FullScreenSet += Time.deltaTime;
                            }
                            else
                            {
                                FullScreenSet = 0;
                                EditorGUI.DrawPreviewTexture(new Rect(8, 33, 190, 190), tex, NodeEditor._Shadero_Material);
                            }
                        }
                    }
                }
            }
            GUILayout.Space(200);

            GUIStyle g = new GUIStyle();

            if (NodeEditor._Shadero_Material != null)
            {
                int ms = g.fontSize;
                g.fontSize = 10;
                g.alignment = TextAnchor.UpperRight;
                g.normal.textColor = Color.white;
                GUILayout.Label("*Preview Material", g);
                g.fontSize = ms;
            }
            else
            {
                GUILayout.Label("Preview Material");
            }
            PreviewLiveAnimation = GUILayout.Toggle(PreviewLiveAnimation, "Active live animation");
            if (PreviewLiveAnimation) NodeEditor.RepaintClients();

            g = new GUIStyle();
            g.fontSize = 12;
            g.normal.textColor = Color.white;

            UseOwnMaterial = GUILayout.Toggle(UseOwnMaterial, "Use your own Material");

            if (!UseOwnMaterial)
            {
                NodeEditor._Shadero_Material = AssetDatabase.LoadAssetAtPath("Assets/ShaderoSprite/Plugins/EditorResources/Preview/PreviewMaterial.mat", typeof(Material)) as Material;
            }
            else
            {
                GUILayout.Space(25);
                NodeEditor._Shadero_Material = (Material)EditorGUI.ObjectField(new Rect(5, 285, 200, 18), NodeEditor._Shadero_Material, typeof(Material), true);
                if (NodeEditor._Shadero_Material == null) UseOwnMaterial = false;
            }

            if (MaterialChange == null) MaterialChange = AssetDatabase.GetAssetPath(NodeEditor._Shadero_Material);
            if (MaterialChange != AssetDatabase.GetAssetPath(NodeEditor._Shadero_Material))
            {
                NodeEditor.FlagIsMaterialChanged = true;
                NodeEditor.ShaderToNull=true;
            } 
            MaterialChange = AssetDatabase.GetAssetPath(NodeEditor._Shadero_Material);
              GUILayout.Space(10);

            if (NodeEditor.RecalculateFlag)
            {
                NodeEditor.ForceWriteFlag = true;
                NodeEditor.RecalculateAll(canvasCache.nodeCanvas);
                NodeEditor.RecalculateFlag = false;
            }

            if (GUILayout.Button(new GUIContent("New Shader Project", "Loads an Specified Empty Shadero project")))
            {
                _ShaderoShaderEditorFramework.Utilities.GenericMenu menu = new _ShaderoShaderEditorFramework.Utilities.GenericMenu();
                NodeCanvasManager.FillCanvasTypeMenu(ref menu, canvasCache.NewNodeCanvas);
                menu.Show(createCanvasUIPos.position, createCanvasUIPos.width);
             }
            if (NodeEditor.NewCanvasWasCalled)
            {
                UseOwnMaterial = false;
                NodeEditor._Shadero_Material = null;
                NodeEditor.NewCanvasWasCalled = false;
            }

            if (Event.current.type == EventType.Repaint)
            {
                Rect popupPos = GUILayoutUtility.GetLastRect();
                createCanvasUIPos = new Rect(popupPos.x + 2, popupPos.yMax + 2, popupPos.width - 4, 0);
            }

            GUILayout.Space(6);
       
            if (GUILayout.Button(new GUIContent("Save Shader Project", "Saves the project to a project Save File in the Assets Folder")))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save Shadero Shader Project", "Shadero Shader Project", "asset", "", NodeEditor.editorPath + "Shadero_Projects/");
                if (!string.IsNullOrEmpty(path))
                {
                    canvasCache.SaveNodeCanvas(path);
                    // NodeEditor.FlagIsSavedMaterial = true;
                }
            }

            if (GUILayout.Button(new GUIContent("Load Shader Project", "Loads the project from a project Save File in the Assets Folder")))
            {
                string path = EditorUtility.OpenFilePanel("Load Shadero Shader Project", NodeEditor.editorPath + "Shadero_Projects/", "asset");
                if (!path.Contains(Application.dataPath))
                {
                    if (!string.IsNullOrEmpty(path))
                        ShowNotification(new GUIContent("You should select an asset inside your project folder!"));
                }
                else
                {
                    UseOwnMaterial = false;
                    canvasCache.LoadNodeCanvas(path);
               }
            }

            GUILayout.Space(6);

            g = new GUIStyle();
            g.fontSize = 12;
            g.alignment = TextAnchor.LowerLeft;
            g.normal.textColor = Color.white;

            if (!NodeEditor.NoBuildShader)
            {

                GUILayout.Label(" ", g);
                GUILayout.Label("What to do ?", g);
                GUILayout.Label(" ", g);
                GUILayout.Label("- You need to create a ", g);
                GUILayout.Label("  Build Shader \"Node Shader\"", g);
                GUILayout.Label("  , Right Click on the", g);
                GUILayout.Label("  Node Canvas and Select ", g);
                GUILayout.Label("  \"Add Build Shader\".", g);


            }
            else
            {
                GUILayout.Label(" ", g);
                GUILayout.Label("What to do ?", g);
                GUILayout.Label(" ", g);
                GUILayout.Label("-If you want to use your ", g);
                GUILayout.Label("  own material, active ", g);
                GUILayout.Label(" the \"Use your own material\" )", g);
                GUILayout.Label(" button. ", g);
                GUILayout.Label("", g);
                GUILayout.Label("  We recommand you to always ", g);
                GUILayout.Label("  active the \"Active live animation\"", g);
                GUILayout.Label("  for a better experience.", g);


            }
          
            if (canvasCache.editorState.selectedNode != null && Event.current.type != EventType.Ignore)
                canvasCache.editorState.selectedNode.DrawNodePropertyEditor();
        }

        public void LoadSceneCanvasCallback(object canvas)
        {
            canvasCache.LoadSceneNodeCanvas((string)canvas);
        }

        #endregion
    }
}
