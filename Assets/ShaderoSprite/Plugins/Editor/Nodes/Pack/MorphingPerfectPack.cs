using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Pack/Morphing Perfect Pack")]
    public class MorphingPerfectPack : Node
    {
        [HideInInspector]
        public const string ID = "MorphingPerfectPack";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 1;
        [HideInInspector]
        public float Variable2 = 0;
        [HideInInspector]
        public float Variable3 = 1;
        [HideInInspector]
        public float Variable4 = 1;
        [HideInInspector]
        public float Variable5 = 1;
        public static int count = 1;
        public static bool tag = false;
        public static string code;
        [HideInInspector]
        public Texture2D tex;
        [HideInInspector]
        public Texture2D tex2;
        [HideInInspector]
        public Texture2D tex3;
        [HideInInspector]
        public Texture2D tex4;

        public static int total = 1;
        [HideInInspector]
        public bool AddParameters = true;
        [HideInInspector]
        public bool AddMotion = true;
        [HideInInspector]
        public bool AddMotionAlternate = true;

        public static void Init()
        {
            tag = false;
            count = 1;
        }

        public void Function()
        {
            code = "";
            code += "float4 MorphingPerfectPack(float2 uv, sampler2D source, sampler2D source2, sampler2D source3, sampler2D source4, float blend, float StrangerValue)\n";
            code += "{\n";
            code += "float smooth = 0.10f;\n";
            code += "float r = 1 - smoothstep(0.0, smooth, uv.x);\n";
            code += "r += smoothstep(1. - smooth, 1., uv.x);\n";
            code += "r += 1 - smoothstep(0.0, smooth, uv.y);\n";
            code += "r += smoothstep(1 - smooth, 1., uv.y);\n";
            code += "r = saturate(r);\n";
            code += "float2 uv2 = tex2D(source3, uv).rg;\n";
            code += "float2 uv3 = tex2D(source4, uv).rg;\n";
            code += "uv2 = lerp(uv, uv2, blend);\n";
            code += "uv3 = lerp(uv3, uv, blend);\n";
            code += "float4 r1 = tex2D(source, uv2);\n";
            code += "uv = lerp(uv, uv2, StrangerValue * blend - StrangerValue);\n";
            code += "float4 r2 = tex2D(source2, uv3);\n";
            code += "r1 = lerp(r1, r2, blend) ;\n";
            code += "r1.a = lerp(r1.a, r2.a, blend)*(1-r);\n";
            code += "return r1;\n";
            code += "}\n";

        }


        public override Node Create(Vector2 pos)
        {
            Function();

            MorphingPerfectPack node = ScriptableObject.CreateInstance<MorphingPerfectPack>();
            node.name = "Morphing Perfect Pack UV";
            node.rect = new Rect(pos.x, pos.y, 286, 580);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node; 
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_displacement.jpg");
            GUI.DrawTexture(new Rect(2, 0, 286, 52), preview);
            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();
            tex = (Texture2D)EditorGUI.ObjectField(new Rect(8, 98, 130, 130), tex, typeof(Texture2D), true);
            tex2 = (Texture2D)EditorGUI.ObjectField(new Rect(146, 98, 130, 130), tex2, typeof(Texture2D), true);
            tex3 = (Texture2D)EditorGUI.ObjectField(new Rect(8, 255, 130, 130), tex3, typeof(Texture2D), true);
            tex4 = (Texture2D)EditorGUI.ObjectField(new Rect(146, 255, 130, 130), tex4, typeof(Texture2D), true);
            EditorGUI.LabelField(new Rect(8, 236, 130, 130), "UV Before (32x32)");
            EditorGUI.LabelField(new Rect(146, 236, 130, 130), "UV After (32x32)");
            GUILayout.BeginHorizontal();
            GUILayout.Label("BEFORE");
            GUILayout.Label("AFTER");
            GUILayout.EndHorizontal();
            if (GUI.Button(new Rect(220, 0, 60, 30), "Help"))
            {
                Application.OpenURL("https://forum.vetasoft.store/post/124");
            }
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable2, tex);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable3, tex2);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable4, tex3);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable6, tex4);
            }

            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
         

            if (GUILayout.Button("Reset"))
            {
                Variable = 1;
                Variable5 = 1;
            }
            
            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
        
          //  AddMotion = GUILayout.Toggle(AddMotion, "Fix Side UV", GUILayout.Width(135));
        
         
            GUILayout.Label("Blend: " + (Variable*100).ToString("0.00")+"%");
            Variable =HorizontalSlider(Variable, 0, 1);
        

            GUILayout.Label("Stranger: " + Variable5.ToString("0.00"));
            Variable5 =HorizontalSlider(Variable5, 0, 4);
        


        }

        private string FinalVariable;
        private string FinalVariable2;
        private string FinalVariable3;
        private string FinalVariable4;
        private string FinalVariable5;
        private string FinalVariable6;
        [HideInInspector]
        public int MemoCount = -1;
        public override bool FixCalculate()
        {
            MemoCount = count;
            count++;
            return true;
        }

        public override bool Calculate()
        {
            tag = true;
            SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
            SuperFloat4 s_out = new SuperFloat4();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_MorphingPack_" + NodeCount;
            string SourceName = "MorphingPack_Before_" + NodeCount;
            string SourceName2 = "MorphingPack_After_" + NodeCount;
            string SourceName3 = "MorphingPack_UV_Before_" + NodeCount;
            string SourceName4 = "MorphingPack_UV_After_" + NodeCount;
            string DefaultNameVariable1 = "_MorphingPack_Blend_" + NodeCount;
            string DefaultNameVariable2 = "_MorphingPack_Stranger_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 4)) = " + Variable5.ToString();
            string uv = s_in.Result;
            string Source = SourceName;
            string Source2 = SourceName2;
            string Source3 = SourceName3;
            string Source4 = SourceName4;

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = SourceName;
            FinalVariable3 = SourceName2;
            FinalVariable4 = SourceName3;
            FinalVariable5 = DefaultNameVariable2;
            FinalVariable6 = SourceName4;

            // source
            if (s_in.Result == null)
            {
                uv = "i.texcoord";
                if (Source == "_MainTex") uv = "i.texcoord";
                if (Source == "_GrabTexture") uv = "i.screenuv";
            }
            else
            {
                uv = s_in.Result;
            }

            s_out.StringPreviewLines = s_in.StringPreviewNew;


            if (AddParameters)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = MorphingPerfectPack(" + uv + "," + Source + "," + Source2 + "," + Source3 + "," + Source4 + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = MorphingPerfectPack(" + uv + "," + Source + "," + Source2 + "," + Source3 + "," + Source4 + "," + Variable.ToString() + "," + Variable5.ToString() + ");\n";
            }

            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines;

            s_out.Result = DefaultName;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

            if (AddParameters)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
            }
            s_out.ParametersLines += SourceName + "(\"" + SourceName + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersLines += SourceName2 + "(\"" + SourceName2 + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersLines += SourceName3 + "(\"" + SourceName3 + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersLines += SourceName4 + "(\"" + SourceName4 + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName + ";\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName2 + ";\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName3 + ";\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName4 + ";\n";

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}