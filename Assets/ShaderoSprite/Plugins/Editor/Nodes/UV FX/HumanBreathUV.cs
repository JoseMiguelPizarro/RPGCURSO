using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Organic Shader/Human Breath UV")]
    public class HumanBreathUV : Node
    {
        [HideInInspector]
        public const string ID = "HumanBreathUV";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0.05f;
        [HideInInspector]
        public float Variable2 = 0.05f;
        public static int count = 1;
        public static bool tag = false;
        public static string code;

        [HideInInspector]
        public bool AddParameters = true;
   
        public static void Init()
        {
            tag = false;
            count = 1;
        }

        public void Function()
        {
            code = "";
            code += "float2 HumanBreathUV(float2 uv, float4 rgb, float intensity, float speed)\n";
            code += "{\n";
            code += "float t = _Time * 15 * speed;\n";
            code += "float val = exp(sin(t * 3.1415) - 0.36787944) * intensity * 0.01;\n";
            code += "uv.y = lerp(uv.y, uv.y + val - 0.01, rgb.r);\n";
            code += "return uv;\n";
            code += "}\n";

        }


        public override Node Create(Vector2 pos)
        {
            Function();

            HumanBreathUV node = ScriptableObject.CreateInstance<HumanBreathUV>();

            node.name = "Human Breath UV";
            node.rect = new Rect(pos.x, pos.y, 172, 300);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateOutput("UV", "SuperFloat2");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_displacement.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            GUILayout.EndHorizontal();
            Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));

            if (GUILayout.Button("Reset"))
            {
                Variable = 1f;
                Variable2 = 1f;
            }
            if (GUI.Button(new Rect(110, 0, 60, 30), "Help"))
            {
                Application.OpenURL("https://forum.vetasoft.store/post/179");
            }
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            }

            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

            GUILayout.Label("Intensity: (0 to 2) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0f, 2f);
            GUILayout.Label("Speed: (0 to 3) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0f, 3f);



        }

        private string FinalVariable;
        private string FinalVariable2;
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
            SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
            SuperFloat2 s_out = new SuperFloat2();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_HumanBreath_" + NodeCount;
            string DefaultNameVariable1 = "_HumanBreat_Intensity_" + NodeCount;
            string DefaultNameVariable2 = "_HumanBreat_Speed_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 2)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 3)) = " + Variable2.ToString();
            string uv = s_in.Result;
            string RGBA = s_in2.Result;
            string Source = "";

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;

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
             
            s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

       
            if (AddParameters)
            {
                s_out.ValueLine = "float2 " + DefaultName + " = HumanBreathUV(" + uv + "," + RGBA + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float2 " + DefaultName + " = HumanBreathUV(" + uv + "," + RGBA + "," + Variable.ToString() + "," + Variable2.ToString() + ");\n";
            }


            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

            s_out.Result = DefaultName;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            if (AddParameters)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
            }
            Outputs[0].SetValue<SuperFloat2>(s_out);

            count++;
            return true;
        }
    }
}