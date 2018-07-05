using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Pack/Displacement Pack")]
    public class DisplacementPack : Node
    {
        [HideInInspector]
        public const string ID = "DisplacementPack";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0;
        [HideInInspector]
        public float Variable2 = 0;
        [HideInInspector]
        public float Variable3 = 1;
        public static int count = 1;
        public static bool tag = false;
        public static string code;
        [HideInInspector]
        public Texture2D tex;

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
            code += "float4 DisplacementPack(float2 uv,sampler2D source,float x, float y, float value, float motion, float motion2)\n";
            code += "{\n";
            code += "float t=_Time.y;\n";
            code += "float2 mov =float2(x*t,y*t)*motion;\n";
            code += "float2 mov2 =float2(x*t*2,y*t*2)*motion2;\n";
            code += "float4 rgba=tex2D(source, uv + mov);\n";
            code += "float4 rgba2=tex2D(source, uv + mov2);\n";
            code += "float r=(rgba2.r+rgba2.g+rgba2.b)/3;\n";
            code += "r*=rgba2.a;\n";
            code += "uv+=mov2*0.25;\n";
            code += "return tex2D(source,lerp(uv,uv+float2(rgba.r*x,rgba.g*y),value*r));\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            DisplacementPack node = ScriptableObject.CreateInstance<DisplacementPack>();
            node.name = "Displacement Pack";
            node.rect = new Rect(pos.x, pos.y, 342, 300);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_displacementpack.jpg");
            GUI.DrawTexture(new Rect(2, 0, 342, 52), preview);
            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();
            tex = (Texture2D)EditorGUI.ObjectField(new Rect(8, 78, 130, 130), tex, typeof(Texture2D), true);

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable4, tex);
            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");

            if (GUILayout.Button("Reset", GUILayout.Width(190)))
            {
                Variable = 0;
                Variable2 = 0;
                Variable3 = 1;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");
            GUILayout.Label("     ");
            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters", GUILayout.Width(190));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");
            GUILayout.Label("     ");
            GUILayout.Label("Pos X: (-1 to 1) " + Variable.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable = HorizontalSlider(Variable, -1, 1, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");
            GUILayout.Label("     ");
            GUILayout.Label("Pos Y: (-1 to 1) " + Variable2.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable2 = HorizontalSlider(Variable2, -1, 1, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            AddMotion = GUILayout.Toggle(AddMotion, "Add Motion", GUILayout.Width(120));
            GUILayout.Label(" ");
            GUILayout.Label("Value: (-3 to 3) " + Variable3.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            AddMotionAlternate = GUILayout.Toggle(AddMotionAlternate, "Add Motion Plus", GUILayout.Width(130));
            Variable3 = HorizontalSlider(Variable3, -3, 3, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();




        }

        private string FinalVariable;
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
            SuperFloat4 s_out = new SuperFloat4();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_DisplacementPack_" + NodeCount;
            string SourceName = "DisplacementPack_" + NodeCount;
            string DefaultNameVariable1 = "_DisplacementPack_ValueX_" + NodeCount;
            string DefaultNameVariable2 = "_DisplacementPack_ValueY_" + NodeCount;
            string DefaultNameVariable3 = "_DisplacementPack_Size_" + NodeCount;
            string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(-1, 1)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(-3, 3)) = " + Variable3.ToString();
            string uv = s_in.Result;
            string Source = SourceName;

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = SourceName;



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

            string m1 = "0";
            string m2 = "0";

            if (AddMotion) { m1 = "1"; }
            if (AddMotionAlternate) { m2 = "1"; }

            if (AddParameters)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = DisplacementPack(" + uv + "," + Source + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + m1 + "," + m2 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = DisplacementPack(" + uv + "," + Source + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + m1 + "," + m2 + ");\n";
            }

            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines;

            s_out.Result = DefaultName;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

            if (AddParameters)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
            }
            s_out.ParametersLines += SourceName + "(\"" + SourceName + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName + ";\n";
            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}