using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Organic Shader/Human Breath Ex UV")]
    public class HumanBreathExUV : Node
    {
        [HideInInspector]
        public const string ID = "HumanBreathExUV";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0.05f;
        [HideInInspector]
        public float Variable2 = 0.05f;
        [HideInInspector]
        public float Variable3 = 0.05f;
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
            code += "float2 HumanBreathExUV(float2 uv, sampler2D smp, float intensity, float speed, float sideintensity)\n";
            code += "{\n";
            code += "float t = _Time * 15 * speed;\n";
            code += "float i = intensity * 0.01;\n";
            code += "float si = sideintensity * 0.01;\n";
            code += "float val = (sin(t * 3.1415));\n";
            code += "float val2 = exp(-sin(t * 3.1415));\n";
            code += "float org = val * i - i / 2;\n";
            code += "float4 n = tex2D(smp, uv+org);\n";
            code += "uv.y = lerp(uv.y, uv.y+org, n.r);\n";
            code += "n = tex2D(smp, uv);\n";
            code += "uv.x = lerp(uv.x, uv.x + val2 * si, n.g);\n";
            code += "uv.x = lerp(uv.x, uv.x - val2 * si, n.b);\n";
            code += "return uv;\n";
            code += "}\n";

        }


        public override Node Create(Vector2 pos)
        {
            Function();

            HumanBreathExUV node = ScriptableObject.CreateInstance<HumanBreathExUV>();

            node.name = "Human Breath Ex UV";
            node.rect = new Rect(pos.x, pos.y, 172, 400);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
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
            Inputs[1].DisplayLayout(new GUIContent("Source SOF Texture", "Shadero Organic File"));

            if (GUILayout.Button("Reset"))
            {
                Variable = 1f;
                Variable2 = 1f;
                Variable3 = 1f;
            }
            if (GUI.Button(new Rect(110, 0, 60, 30), "Help"))
            {
                Application.OpenURL("https://forum.vetasoft.store/post/179");
            }
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            }

            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

            GUILayout.Label("Intensity: (0 to 2) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0f, 2f);
            GUILayout.Label("Speed: (0 to 3) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0f, 3f);
            GUILayout.Label("Side Intensity");
            GUILayout.Label("(0 to 2) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, 0f, 2f);



        }

        private string FinalVariable;
        private string FinalVariable2;
        private string FinalVariable3;
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
            SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
            SuperFloat2 s_out = new SuperFloat2();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_HumanBreathEx_" + NodeCount;
            string DefaultNameVariable1 = "_HumanBreatEx_Intensity_" + NodeCount;
            string DefaultNameVariable2 = "_HumanBreatEx_Speed_" + NodeCount;
            string DefaultNameVariable3 = "_HumanBreatEx_Side_Intensity_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 2)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 3)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(0, 2)) = " + Variable3.ToString();
            string uv = s_in.Result;
            string RGBA = s_in2.Result;
            string Source = "";

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;

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
            if (s_in2.Result == null)
            {
                Source = "_MainTex";
                if (uv == "i.texcoord") Source = "_MainTex";
                if (uv == "i.screenuv") Source = "_GrabTexture";
            }
            else
            {
                Source = s_in2.Result;
            }
             
             
            s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

       
            if (AddParameters)
            {
                s_out.ValueLine = "float2 " + DefaultName + " = HumanBreathExUV(" + uv + "," + RGBA + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float2 " + DefaultName + " = HumanBreathExUV(" + uv + "," + RGBA + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + ");\n";
            }


            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

            s_out.Result = DefaultName;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            if (AddParameters)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
            }
            Outputs[0].SetValue<SuperFloat2>(s_out);

            count++;
            return true;
        }
    }
}