using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/FX/Shiny Only FX")]
    public class ShinyOnlyFX : Node
    {
        [HideInInspector]
        public const string ID = "ShinyOnlyFX";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0.0f;
        [HideInInspector]
        public float Variable2 = -0.1f;
        [HideInInspector]
        public float Variable3 = 0.25f;
        [HideInInspector]
        public float Variable4 = 1.0f;
        [HideInInspector]
        public float Variable5 = 1.0f;

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
            code += "float4 ShinyOnlyFX(float2 uv, float pos, float size, float smooth, float intensity, float speed)\n";
            code += "{\n";
            code += "pos = pos + 0.5+sin(_Time*20*speed)*0.5;\n";
            code += "uv = uv - float2(pos, 0.5);\n";
            code += "float a = atan2(uv.x, uv.y) + 1.4, r = 3.1415;\n";
            code += "float d = cos(floor(0.5 + a / r) * r - a) * length(uv);\n";
            code += "float dist = 1.0 - smoothstep(size, size + smooth, d);\n";
            code += "return dist*intensity;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            ShinyOnlyFX node = ScriptableObject.CreateInstance<ShinyOnlyFX>();

            node.name = "Shiny Only FX";
            node.rect = new Rect(pos.x, pos.y, 180, 400);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }
        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_shinyonly.jpg");
            GUI.DrawTexture(new Rect(2, 0, 180, 46), preview);

            
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();
       
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
            }

            if (GUILayout.Button("Reset"))
            {
                Variable = 0.0f;
                Variable2 = -0.1f;
                Variable3 = 0.25f;
                Variable4 = 1f;
                Variable5 = 1f;

            }
            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");
            GUILayout.Label("Pos: (-1 to 1) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, -1, 1);

            GUILayout.Label("Size: (-1 to 1) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, -1, 1);

            GUILayout.Label("Smooth: (0 to 1) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, 0, 1);

            GUILayout.Label("intensity: (0 to 4) " + Variable4.ToString("0.00"));
            Variable4 =HorizontalSlider(Variable4, 0, 4);

            GUILayout.Label("speed: (0 to 8) " + Variable5.ToString("0.00"));
            Variable5 =HorizontalSlider(Variable5, 0, 8);

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

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_ShinyOnlyFX_" + NodeCount;
            string DefaultNameVariable1 = "_ShinyOnlyFX_Pos_" + NodeCount;
            string DefaultNameVariable2 = "_ShinyOnlyFX_Size_" + NodeCount;
            string DefaultNameVariable3 = "_ShinyOnlyFX_Smooth_" + NodeCount;
            string DefaultNameVariable4 = "_ShinyOnlyFX_Intensity_" + NodeCount;
            string DefaultNameVariable5 = "_ShinyOnlyFX_Speed_" + NodeCount;
            string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(-1, 1)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(0, 1)) = " + Variable3.ToString();
            string DefaultParameters4 = ", Range(0, 4)) = " + Variable4.ToString();
            string DefaultParameters5 = ", Range(0, 8)) = " + Variable5.ToString();
            string uv = s_in.Result;
         
            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = DefaultNameVariable4;
            FinalVariable5 = DefaultNameVariable5;



            // source
            if (s_in.Result == null)
            {
                uv = "i.texcoord";
            }
            else
            {
                uv = s_in.Result;
            }

            s_out.StringPreviewLines = s_in.StringPreviewNew;

            if (parametersOK)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = ShinyOnlyFX(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = ShinyOnlyFX(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + ");\n";
            }

            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines;

            s_out.Result = DefaultName;

            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
            if (parametersOK)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
                s_out.ParametersLines += DefaultNameVariable4 + "(\"" + DefaultNameVariable4 + "\"" + DefaultParameters4 + "\n";
                s_out.ParametersLines += DefaultNameVariable5 + "(\"" + DefaultNameVariable5 + "\"" + DefaultParameters5 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
            }

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}