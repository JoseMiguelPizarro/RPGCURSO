using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
    [Node(false, "UV/FX (UV)/Rotation UV")]

    public class RotationUV : Node
    {
        public const string ID = "RotationUV";
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0;
        [HideInInspector]
        public float Variable2 = 0.5f;
        [HideInInspector]
        public float Variable3 = 0.5f;
        [HideInInspector]
        public float Variable4 = 0f;
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
            code += "float2 RotationUV(float2 uv, float rot, float posx, float posy, float speed)\n";
            code += "{\n";
            code += "rot=rot+(_Time*speed*360);\n";
            code += "uv = uv - float2(posx, posy);\n";
            code += "float angle = rot * 0.01744444;\n";
            code += "float sinX = sin(angle);\n";
            code += "float cosX = cos(angle);\n";
            code += "float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);\n";
            code += "uv = mul(uv, rotationMatrix) + float2(posx, posy);\n";
            code += "return uv;\n";
            code += "}\n";
        }
        public override Node Create(Vector2 pos)
        {
            Function();
            RotationUV node = ScriptableObject.CreateInstance<RotationUV>();
            node.name = "Rotation UV";
            node.rect = new Rect(pos.x, pos.y, 172, 360);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("UV", "SuperFloat2");
            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_rotation.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
            Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
            GUILayout.EndHorizontal();


            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
            if (GUILayout.Button("Reset"))
            {
                Variable = 0;
                Variable2 = 0.5f;
                Variable3 = 0.5f;
                Variable4 = 0f;
            }

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
            }

            GUILayout.Label("Rot: (-360 to 360) " + Variable.ToString("0."));
            Variable =HorizontalSlider(Variable, -360, 360);
            GUILayout.Label("Pos X: (-2 to 2) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, -2, 2);
            GUILayout.Label("Pos Y: (-2 to 2) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, -2, 2);
            GUILayout.Label("Speed: (-8 to 8) " + Variable4.ToString("0.00"));
            Variable4 =HorizontalSlider(Variable4, -8, 8);

        }

        private string FinalVariable1;
        private string FinalVariable2;
        private string FinalVariable3;
        private string FinalVariable4;
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
            string DefaultName = "RotationUV_";
            string DefaultNameVariable1 = DefaultName + "Rotation_" + NodeCount;
            string DefaultNameVariable2 = DefaultName + "Rotation_PosX_" + NodeCount;
            string DefaultNameVariable3 = DefaultName + "Rotation_PosY_" + NodeCount;
            string DefaultNameVariable4 = DefaultName + "Rotation_Speed_" + NodeCount;
            DefaultName = DefaultName + NodeCount;
            string DefaultParameters1 = ", Range(-360, 360)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(-1, 2)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(-1, 2)) =" + Variable3.ToString();
            string DefaultParameters4 = ", Range(-8, 8)) =" + Variable4.ToString();
            string VoidName = "RotationUV";
            string PreviewVariable = s_in.Result;

            if (PreviewVariable == null) PreviewVariable = "i.texcoord";

            FinalVariable1 = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = DefaultNameVariable4;

            s_out.StringPreviewLines = s_in.StringPreviewNew;

            if (AddParameters)
            {
                s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
                + ","
                + DefaultNameVariable1 + ","
                + DefaultNameVariable2 + ","
                + DefaultNameVariable3 + ","
                + DefaultNameVariable4 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
                     + ","
                     + Variable.ToString() + ","
                     + Variable2.ToString() + ","
                     + Variable3.ToString() + ","
                     + Variable4.ToString() + ");\n";

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

                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
            }

            Outputs[0].SetValue<SuperFloat2>(s_out);

            count++;

            return true;
        }
    }
}
