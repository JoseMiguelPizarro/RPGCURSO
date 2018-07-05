using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/FX/Plasma Light FX")]
    public class PlasmaLightFX : Node
    {
        [HideInInspector]
        public const string ID = "PlasmaLightFX";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0.5f;
        [HideInInspector]
        public float Variable2 = 0.5f;
        [HideInInspector]
        public float Variable3 = 1f;
        [HideInInspector]
        public bool parametersOK = true;


        public static int count = 1;
        public static bool tag = false;
        public static string code;


        public static void Init()
        {
            tag = false;
            count = 1;
        }





        public void Function()
        {
            code = "";
            code += "inline float RBFXmod2(float x,float modu)\n";
            code += "{\n";
            code += "return x - floor(x * (1.0 / modu)) * modu;\n";
            code += "}\n";
            code += "\n";
            code += "float3 RBFXrainbow2(float t)\n";
            code += "{\n";
            code += "t= RBFXmod2(t,1.0);\n";
            code += "float tx = t * 8;\n";
            code += "float r = clamp(tx - 4.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);\n";
            code += "float g = tx < 2.0 ? clamp(tx, 0.0, 1.0) : clamp(4.0 - tx, 0.0, 1.0);\n";
            code += "float b = tx < 4.0 ? clamp(tx - 2.0, 0.0, 1.0) : clamp(6.0 - tx, 0.0, 1.0);\n";
            code += "return float3(r, g, b);\n";
            code += "}\n";
            code += "\n";
            code += "float4 PlasmaLight(float4 txt, float2 uv, float _Fade, float speed, float bw)\n";
            code += "{\n";
            code += "float _TimeX=_Time.y * speed;\n";
            code += "float a = 1.1 + _TimeX * 2.25;\n";
            code += "float b = 0.5 + _TimeX * 1.77;\n";
            code += "float c = 8.4 + _TimeX * 1.58;\n";
            code += "float d = 610 + _TimeX * 2.03;\n";
            code += "float x1 = 2.0 * uv.x;\n";
            code += "float n = sin(a + x1) + sin(b - x1) + sin(c + 2.0 * uv.y) + sin(d + 5.0 * uv.y);\n";
            code += "n = RBFXmod2(((5.0 + n) / 5.0), 1.0);\n";
            code += "float4 nx=txt;\n";
            code += "n += nx.r * 0.2 + nx.g * 0.4 + nx.b * 0.2;\n";
            code += "float4 ret=float4(RBFXrainbow2(n),txt.a);\n";
            code += "float g=ret.g;\n";
            code += "ret= lerp(ret,g+ret,bw);\n";
            code += "ret = lerp(txt,txt+ret,_Fade);\n";
            code += "ret.a = txt.a;\n";
            code += "return ret;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            PlasmaLightFX node = ScriptableObject.CreateInstance<PlasmaLightFX>();

            node.name = "Plasma Light FX";

            node.rect = new Rect(pos.x, pos.y, 172, 285);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_plasmarainbowlight.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();
            Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            }

            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameters");

            GUILayout.Label("Add: (0 to 1) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0, 1);
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            }
            GUILayout.Label("Speed: (0 to 1) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0, 1);
            GUILayout.Label("Gray: (0 to 1) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, 0, 1);



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
            SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
            SuperFloat4 s_out = new SuperFloat4();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_PlasmaLightFX_" + NodeCount;
            string DefaultNameVariable1 = "_PlasmaLightFX_Fade_" + NodeCount;
            string DefaultNameVariable2 = "_PlasmaLightFX_Speed_" + NodeCount;
            string DefaultNameVariable3 = "_PlasmaLightFX_BW_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 1)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(0, 1)) = " + Variable3.ToString();
            string uv = s_in.Result;
            string rgba = s_in2.Result;

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;



            // source
            if (s_in.Result == null)
            {
                uv = "i.texcoord";
            }
            else
            {
                uv = s_in.Result;
            }
            if (s_in2.Result == null)
            {
                rgba = "float4(1,1,1,1)";
            }
            else
            {
                rgba = s_in2.Result;
            }
            s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

            if (parametersOK)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = PlasmaLight(" + rgba + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = PlasmaLight(" + rgba + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString()+"," + Variable3.ToString() + ");\n";
            }

            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

            s_out.Result = DefaultName;

            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            if (parametersOK)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
            }

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}