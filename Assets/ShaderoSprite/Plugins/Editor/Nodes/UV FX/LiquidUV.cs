using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Liquid UV")]

public class LiquidUV : Node
{
        public const string ID = "LiquidUV";
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 2;
        [HideInInspector]
        public float Variable2 = 2;
        [HideInInspector]
        public float Variable3 = 0.3f;
        [HideInInspector]
        public float Variable4 = 0.3f;
        [HideInInspector]
        public float Variable5 = 1f;
        [HideInInspector]
        public bool AddParameters = true;

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
            code += "float2 LiquidUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)\n";
            code += "{ Speed *= _Time * 100;\n";
            code += "float x = sin(p.y * 4 * WaveX + Speed);\n";
            code += "float y = cos(p.x * 4 * WaveY + Speed);\n";
            code += "x += sin(p.x)*0.1;\n";
            code += "y += cos(p.y)*0.1;\n";
            code += "x *= y;\n";
            code += "y *= x;\n";
            code += "x *= y + WaveY*8;\n";
            code += "y *= x + WaveX*8;\n";
            code += "p.x = p.x + x * DistanceX * 0.015;\n";
            code += "p.y = p.y + y * DistanceY * 0.015;\n";
            code += "\n";
            code += "return p;\n";
            code += "}\n";
         
        }
        public override Node Create(Vector2 pos)
        {
            Function();
            LiquidUV node = ScriptableObject.CreateInstance<LiquidUV>();

            node.name = "Liquid UV";
            node.rect = new Rect(pos.x, pos.y, 172, 380);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("UV", "SuperFloat2");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_liquid.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
            Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
            GUILayout.EndHorizontal();

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
            }
            if (GUILayout.Button("Reset"))
            {
                Variable = 2;
                Variable2 = 2;
                Variable3 = 0.3f;
                Variable4 = 0.3f;
                Variable5 = 1f;

            }
            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");


            GUILayout.Label("Wave X (0 to 2) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0, 2);
            GUILayout.Label("Wave Y (0 to 2) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0, 2);
            GUILayout.Label("Dist X (1 to 10) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, 0, 1);
            GUILayout.Label("DistY (1 to 10) " + Variable4.ToString("0.00"));
            Variable4 =HorizontalSlider(Variable4, 0, 1);
            GUILayout.Label("Speed (-2 to 2) " + Variable5.ToString("0.00"));
            Variable5 =HorizontalSlider(Variable5, -2, 2);


        }

        private string FinalVariable1;
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
            SuperFloat2 s_out = new SuperFloat2();


            string NodeCount = MemoCount.ToString();
            string DefaultName = "LiquidUV_";
            string DefaultNameVariable1 = DefaultName + "WaveX_" + NodeCount;
            string DefaultNameVariable2 = DefaultName + "WaveY_" + NodeCount;
            string DefaultNameVariable3 = DefaultName + "DistanceX_" + NodeCount;
            string DefaultNameVariable4 = DefaultName + "DistanceY_" + NodeCount;
            string DefaultNameVariable5 = DefaultName + "Speed_" + NodeCount;

            DefaultName = DefaultName + NodeCount;

            string DefaultParameters1 = ", Range(0, 2)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 2)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(0, 1)) = " + Variable3.ToString();
            string DefaultParameters4 = ", Range(0, 1)) = " + Variable4.ToString();
            string DefaultParameters5 = ", Range(-2, 2)) = " + Variable5.ToString();
            string VoidName = "LiquidUV";
            string PreviewVariable = s_in.Result;
            if (PreviewVariable == null) PreviewVariable = "i.texcoord";

            FinalVariable1 = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = DefaultNameVariable4;
            FinalVariable5 = DefaultNameVariable5;

            s_out.StringPreviewLines = s_in.StringPreviewNew;

            if (AddParameters)
            {
                s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
                + ","
                + DefaultNameVariable1 + ","
                + DefaultNameVariable2 + ","
                + DefaultNameVariable3 + ","
                + DefaultNameVariable4 + ","
                + DefaultNameVariable5 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
                     + ","
                     + Variable.ToString() + ","
                     + Variable2.ToString() + ","
                     + Variable3.ToString() + ","
                     + Variable4.ToString() + ","
                     + Variable5.ToString() + ");\n";

            }
            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.Result = DefaultName;

            s_out.ParametersLines += s_in.ParametersLines;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;


            if (AddParameters)
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




            Outputs[0].SetValue<SuperFloat2>(s_out);

            count++;

            return true;
        }
    }
}