using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
using UnityEditor;
using Object = UnityEngine.Object;

namespace _ShaderoShaderEditorFramework
{
    public static class NodeEditor
    {
        public static string editorPath = "Assets/ShaderoSprite/Plugins/";

        public static Material _Shadero_Material;
        public static bool FlagIsSaved;
        public static bool FlagIsSavedMaterial = false;
        public static bool FlagIsLoadedMaterial = false;
        public static bool FlagIsMaterialChanged = false;

        public static bool RGBAonBuildShader = false;
        public static bool NoBuildShader;
        public static bool NoBuildShaderContext;

        public static float BuildShaderPosX;
        public static float BuildShaderPosY;

        public static bool ShaderToNull = false;

        public static bool NewCanvasWasCalled = false;

        public static NodeCanvas curNodeCanvas;
        public static NodeEditorState curEditorState;

        internal static Action NEUpdate;
        public static void Update() { if (NEUpdate != null) NEUpdate(); }
        public static Action ClientRepaints;

        public static bool RecalculateFlag;
        public static bool ForceWriteFlag;
        public static bool AutoUpdate = true;
        public static bool AutoUpdateFlag;
        public static bool AutoUpdateBuildFlag;
        public static void RepaintClients() { if (ClientRepaints != null) ClientRepaints(); }

        #region Setup


        private static bool initiatedBase;
        private static bool initiatedGUI;
        public static bool InitiationError;

        public static float UpdatePreview = 1;
        public static void checkInit(bool GUIFunction)
        {
            if (!InitiationError)
            {
                if (!initiatedBase)
                    setupBaseFramework();
                if (GUIFunction && !initiatedGUI)
                    setupGUI();
            }
        }

        public static void ReInit(bool GUIFunction)
        {
            InitiationError = initiatedBase = initiatedGUI = false;

            setupBaseFramework();
            if (GUIFunction)
                setupGUI();
        }

        private static void setupBaseFramework()
        {

            ResourceManager.SetDefaultResourcePath(editorPath + "EditorResources/");

            ConnectionTypes.FetchTypes();
            NodeTypes.FetchNodes();
            NodeCanvasManager.GetAllCanvasTypes();

            NodeEditorCallbacks.SetupReceivers();
            NodeEditorCallbacks.IssueOnEditorStartUp();

            NodeEditorInputSystem.SetupInput();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
            UnityEditor.EditorApplication.update += Update;
#endif

            initiatedBase = true;
        }

        private static void setupGUI()
        {
            if (!initiatedBase)
                setupBaseFramework();
            initiatedGUI = false;

            GUIScaleUtility.CheckInit();

            if (!NodeEditorGUI.Init())
            {
                InitiationError = true;
                return;
            }

#if UNITY_EDITOR
            RepaintClients();
#endif

            initiatedGUI = true;
        }


        #endregion


