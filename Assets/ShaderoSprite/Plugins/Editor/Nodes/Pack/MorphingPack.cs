using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Pack/Morphing Pack")]
    public class MorphingPack : Node
    {
        [HideInInspector]
        public const string ID = "MorphingPack";
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
            code += "float4 MorphingPack(float2 uv, sampler2D source, sampler2D source2, sampler2D source3, float blend, float StrangerValue)\n";
            code += "{\n";
            code += "float2 uv2 = tex2D(source3, uv).rg;\n";
            code += "uv2 = lerp(uv, uv2, blend);\n";
            code += "float4 r1 = tex2D(source, uv2);\n";
            code += "uv = lerp(uv, uv2, StrangerValue * blend - StrangerValue);\n";
            code += "float4 r2 = tex2D(source2, uv);\n";
            code += "float ra=r1.a;\n";
            code += "r1 = lerp(r1, float4(r2.rgb, r2.a), blend);\n";
            code += "r1.a = lerp(ra, r2.a, blend);\n";
            code += "return r1;\n";
            code += "}\n";

        }


        public override Node Create(Vector2 pos)
        {
            Function();

            MorphingPack node = ScriptableObject.CreateInstance<MorphingPack>();
            node.name = "MorphingPack UV";
            node.rect = new Rect(pos.x, pos.y, 286, 450);
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
            EditorGUI.LabelField(new Rect(8, 236, 130, 130), "UV (32x32)");
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
            }

            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");
            GUILayout.Label(" ");


            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");
            if (GUILayout.Button("Reset", GUILayout.Width(135)))
            {
                Variable = 1;
                Variable5 = 1;
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");
            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters", GUILayout.Width(135));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ");
            AddMotion = GUILayout.Toggle(AddMotion, "Fix Side UV", GUILayout.Width(135));
            GUILayout.EndHorizontal();

         
            GUILayout.BeginHorizontal();
            GUILayout.Label(" ");
            GUILayout.Label("Blend: " + (Variable*100).ToString("0.00")+"%", GUILayout.Width(135));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable = HorizontalSlider(Variable, 0, 1, GUILayout.Width(135),135);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label(" ");
            GUILayout.Label("Stranger: " + Variable5.ToString("0.00"), GUILayout.Width(135));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable5 =HorizontalSlider(Variable5, 0, 4, GUILayout.Width(135),135);
            GUILayout.EndHorizontal();



        }

        private string FinalVariable;
        private string FinalVariable2;
        private string FinalVariable3;
        private string FinalVariable4;
        private string FinalVariable5;
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

            if (AddMotion) FixUV.tag = true;
            string NodeCount = MemoCount.ToString();
            string DefaultName = "_MorphingPack_" + NodeCount;
            string SourceName = "MorphingPack_" + NodeCount;
            string SourceName2 = "MorphingPack2_" + NodeCount;
            string SourceName3 = "MorphingPack3_" + NodeCount;
            string DefaultNameVariable1 = "_MorphingPack_Blend_" + NodeCount;
            string DefaultNameVariable2 = "_MorphingPack_Stranger_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 4)) = " + Variable5.ToString();
            string uv = s_in.Result;
            string Source = SourceName;
            string Source2 = SourceName2;
            string Source3 = SourceName3;

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = SourceName;
            FinalVariable3 = SourceName2;
            FinalVariable4 = SourceName3;
            FinalVariable5 = DefaultNameVariable2;

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
                s_out.ValueLine = "float4 " + DefaultName + " = MorphingPack(" + uv + "," + Source + "," + Source2 + "," + Source3 + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = MorphingPack(" + uv + "," + Source + "," + Source2 + "," + Source3 + "," + Variable.ToString() + "," + Variable5.ToString() + ");\n";
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
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName + ";\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName2 + ";\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName3 + ";\n";

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}