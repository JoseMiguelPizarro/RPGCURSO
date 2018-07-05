using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Build Shader")]
    public class BuildShader : Node
    {
        public const string ID = "BuildShader";
        [HideInInspector]
        public override string GetID { get { return ID; } }

        private static bool ForceNoHdr;
        private static bool ForceFog;
        private static bool LightningSupport;
        private static bool ChangeVariableName;
        private static bool SupportPixelSnap;
        private bool B_NoHdr;

        private static string ShaderBlend = "Blend SrcAlpha OneMinusSrcAlpha";
        private static int ShaderUse = 0;

        [Multiline(50)]
        public static string result;

        private Material MemoShaderMaterial;
        public static string MemoResult;

        [HideInInspector]
        public bool BuildErrorFlag = false;
        [HideInInspector]
        public bool BuildLightningSupport = false;

        [HideInInspector]
        private Shader Buildshader;
        private string MBuildshader;

        [HideInInspector]
        public static string[] VariableName = new string[256];
        [HideInInspector]
        public static string[] OrigineVariableName = new string[256];
        [HideInInspector]
        public static string[] NewVariableName = new string[256];
        [HideInInspector]
        public static int NumberVariable = 0;

        public override Node Create(Vector2 pos)
        {
            BuildShader node = ScriptableObject.CreateInstance<BuildShader>();
            NumberVariable = 0;
            node.name = "Build Shader";
            node.rect = new Rect(pos.x, pos.y, 260, 280);
            node.CreateInput("RGBA", "SuperFloat4");
            return node;
        }
        public void UpdateSave()
        {
            if (NodeEditor.curEditorState.LivePreviewShaderPath != "")
            {
                string path = NodeEditor.curEditorState.LivePreviewShaderPath;
                if (!string.IsNullOrEmpty(path))
                {
                    if (BuildErrorFlag)
                    {
                    }
                    else
                    {
                        if (System.IO.File.Exists(path))
                        {
                            File.WriteAllText(path, MemoResult);
                            NodeEditor.UpdatePreview = 1;
                            SceneView.RepaintAll();
                            AssetDatabase.Refresh();
                        }
                    }
                }
            }
        }

        protected internal override void NodeGUI()
        {
            NodeEditor.BuildShaderPosX = rect.x;
            NodeEditor.BuildShaderPosY = rect.y;
            if (NodeEditor.ShaderToNull)
            {
                Buildshader = null;
                NodeEditor.ShaderToNull = false;
            }
            NodeEditor.NoBuildShader = true;
            // Save
            if (NodeEditor.ForceWriteFlag)
            {
                UpdateSave();
                NodeEditor.ForceWriteFlag = false;
            }

            if (Inputs[0].GetNodeAcrossConnection() == null) NodeEditor.RGBAonBuildShader = false; else NodeEditor.RGBAonBuildShader = true;

            if (Inputs[0].GetNodeAcrossConnection() == null) Inputs[0].knobTexture = ResourceManager.GetTintedTexture("Textures/In_Knob.png", new Color(0.50f, 0.50f, 0.50f, 0.75f));
            else Inputs[0].knobTexture = ResourceManager.GetTintedTexture("Textures/In_Knob.png", Color.white);

            Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA colors"));

            GUILayout.Space(6);
            GUIStyle g = new GUIStyle();
            int ms = g.fontSize;
            g.fontSize = 11;
            g.alignment = TextAnchor.LowerLeft;
            g.normal.textColor = Color.white;
            GUILayout.Label("Used Shader*", g);
            g.fontSize = ms;

            GUILayout.Space(32);

            if (Buildshader == null)
            {
                Texture2D preview = ResourceManager.LoadTexture("Textures/arrow.png");
                GUI.DrawTexture(new Rect(-30, 35, 350, 50), preview);
                NumberVariable = 0;

            }

            Buildshader = (Shader)EditorGUI.ObjectField(new Rect(5, 50, 250, 18), Buildshader, typeof(Shader), true);
            if (MBuildshader != AssetDatabase.GetAssetPath(Buildshader))
            {
                if (Buildshader != null)
                {
                    NodeEditor.ForceWriteFlag = true;
                    NodeEditor.RecalculateFlag = true;
                    NodeEditor.UpdatePreview = 1;
                    SceneView.RepaintAll();
                    AssetDatabase.Refresh();
                }
            }
            MBuildshader = AssetDatabase.GetAssetPath(Buildshader);

            if (NodeEditor.FlagIsLoadedMaterial)
            {
                Buildshader = NodeEditor.curEditorState.ShaderInMemory;
                NodeEditor.FlagIsLoadedMaterial = false;
            }
            else
            {
                NodeEditor.curEditorState.ShaderInMemory = Buildshader;
            }


            g = new GUIStyle();
            g.fontSize = 12;
            g.normal.textColor = Color.white;
            GUILayout.Label("Shader Name*", g);
            int msize = GUI.skin.textField.fontSize;
            GUI.skin.textField.fontSize = 20;
            GUILayout.Label(ShaderNameX, g);
            ShaderNameX = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(Buildshader));
            GUI.skin.textField.fontSize = msize;
            if (Buildshader != null) NodeEditor.curEditorState.LivePreviewShaderPath = AssetDatabase.GetAssetPath(Buildshader);
            if (Buildshader == null) NodeEditor.curEditorState.LivePreviewShaderPath = "";
            NodeEditor.curEditorState.ShaderName = ShaderNameX;
            if (NodeEditor.FlagIsMaterialChanged)
            {
                NodeEditor.FlagIsMaterialChanged = false;
            }

            if (NodeEditor.curEditorState.LivePreviewShaderPath == "") NodeEditor.FlagIsSaved = false; else NodeEditor.FlagIsSaved = true;


            if (NodeEditor._Shadero_Material == null)
            {
                if (MemoShaderMaterial) NodeEditor._Shadero_Material = MemoShaderMaterial;
            }

            if (NodeEditor._Shadero_Material)
            {
                MemoShaderMaterial = NodeEditor._Shadero_Material;
            }

            if (NodeEditor.curEditorState.LivePreviewShaderPath != "") NodeEditor.AutoUpdateFlag = true; else NodeEditor.AutoUpdateFlag = false;

            if (NodeEditor.curEditorState.LivePreviewShaderPath != "")
            {
                if (GUILayout.Button(new GUIContent("Update Shader", "Saves the Shader to a file in the Assets Folder")))
                {
                    if (NodeEditor.curEditorState.LivePreviewShaderPath != "")
                    {
                        string path = NodeEditor.curEditorState.LivePreviewShaderPath;
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (BuildErrorFlag)
                            {
                                EditorUtility.DisplayDialog("Shadero - Build Error", "You must connect a RGBA source", "OK");
                                BuildErrorFlag = false;
                            }
                            else
                            {
                                NodeEditor.RecalculateFlag = true;
                                File.WriteAllText(path, result);
                                NodeEditor.UpdatePreview = 1;
                                SceneView.RepaintAll();
                                AssetDatabase.Refresh();
                            }
                        }
                    }


                }

                NodeEditor.AutoUpdate = GUILayout.Toggle(NodeEditor.AutoUpdate, "Auto Update");

            }
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();
            ForceNoHdr = GUILayout.Toggle(ForceNoHdr, "Force No Hdr");
            ForceFog = GUILayout.Toggle(ForceFog, "Force Fog");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            SupportPixelSnap = GUILayout.Toggle(SupportPixelSnap, "Pixel Snap Support");
            GUILayout.EndHorizontal();
            GUILayout.Label("Blend Mode");
            string[] test = new string[6];
            test[0] = "Normal";
            test[1] = "Add";
            test[2] = "Soft\nAdd";
            test[3] = "Mul";
            test[4] = "2x Mul";
            test[5] = "Pre-\nMul";
            ShaderUse = GUILayout.Toolbar(ShaderUse, test, GUILayout.Height(30));
            if (ShaderUse == 0) { ShaderBlend = "Blend SrcAlpha OneMinusSrcAlpha"; }
            if (ShaderUse == 1) { ShaderBlend = "Blend SrcAlpha One"; }
            if (ShaderUse == 2) { ShaderBlend = "Blend OneMinusDstColor One"; }
            if (ShaderUse == 3) { ShaderBlend = "Blend DstColor Zero"; }
            if (ShaderUse == 4) { ShaderBlend = "Blend DstColor SrcColor"; }
            if (ShaderUse == 5) { ShaderBlend = "Blend One OneMinusSrcAlpha"; }


            if (Buildshader != null)
            {
                GUILayout.Space(18);
                ChangeVariableName = GUILayout.Toggle(ChangeVariableName, "Force Change Variable Name");
                GUILayout.Label("(Experimental, use it only if you \n have finished the shader)");


                if (ChangeVariableName)
                {
                    rect.height = 400 + (NumberVariable * 22);
                    rect.width = 460;
                    for (int c = 0; c < NumberVariable; c++)
                    {
                        GUILayout.BeginHorizontal();
                        if (OrigineVariableName[c] != VariableName[c])
                        {
                            NewVariableName[c] = VariableName[c];
                            OrigineVariableName[c] = VariableName[c];
                        }
                        if (NewVariableName[c] == null) NewVariableName[c] = VariableName[c];
                        NewVariableName[c] = GUILayout.TextField(NewVariableName[c]);
                        GUILayout.Label(VariableName[c]);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    rect.height = 400;
                    rect.width = 300;
                }
            }
        }



        public static string CleanUpScript(string script)
        {
            if (script == "") return "";

            string[] newscript = script.Split("\n"[0]);
            string rscript = "";
            for (int c = 0; c < newscript.Length - 1; c++)
            {
                string firstWord = newscript[c].Split(new char[] { ' ' })[0];
                if (firstWord == "float" || firstWord == "float2" || firstWord == "float4")
                {
                    for (int d = c; d < newscript.Length - 1; d++)
                    {
                        if (newscript[c] == newscript[d + 1]) newscript[d + 1] = "";
                    }
                }
            }
            rscript = "";
            for (int c = 0; c < newscript.Length; c++)
            {
                if (newscript[c] != "") rscript += newscript[c] + "\n";
            }
            return rscript;
        }
        public static string CleanUpScriptVariable(string script)
        {
            if (script == "") return "";

            string[] newscript = script.Split("\n"[0]);
            string rscript = "";
            for (int c = 0; c < newscript.Length; c++)
            {
                for (int d = c; d < newscript.Length - 1; d++)
                {
                    if (newscript[c] == newscript[d + 1]) newscript[d + 1] = "";
                }
            }
            rscript = "";
            for (int c = 0; c < newscript.Length; c++)
            {
                if (newscript[c] != "") rscript += newscript[c] + "\n";
            }
            return rscript;
        }


        public static void CheckVariables(string Parameters)
        {
            string[] newscript = Parameters.Split("\n"[0]);
            int number = 0;
            for (int c = 0; c < newscript.Length; c++)
            {
                if (newscript[c].Contains("float4"))
                {
                    string[] word = newscript[c].Split(" "[0]);
                    string fword = word[1].Remove(word[1].Length - 1);
                    VariableName[number] = fword;
                    if (NewVariableName == null) OrigineVariableName[c] = fword;
                    number++;

                }
                else
                if (newscript[c].Contains("float"))
                {
                    string[] word = newscript[c].Split(" "[0]);
                    string fword = word[1].Remove(word[1].Length - 1);
                    VariableName[number] = fword;
                    if (NewVariableName == null) OrigineVariableName[c] = fword;

                    number++;
                }
                else
                if (newscript[c].Contains("sampler2D"))
                {
                    string[] word = newscript[c].Split(" "[0]);
                    string fword = word[1].Remove(word[1].Length - 1);
                    VariableName[number] = fword;
                    if (NewVariableName == null) OrigineVariableName[c] = fword;
                    number++;
                }


            }
            NumberVariable = number;
        }

        public static string WriteFunction(string AddingTag)
        {
            if (GrayScale.tag) AddingTag += GrayScale.code;
            if (Blur.tag) AddingTag += Blur.code;
            if (AnimatedOffsetUV.tag) AddingTag += AnimatedOffsetUV.code;
            if (AnimatedPingPongOffsetUV.tag) AddingTag += AnimatedPingPongOffsetUV.code;
            if (DistortionUV.tag) AddingTag += DistortionUV.code;
            if (OffsetUV.tag) AddingTag += OffsetUV.code;
            if (RotationUV.tag) AddingTag += RotationUV.code;
            if (OutlineFX.tag) AddingTag += OutlineFX.code;
            if (UniColor.tag) AddingTag += UniColor.code;
            if (BlurHQ.tag) AddingTag += BlurHQ.code;
            if (CompressionFX.tag) AddingTag += CompressionFX.code;
            if (OutlineEmpty.tag) AddingTag += OutlineEmpty.code;
            if (Hologram.tag) AddingTag += Hologram.code;
            if (ClippingLeft.tag) AddingTag += ClippingLeft.code;
            if (ClippingRight.tag) AddingTag += ClippingRight.code;
            if (ClippingUp.tag) AddingTag += ClippingUp.code;
            if (ClippingDown.tag) AddingTag += ClippingDown.code;
            if (ColorRGBA.tag) AddingTag += ColorRGBA.code;
            if (TintRGBA.tag) AddingTag += TintRGBA.code;
            if (InverseColor.tag) AddingTag += InverseColor.code;
            if (Threshold.tag) AddingTag += Threshold.code;
            if (Brightness.tag) AddingTag += Brightness.code;
            if (Darkness.tag) AddingTag += Darkness.code;
            if (TwistUV.tag) AddingTag += TwistUV.code;
            if (AnimatedTwistUV.tag) AddingTag += AnimatedTwistUV.code;
            if (ZoomUV.tag) AddingTag += ZoomUV.code;
            if (PixelXYUV.tag) AddingTag += PixelXYUV.code;
            if (Desintegration.tag) AddingTag += Desintegration.code;
            if (Destroyer.tag) AddingTag += Destroyer.code;
            if (BurnFX.tag) AddingTag += BurnFX.code;
            if (PlasmaFX.tag) AddingTag += PlasmaFX.code;
            if (ColorHSV.tag) AddingTag += ColorHSV.code;
            if (GhostFX.tag) AddingTag += GhostFX.code;
            if (CircleFade.tag) AddingTag += CircleFade.code;
            if (FourGradients.tag) AddingTag += FourGradients.code;
            if (ThresholdSmooth.tag) AddingTag += ThresholdSmooth.code;
            if (TiltUpUV.tag) AddingTag += TiltUpUV.code;
            if (TiltDownUV.tag) AddingTag += TiltDownUV.code;
            if (TiltLeftUV.tag) AddingTag += TiltLeftUV.code;
            if (TiltRightUV.tag) AddingTag += TiltRightUV.code;
            if (Generate_Circle.tag) AddingTag += Generate_Circle.code;
            if (Generate_Shape.tag) AddingTag += Generate_Shape.code;
            if (DisplacementUV.tag) AddingTag += DisplacementUV.code;
            if (OperationBlend.tag) AddingTag += OperationBlend.code;
            if (Generate_Pyramid.tag) AddingTag += Generate_Pyramid.code;
            if (Generate_Checker.tag) AddingTag += Generate_Checker.code;
            if (Generate_Fire.tag) AddingTag += Generate_Fire.code;
            if (PremadeGradients.tag) AddingTag += PremadeGradients.code;
            if (ColorGradients.tag) AddingTag += ColorGradients.code;
            if (DisplacementPlusUV.tag) AddingTag += DisplacementPlusUV.code;
            if (Generate_Voronoi.tag) AddingTag += Generate_Voronoi.code;
            if (FlipUV_H.tag) AddingTag += FlipUV_H.code;
            if (FlipUV_V.tag) AddingTag += FlipUV_V.code;
            if (Generate_LightningFX.tag) AddingTag += Generate_LightningFX.code;
            if (AnimatedMouvementUV.tag) AddingTag += AnimatedMouvementUV.code;
            if (PatternMovement.tag) AddingTag += PatternMovement.code;
            if (OperationBlendMask.tag) AddingTag += OperationBlendMask.code;
            if (PatternMovementMask.tag) AddingTag += PatternMovementMask.code;
            if (KaleidoscopeUV.tag) AddingTag += KaleidoscopeUV.code;
            if (TurnAlphaToBlack.tag) AddingTag += TurnAlphaToBlack.code;
            if (RenderedTexture.tag) AddingTag += RenderedTexture.code;
            if (TurnBlackToAlpha.tag) AddingTag += TurnBlackToAlpha.code;
            if (ResizeUV.tag) AddingTag += ResizeUV.code;
            if (SpriteSheetFrameUV.tag) AddingTag += SpriteSheetFrameUV.code;
            if (SpriteSheetAnimationUV.tag) AddingTag += SpriteSheetAnimationUV.code;
            if (Generate_Xray.tag) AddingTag += Generate_Xray.code;
            if (Generate_Donuts.tag) AddingTag += Generate_Donuts.code;
            if (Generate_Line.tag) AddingTag += Generate_Line.code;
            if (Generate_Noise.tag) AddingTag += Generate_Noise.code;
            if (PixelUV.tag) AddingTag += PixelUV.code;
            if (FishEyeUV.tag) AddingTag += FishEyeUV.code;
            if (PinchUV.tag) AddingTag += PinchUV.code;
            if (TurnMetal.tag) AddingTag += TurnMetal.code;
            if (TurnGold.tag) AddingTag += TurnGold.code;
            if (TurnTransparent.tag) AddingTag += TurnTransparent.code;
            if (HdrControlValue.tag) AddingTag += HdrControlValue.code;
            if (HdrCreate.tag) AddingTag += HdrCreate.code;
            if (PositionUV.tag) AddingTag += PositionUV.code;
            if (FadeToAlpha.tag) AddingTag += FadeToAlpha.code;
            if (SuperGrayScale.tag) AddingTag += SuperGrayScale.code;
            if (ColorFilters.tag) AddingTag += ColorFilters.code;
            if (DisplacementPack.tag) AddingTag += DisplacementPack.code;
            if (PatternMovementPack.tag) AddingTag += PatternMovementPack.code;
            if (AnimatedZoomUV.tag) AddingTag += AnimatedZoomUV.code;
            if (InnerGlow.tag) AddingTag += InnerGlow.code;
            if (Generate_Spiral.tag) AddingTag += Generate_Spiral.code;
            if (FixUV.tag) AddingTag += FixUV.code;
            if (SimpleDisplacementUV.tag) AddingTag += SimpleDisplacementUV.code;
            if (SpriteSheetUVAnimPack.tag) AddingTag += SpriteSheetUVAnimPack.code;
            if (MorphingPack.tag) AddingTag += MorphingPack.code;
            if (MorphingPerfectPack.tag) AddingTag += MorphingPerfectPack.code;
            if (AnimatedRotationUV.tag) AddingTag += AnimatedRotationUV.code;
            if (AlphaAsAura.tag) AddingTag += AlphaAsAura.code;
            if (AlphaIntensity.tag) AddingTag += AlphaIntensity.code;
            if (Generate_Starfield.tag) AddingTag += Generate_Starfield.code;
            if (TurnGB.tag) AddingTag += TurnGB.code;
            if (TurnC64.tag) AddingTag += TurnC64.code;
            if (TurnEGA.tag) AddingTag += TurnEGA.code;
            if (TurnCGA.tag) AddingTag += TurnCGA.code;
            if (TurnCGA2.tag) AddingTag += TurnCGA2.code;
            if (Make3DFX.tag) AddingTag += Make3DFX.code;
            if (BlurHQPlus.tag) AddingTag += BlurHQPlus.code;
            if (Emboss.tag) AddingTag += Emboss.code;
            if (EmbossFull.tag) AddingTag += EmbossFull.code;
            if (Sharpness.tag) AddingTag += Sharpness.code;
            if (MotionBlurFast.tag) AddingTag += MotionBlurFast.code;
            if (ShadowLight.tag) AddingTag += ShadowLight.code;
            if (ShinyFX.tag) AddingTag += ShinyFX.code;
            if (PlasmaLightFX.tag) AddingTag += PlasmaLightFX.code;
            if (HolographicParalax.tag) AddingTag += HolographicParalax.code;
            if (HumanBreathUV.tag) AddingTag += HumanBreathUV.code;
            if (HumanBreathExUV.tag) AddingTag += HumanBreathExUV.code;
            if (GenerateSOFTexture.tag) AddingTag += GenerateSOFTexture.code;
            if (AnimatedInfiniteZoomUV.tag) AddingTag += AnimatedInfiniteZoomUV.code;
            if (ShinyOnlyFX.tag) AddingTag += ShinyOnlyFX.code;
            
            if (CircleHole.tag) AddingTag += CircleHole.code;
            if (DoodleUV.tag) AddingTag += DoodleUV.code;
            if (LiquidUV.tag) AddingTag += LiquidUV.code;
            return AddingTag;
        }

        public static string BuildVertexShader(string shadername, string preview, string res, string ParametersLines, string ParametersDeclarationLines)
        {


            CheckVariables(CleanUpScriptVariable(ParametersDeclarationLines));

            string First = "";
            First += "//////////////////////////////////////////////////////////////\n";
            First += "/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //\n";
            First += "/// Shader generate with Shadero 1.9.0                      //\n";
            First += "/// http://u3d.as/V7t #AssetStore                           //\n";
            First += "/// http://www.shadero.com #Docs                            //\n";
            First += "//////////////////////////////////////////////////////////////\n";
            First += "\n";
            First += "Shader \"" + shadername + "\"\n";
            First += "{\n";
            First += "Properties\n";
            First += "{\n";
            First += "[PerRendererData] _MainTex(\"Sprite Texture\", 2D) = \"white\" {}\n";
            if (SupportPixelSnap) First += "[MaterialToggle] PixelSnap (\"Pixel snap\", Float) = 0\n";
            string Second = "";
            Second += "_SpriteFade(\"SpriteFade\", Range(0, 1)) = 1.0\n";
            Second += "\n";
            Second += "// required for UI.Mask\n";
            Second += "[HideInInspector]_StencilComp(\"Stencil Comparison\", Float) = 8\n";
            Second += "[HideInInspector]_Stencil(\"Stencil ID\", Float) = 0\n";
            Second += "[HideInInspector]_StencilOp(\"Stencil Operation\", Float) = 0\n";
            Second += "[HideInInspector]_StencilWriteMask(\"Stencil Write Mask\", Float) = 255\n";
            Second += "[HideInInspector]_StencilReadMask(\"Stencil Read Mask\", Float) = 255\n";
            Second += "[HideInInspector]_ColorMask(\"Color Mask\", Float) = 15\n";
            Second += "\n";
            Second += "}\n";
            Second += "\n";
            Second += "SubShader\n";
            Second += "{\n";
            Second += "\n";
            string WorldParalaxTagStr = "";
            if (WorldParalaxTag) WorldParalaxTagStr = "\"DisableBatching\" = \"True\"";
            Second += "Tags {\"Queue\" = \"Transparent\" \"IgnoreProjector\" = \"true\" \"RenderType\" = \"Transparent\" \"PreviewType\"=\"Plane\" \"CanUseSpriteAtlas\"=\"True\" "+ WorldParalaxTagStr + "}\n";
            Second += "ZWrite Off " + ShaderBlend + " Cull Off\n";
            Second += "\n";

            if (GrabPassTag)
            {
                Second += "GrabPass { \"_GrabTexture\"  } \n";
                Second += "\n";
            }

            Second += "// required for UI.Mask\n";
            Second += "Stencil\n";
            Second += "{\n";
            Second += "Ref [_Stencil]\n";
            Second += "Comp [_StencilComp]\n";
            Second += "Pass [_StencilOp]\n";
            Second += "ReadMask [_StencilReadMask]\n";
            Second += "WriteMask [_StencilWriteMask]\n";
            Second += "}\n";
            Second += "\n";
            Second += "Pass\n";
            Second += "{\n";
            Second += "\n";
            Second += "CGPROGRAM\n";
            Second += "#pragma vertex vert\n";
            Second += "#pragma fragment frag\n";
            Second += "#pragma fragmentoption ARB_precision_hint_fastest\n";
            if (ForceFog) Second += " #pragma multi_compile_fog\n";
            if (SupportPixelSnap) Second += "#pragma multi_compile _ PIXELSNAP_ON\n";
            Second += "#include \"UnityCG.cginc\"\n";
            Second += "\n";
            Second += "struct appdata_t";
            Second += "{\n";
            Second += "float4 vertex   : POSITION;\n";
            Second += "float4 color    : COLOR;\n";
            Second += "float2 texcoord : TEXCOORD0;\n";
            Second += "};\n";
            Second += "\n";
            Second += "struct v2f\n";
            Second += "{\n";
            Second += "float2 texcoord  : TEXCOORD0;\n";
            if (ForceFog) Second += "UNITY_FOG_COORDS(1)\n";
            Second += "float4 vertex   : SV_POSITION;\n";
     
            if (GrabPassTag)
            {
                Second += "float2 screenuv : TEXCOORD1;\n";
            }
            if (WorldPosTag)
            {
                Second += "float3 worldPos : TEXCOORD2;\n";
            }
            

            Second += "float4 color    : COLOR;\n";
            Second += "};\n";
            Second += "\n";

            if (GrabPassTag) Second += "sampler2D _GrabTexture;\n";

            Second += "sampler2D _MainTex;\n";
            Second += "float _SpriteFade;\n";

            string Thirth = "";
            Thirth += "\n";
            Thirth += "v2f vert(appdata_t IN)\n";
            Thirth += "{\n";
            Thirth += "v2f OUT;\n";
            Thirth += "OUT.vertex = UnityObjectToClipPos(IN.vertex);\n";

            if (WorldPosTag)
            {
                Thirth += "OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);\n";
            }
            if (GrabPassTag)
            {
                Thirth += "float4 screenpos = ComputeGrabScreenPos(OUT.vertex);\n";
                Thirth += "OUT.screenuv = screenpos.xy / screenpos.w;\n";
            }

            Thirth += "OUT.texcoord = IN.texcoord;\n";
            Thirth += "OUT.color = IN.color;\n";
            if (ForceFog) Thirth += "UNITY_TRANSFER_FOG(OUT, OUT.vertex);\n";
            Thirth += "return OUT;\n";
            Thirth += "}\n";
            Thirth += "\n";
            Thirth += "\n";

            string Start = "";
            Start += "float4 frag (v2f i) : COLOR\n";
            Start += "{\n";

            string Fourth = "";
            Fourth += "}\n";
            Fourth += "\n";
            Fourth += "ENDCG\n";
            Fourth += "}\n";
            Fourth += "}\n";
            Fourth += "Fallback \"Sprites/Default\"\n";
            Fourth += "}\n";


            string AddingTag = "";
            AddingTag = WriteFunction(AddingTag);

            // RGBA
            result = "";
            result += First;
            result += CleanUpScriptVariable(ParametersLines);
            result += Second;
            result += CleanUpScriptVariable(ParametersDeclarationLines);
            result += Thirth;
            result += AddingTag;
            result += Start;
            result += CleanUpScript(preview) + "float4 FinalResult = " + res + ";\n";
            result += "FinalResult.rgb *= i.color.rgb;\n";
            result += "FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;\n";
            if (ForceFog) result += "UNITY_APPLY_FOG(i.fogCoord, FinalResult);\n";
            if (ForceNoHdr) result += "FinalResult = saturate(FinalResult);\n";
            result += "return FinalResult;\n";
            result += Fourth;


            // Force Change Variable
            if (ChangeVariableName)
            {
                for (int c = 0; c < NumberVariable; c++)
                {
                    result = result.Replace(VariableName[c], NewVariableName[c]);
                }
            }

            return result;
        }
        public static string BuildLightShader(string shadername, string preview, string res, string ParametersLines, string ParametersDeclarationLines)
        {
            string First = "";
            First += "//////////////////////////////////////////////////////////////\n";
            First += "/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //\n";
            First += "/// Shader generate with Shadero 1.9.0                      //\n";
            First += "/// http://u3d.as/V7t #AssetStore                           //\n";
            First += "/// http://www.shadero.com #Docs                            //\n";
            First += "//////////////////////////////////////////////////////////////\n";
            First += "\n";
            First += "Shader \"" + shadername + "\"\n";
            First += "{\n";
            First += "Properties\n";
            First += "{\n";
            First += "[PerRendererData] _MainTex(\"Sprite Texture\", 2D) = \"white\" {}\n";
            First += "_Color (\"Tint\", Color) = (1,1,1,1)\n";
            if (SupportPixelSnap) First += "[MaterialToggle] PixelSnap (\"Pixel snap\", Float) = 0\n";
            First += "[HideInInspector] _RendererColor(\"RendererColor\", Color) = (1,1,1,1)\n";
            First += "[HideInInspector] _Flip(\"Flip\", Vector) = (1,1,1,1)\n";
            First += "[PerRendererData] _AlphaTex(\"External Alpha\", 2D) = \"white\" {}\n";
            First += "[PerRendererData] _EnableExternalAlpha(\"Enable External Alpha\", Float) = 0\n";

            string Second = "";
            Second += "_SpriteFade(\"SpriteFade\", Range(0, 1)) = 1.0\n";
            Second += "\n";
            Second += "}\n";
            Second += "\n";
            Second += "SubShader\n";
            Second += "{\n";

            Second += "Tags\n";
            Second += "{\n";
            Second += "\"Queue\" = \"Transparent\"\n";
            Second += "\"IgnoreProjector\" = \"True\"\n";
            Second += "\"RenderType\" = \"Transparent\"\n";
            Second += "\"PreviewType\" = \"Plane\"\n";
            Second += "\"CanUseSpriteAtlas\" = \"True\"\n";
            Second += "\n";
            Second += "}\n";
            Second += "\n";
            Second += "Cull Off\n";
            Second += "Lighting Off\n";
            Second += "ZWrite Off\n";
            Second += ShaderBlend + "\n";
            Second += "\n";

            if (GrabPassTag)
            {
                Second += "GrabPass { \"_GrabTexture\"  } \n";
                Second += "\n";
            }

            Second += "\n";
            Second += "CGPROGRAM\n";
            Second += "\n";
            string Ffog = "";
            if (ForceFog) Ffog = "nofog";
            Second += "#pragma surface surf Lambert vertex:vert "+ Ffog+" nolightmap nodynlightmap keepalpha noinstancing\n";
            Second += "#pragma multi_compile _ PIXELSNAP_ON\n";
            Second += "#pragma multi_compile _ ETC1_EXTERNAL_ALPHA\n";
             Second += "#include \"UnitySprites.cginc\"\n";


            Second += "struct Input\n";
            Second += "{\n";
            Second += "float2 texcoord;\n";
            Second += "float4 color;\n";
       
            if (GrabPassTag)
            {
                Second += "float2 screenuv;\n";
            }
            if (WorldPosTag)
            {
                Second += "float3 worldPos;\n";
            }

            Second += "};\n";
            Second += "\n";



            if (GrabPassTag) Second += "sampler2D _GrabTexture;\n";

            Second += "float _SpriteFade;\n";

            string Thirth = "";
            Thirth += "\n";
            Thirth += "void vert(inout appdata_full v, out Input o)\n";
            Thirth += "{\n";
            Thirth += "v.vertex.xy *= _Flip.xy;\n";

            Thirth += "#if defined(PIXELSNAP_ON)\n";

            Thirth += "v.vertex = UnityPixelSnap (v.vertex);\n";
            Thirth += "#endif\n";

            Thirth += "UNITY_INITIALIZE_OUTPUT(Input, o);\n";

            if (GrabPassTag)
            {
                Thirth += "float4 screenpos = ComputeGrabScreenPos(o.vertex);\n";
                Thirth += "o.screenuv = screenpos.xy / screenpos.w;\n";
            }
            if (WorldPosTag)
            {
                Thirth += "o.worldPos = mul(unity_ObjectToWorld, v.vertex);\n";
                
            }
            
            Thirth += "o.color = v.color * _Color * _RendererColor;\n";
             Thirth += "}\n";
            Thirth += "\n";
            Thirth += "\n";

            string Start = "";
            Start += "void surf(Input i, inout SurfaceOutput o)\n";

            Start += "{\n";

            string Fourth = "";
            Fourth += "}\n";
            Fourth += "\n";
            Fourth += "ENDCG\n";
            Fourth += "}\n";
            Fourth += "Fallback \"Sprites /Default\"\n";
            Fourth += "}\n";

            // Add function

            string AddingTag = "";
            AddingTag = WriteFunction(AddingTag);

            // RGBA
            result = "";
            result += First;
            result += CleanUpScriptVariable(ParametersLines);
            result += Second;
            result += CleanUpScriptVariable(ParametersDeclarationLines);
            result += Thirth;
            result += AddingTag;
            result += Start;
            result += CleanUpScript(preview) + "float4 FinalResult = " + res + ";\n";
             if (ForceNoHdr) { result += "o.Albedo = saturate(FinalResult.rgb* i.color.rgb);\n"; }
            else
            { result += "o.Albedo = FinalResult.rgb* i.color.rgb;\n"; }
            result += "o.Alpha = FinalResult.a * _SpriteFade * i.color.a;\n";
            if (NormalMapTag) result += NormalMapOutput;
            result += "clip(o.Alpha - 0.05);\n";
            result += Fourth;
            return result;
        }
        public override bool Calculate()
        {
            SuperFloat4 sfa = Inputs[0].GetValue<SuperFloat4>();
            if (sfa.Result != null)
            {
                if (LightingSupportTag)
                {
                    result = BuildLightShader("Shadero Customs/" + Node.ShaderNameX, sfa.StringPreviewNew, sfa.Result, sfa.ParametersLines, sfa.ParametersDeclarationLines);
                    result = result.Replace("texcoord", "uv_MainTex");
                }
                else
                {
                    result = BuildVertexShader("Shadero Customs/" + Node.ShaderNameX, sfa.StringPreviewNew, sfa.Result, sfa.ParametersLines, sfa.ParametersDeclarationLines);

                }
                MemoResult = result;
                BuildErrorFlag = false;
            }
            else
            { BuildErrorFlag = true; }

            return true;
        }
    }
}