        [MenuItem("Assets/Create/Shadero Sprite Shader", false, 11)]
        public static void CreateShader()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string text = "";
            text += "//////////////////////////////////////////////////////////////\n";
            text += "/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //\n";
            text += "/// Shader generate with Shadero 1.9.0                      //\n";
            text += "/// http://u3d.as/V7t #AssetStore                           //\n";
            text += "/// http://www.shadero.com #Docs                            //\n";
            text += "//////////////////////////////////////////////////////////////\n";
            text += "\n";
            text += "Shader \"hidden/ShaderoCustoms/EmptyShader\"\n";
            text += "{\n";
            text += "Properties\n";
            text += "{\n";
            text += "[HideInInspector] _MainTex(\"Base (RGB)\", 2D) = \"white\" {}\n";
            text += "_SpriteFade(\"SpriteFade\", Range(0, 1)) = 1.0\n";
            text += "\n";
            text += "// required for UI.Mask\n";
            text += "[HideInInspector]_StencilComp(\"Stencil Comparison\", Float) = 8\n";
            text += "[HideInInspector]_Stencil(\"Stencil ID\", Float) = 0\n";
            text += "[HideInInspector]_StencilOp(\"Stencil Operation\", Float) = 0\n";
            text += "[HideInInspector]_StencilWriteMask(\"Stencil Write Mask\", Float) = 255\n";
            text += "[HideInInspector]_StencilReadMask(\"Stencil Read Mask\", Float) = 255\n";
            text += "[HideInInspector]_ColorMask(\"Color Mask\", Float) = 15\n";
            text += "\n";
            text += "}\n";
            text += "\n";
            text += "SubShader\n";
            text += "{\n";
            text += "\n";
            text += "Tags {\"Queue\" = \"Transparent\" \"IgnoreProjector\" = \"true\" \"RenderType\" = \"Transparent\"}\n";
            text += "ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off\n";
            text += "\n";
            text += "// required for UI.Mask\n";
            text += "Stencil\n";
            text += "{\n";
            text += "Ref [_Stencil]\n";
            text += "Comp [_StencilComp]\n";
            text += "Pass [_StencilOp]\n";
            text += "ReadMask [_StencilReadMask]\n";
            text += "WriteMask [_StencilWriteMask]\n";
            text += "}\n";
            text += "\n";
            text += "Pass\n";
            text += "{\n";
            text += "\n";
            text += "CGPROGRAM\n";
            text += "#pragma vertex vert\n";
            text += "#pragma fragment frag\n";
            text += "#pragma fragmentoption ARB_precision_hint_fastest\n";
            text += "# include \"UnityCG.cginc\"\n";
            text += "\n";
            text += "struct appdata_t{\n";
            text += "float4 vertex   : POSITION;\n";
            text += "float4 color    : COLOR;\n";
            text += "float2 texcoord : TEXCOORD0;\n";
            text += "};\n";
            text += "\n";
            text += "struct v2f\n";
            text += "{\n";
            text += "float2 texcoord  : TEXCOORD0;\n";
            text += "float4 vertex   : SV_POSITION;\n";
            text += "float4 color    : COLOR;\n";
            text += "};\n";
            text += "\n";
            text += "sampler2D _MainTex;\n";
            text += "float _SpriteFade;\n";
            text += "\n";
            text += "v2f vert(appdata_t IN)\n";
            text += "{\n";
            text += "v2f OUT;\n";
            text += "OUT.vertex = UnityObjectToClipPos(IN.vertex);\n";
            text += "OUT.texcoord = IN.texcoord;\n";
            text += "OUT.color = IN.color;\n";
            text += "return OUT;\n";
            text += "}\n";
            text += "\n";
            text += "float4 frag (v2f i) : COLOR\n";
            text += "{\n";
            text += "float4 FinalResult = tex2D(_MainTex, i.texcoord);\n";
            text += "FinalResult.rgb *= i.color.rgb;\n";
            text += "FinalResult.a = FinalResult.a * _SpriteFade;\n";
            text += "return FinalResult;\n";
            text += "}\n";
            text += "\n";
            text += "ENDCG\n";
            text += "}\n";
            text += "}\n";
            text += "Fallback \"Sprites /Default\"\n";
            text += "}\n";
            text += "\n";


