using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/FX/Holographic Paralax")]
    public class HolographicParalax : Node
    {
        [HideInInspector]
        public const string ID = "HolographicParalax";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 1;
        [HideInInspector]
        public Color Variable2 = new Color(1, 1, 1, 1);
        [HideInInspector]
        public float Variable3 = 1;
        [HideInInspector]
        [Multiline(15)]
        public string result;

        [HideInInspector]
        public bool HDR = false;

        public static int count = 1;
        public static bool tag = false;
        public static string code;
        [HideInInspector]
        public bool parametersOK = true;


        public static void Init()
        {
            tag = false;
            count = 1;
        }
        public void Function()
        {
            code = "";
            code += "float4 HolographicParalax(float2 uv, sampler2D source, float value)\n";
            code += "{\n";
            code += "float rtx = -unity_ObjectToWorld[0][2];\n";
            code += "rtx = rtx * 0.1;\n";
            code += "float4 rgb = tex2D(source, uv + float2(rtx, 0));\n";
            code += "float r = tex2D(source, uv + float2(rtx * (1-value), 0)).r;\n";
            code += "float b = tex2D(source, uv + float2(rtx * (1+value), 0)).b;\n";
            code += "return float4(r, rgb.g, b,rgb.a);\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            HolographicParalax node = ScriptableObject.CreateInstance<HolographicParalax>();

            node.name = "Holographic Paralax";
            node.rect = new Rect(pos.x, pos.y, 180, 300);

            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_holoparalax.jpg");
            GUI.DrawTexture(new Rect(1, 0, 180, 46), preview);
            GUILayout.Space(50);
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));

            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            }

            GUILayout.Label("(0 to 1) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0, 1);

        }
        private string FinalVariable;
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
            WorldParalaxTag = true;
            SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
            SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
            SuperFloat4 s_out = new SuperFloat4();


            string NodeCount = MemoCount.ToString();
            string DefaultName = "_HolographicParalax_" + NodeCount;
            string DefaultNameVariable1 = "_HolographicParalax_Size_" + NodeCount;
            string DefaultParameters1 = ", Range(1, 0)) = " + Variable.ToString();
            string uv = s_in.Result;
            string Source = "";

            FinalVariable = DefaultNameVariable1;

            // uv
            if (s_in2.Result == null)
            {
                Source = "_MainTex";
            }
            else
            {
                Source = s_in2.Result;
            }

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
            if (parametersOK)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = HolographicParalax(" + uv+ "," + Source + "," + DefaultNameVariable1 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = HolographicParalax(" + uv + "," + Source + "," + Variable.ToString() + "));\n";
            }
            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

            s_out.Result = DefaultName;

            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            if (parametersOK)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            }
            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            result = s_out.StringPreviewNew;
            return true;
        }
    }
}