            File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(path + "/New Shadero Sprite Shader.shader"), text);
            AssetDatabase.Refresh();
        }


        #region GUI

        public static void DrawCanvas(NodeCanvas nodeCanvas, NodeEditorState editorState)
        {
            if (!editorState.drawing)
                return;
            checkInit(true);

            DrawSubCanvas(nodeCanvas, editorState);
        }

           private static void DrawSubCanvas(NodeCanvas nodeCanvas, NodeEditorState editorState)
        {
            if (!editorState.drawing)
                return;

            NodeCanvas prevNodeCanvas = curNodeCanvas;
            NodeEditorState prevEditorState = curEditorState;
            curNodeCanvas = nodeCanvas;
            curEditorState = editorState;



            if (Event.current.type == EventType.Repaint)
            {    float width = curEditorState.zoom / NodeEditorGUI.Background.width;
                float height = curEditorState.zoom / NodeEditorGUI.Background.height;
                Vector2 offset = curEditorState.zoomPos + curEditorState.panOffset / curEditorState.zoom;
                Rect uvDrawRect = new Rect(-offset.x * width,
                    (offset.y - curEditorState.canvasRect.height) * height,
                    curEditorState.canvasRect.width * width,
                    curEditorState.canvasRect.height * height);
                GUI.DrawTextureWithTexCoords(curEditorState.canvasRect, NodeEditorGUI.Background, uvDrawRect);
            }

            NodeEditorInputSystem.HandleInputEvents(curEditorState);
            if (Event.current.type != EventType.Layout)
                curEditorState.ignoreInput = new List<Rect>();

            Rect canvasRect = curEditorState.canvasRect;
            curEditorState.zoomPanAdjust = GUIScaleUtility.BeginScale(ref canvasRect, curEditorState.zoomPos, curEditorState.zoom, false);
            if (curEditorState.navigate)
            {   Vector2 startPos = (curEditorState.selectedNode != null ? curEditorState.selectedNode.rect.center : curEditorState.panOffset) + curEditorState.zoomPanAdjust;
                Vector2 endPos = Event.current.mousePosition;
                RTEditorGUI.DrawLine(startPos, endPos, Color.green, null, 3);
                RepaintClients();
            }

            if (curEditorState.connectOutput != null)
            {   NodeOutput output = curEditorState.connectOutput;
                Vector2 startPos = output.GetGUIKnob().center;
                Vector2 startDir = output.GetDirection();
                Vector2 endPos = Event.current.mousePosition;
                Vector2 endDir = NodeEditorGUI.GetSecondConnectionVector(startPos, endPos, startDir);
                NodeEditorGUI.DrawConnection(startPos, startDir, endPos, endDir, output.typeData.Color);
                RepaintClients();
            }

            if (Event.current.type == EventType.Layout && curEditorState.selectedNode != null)
            {
                curNodeCanvas.nodes.Remove(curEditorState.selectedNode);
                curNodeCanvas.nodes.Add(curEditorState.selectedNode);
            }

            for (int nodeCnt = 0; nodeCnt < curNodeCanvas.nodes.Count; nodeCnt++)
                curNodeCanvas.nodes[nodeCnt].DrawConnections();

            for (int nodeCnt = 0; nodeCnt < curNodeCanvas.nodes.Count; nodeCnt++)
            {
                Node node = curNodeCanvas.nodes[nodeCnt];
                node.DrawNode();
                if (Event.current.type == EventType.Repaint)
                    node.DrawKnobs();
            }

            GUIScaleUtility.EndScale();

            GUI.DrawTexture(new Rect(12, 12, NodeEditorGUI.GuiShadero.width / 2, NodeEditorGUI.GuiShadero.height / 2), NodeEditorGUI.GuiShadero);
            GUIStyle g = new GUIStyle();
            g.fontSize = 26;
            g.normal.textColor = Color.white;
            if (Node.ShaderNameX == "") Node.ShaderNameX = "Default";
            g = new GUIStyle();
            g.fontSize = 22;

            if (FlagIsLoadedMaterial)
            {
                FlagIsLoadedMaterial = false;
                Debug.Log("Loaded = " + curEditorState.ShaderName);
            }
            if (FlagIsSavedMaterial)
            {
                FlagIsSavedMaterial = false;
                Debug.Log("Saved = " + curEditorState.ShaderName);
            }


            g = new GUIStyle();
            g.fontSize = 18;
            g.normal.textColor = Color.white;


            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/shadero_firstscreen.jpg");

            g = new GUIStyle();
            g.fontSize = 18;
            g.fontStyle = FontStyle.Italic;
            Color yellow = new Color(0, 1, 1, UpdatePreview);
            UpdatePreview -= Time.deltaTime * 0.2f;
            if (UpdatePreview < 0) UpdatePreview = 0;
            g.normal.textColor = yellow;

            Rect position = _ShaderoShaderEditorFramework.Standard.NodeEditorWindow.editor.position;
            GUI.Label(new Rect(position.width - 320, position.height - 50, 150, 50), "*Updated*", g);

            Node.ShaderNameX = NodeEditor.curEditorState.ShaderName;

          
            if (curNodeCanvas.nodes.Count == 0)
            {
                preview = ResourceManager.LoadTexture("Textures/previews/shadero_firstscreen.jpg");
                Vector2 scr = new Vector2(position.width / 2 - 120, position.height / 2);
                Vector2 size = new Vector2(1365 / 2, 781 / 2);
                GUI.DrawTexture(new Rect(scr.x - size.x / 2, scr.y - size.y / 2, size.x, size.y), preview);

            }

            NodeEditorInputSystem.HandleLateInputEvents(curEditorState);
            curNodeCanvas = prevNodeCanvas;
            curEditorState = prevEditorState;
        }

        #endregion

        #region Space Transformations

         public static Node NodeAtPosition(Vector2 canvasPos)
        {
            NodeKnob focusedKnob;
            return NodeAtPosition(curEditorState, canvasPos, out focusedKnob);
        }

          public static Node NodeAtPosition(Vector2 canvasPos, out NodeKnob focusedKnob)
        {
            return NodeAtPosition(curEditorState, canvasPos, out focusedKnob);
        }

         public static Node NodeAtPosition(NodeEditorState editorState, Vector2 canvasPos, out NodeKnob focusedKnob)
        {
            focusedKnob = null;
            if (NodeEditorInputSystem.shouldIgnoreInput(editorState))
                return null;
            NodeCanvas canvas = editorState.canvas;
            for (int nodeCnt = canvas.nodes.Count - 1; nodeCnt >= 0; nodeCnt--)
            {  Node node = canvas.nodes[nodeCnt];
                if (node.rect.Contains(canvasPos))
                    return node;
                for (int knobCnt = 0; knobCnt < node.nodeKnobs.Count; knobCnt++)
                {  if (node.nodeKnobs[knobCnt].GetCanvasSpaceKnob().Contains(canvasPos))
                    {
                        focusedKnob = node.nodeKnobs[knobCnt];
                        return node;
                    }
                }
            }
            return null;
        }

        public static Vector2 ScreenToCanvasSpace(Vector2 screenPos)
        {
            return ScreenToCanvasSpace(curEditorState, screenPos);
        }
        public static Vector2 ScreenToCanvasSpace(NodeEditorState editorState, Vector2 screenPos)
        {
            return (screenPos - editorState.canvasRect.position - editorState.zoomPos) * editorState.zoom - editorState.panOffset;
        }

        #endregion

        #region Calculation

        public static List<Node> workList;
        private static int calculationCount;
     
        public static void RecalculateAll(NodeCanvas nodeCanvas)
        {
            Node.InitValueCount();
            int[] FixTable = new int[nodeCanvas.nodes.Count];
            for (int w = 0; w < nodeCanvas.nodes.Count; w++)
            {
                FixTable[w] = Math.Abs(nodeCanvas.nodes[w].GetInstanceID());
            }
            Array.Sort(FixTable);
  
            for (int w = 0; w < nodeCanvas.nodes.Count; w++)
            {
                for (int x = 0; x < nodeCanvas.nodes.Count; x++)
                {
                    if (FixTable[w] == Math.Abs(nodeCanvas.nodes[x].GetInstanceID()))
                    {
                        nodeCanvas.nodes[x].FixCalculate();
                        break;
                    }
                }
            }
            /*
            for (int w = 0; w < nodeCanvas.nodes.Count; w++)
             {
            nodeCanvas.nodes[FixTable[w]].FixCalculate();
             }
            */
            //     Debug.Log(" Node[" + w + "]=" + Math.Abs(nodeCanvas.nodes[w].GetInstanceID()).ToString());
            Node.ErrorTag = false;
     //     Node.InitValueCount();
            workList = new List<Node>();
            foreach (Node node in nodeCanvas.nodes)
            {
                if (node.isInput())
                {  node.ClearCalculation();
                     workList.Add(node);
                }
            }
          
            StartCalculation();
        }

        public static void RecalculateFrom(Node node)
        {
            node.ClearCalculation();
            workList = new List<Node> { node };
            StartCalculation();
        }

         public static void StartCalculation()
        {
            checkInit(false);
            
            if (InitiationError)
                return;
        
            if (workList == null || workList.Count == 0)
                return;

             calculationCount = 0;

            bool limitReached = false;
            for (int roundCnt = 0; !limitReached; roundCnt++)
            {   limitReached = true;
                for (int workCnt = 0; workCnt < workList.Count; workCnt++)
                {
                    if (ContinueCalculation(workList[workCnt]))
                        limitReached = false;
                }
            }
           
        }

          private static bool ContinueCalculation(Node node)
        {
            if (node.calculated)
                return false;
            if ((node.descendantsCalculated() || node.isInLoop()) && node.Calculate())
            {   node.calculated = true;
                calculationCount++;
                workList.Remove(node);
                if (node.ContinueCalculation && calculationCount < 1000)
                {
                    for (int outCnt = 0; outCnt < node.Outputs.Count; outCnt++)
                    {
                        NodeOutput output = node.Outputs[outCnt];
                        if (!output.calculationBlockade)
                        {
                            for (int conCnt = 0; conCnt < output.connections.Count; conCnt++)
                                ContinueCalculation(output.connections[conCnt].body);
                        }
                    }

                }


                else if (calculationCount >= 1000)
                    Debug.LogError("Stopped calculation, to many node ! max 1000!");
           
                return true;
            }
            else if (!workList.Contains(node))
            { 
                workList.Add(node);
            }
            return false;
        }

        #endregion
    }
}
