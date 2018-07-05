using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Mask/Mask 2 UV")]
    public class Mask2uv : Node
    {
        [HideInInspector]
        public const string ID = "Mask2uv";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 1f;
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
        public override Node Create(Vector2 pos)
        {
            Mask2uv node = ScriptableObject.CreateInstance<Mask2uv>();

            node.name = "Mask With 2 UV";
            node.rect = new Rect(pos.x, pos.y, 172, 230);

            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateOutput("UV", "SuperFloat2");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_mask_2_rgba.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            GUILayout.EndHorizontal();
            Inputs[1].DisplayLayout(new GUIContent("UV2", "UV2"));
            Inputs[2].DisplayLayout(new GUIContent("RGBA Mask", "RGBA"));
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            }
            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");
            GUILayout.Label("Inverse: (0 to 1) " + Variable.ToString("0.00"));
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

            SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
            SuperFloat2 s_in2 = Inputs[1].GetValue<SuperFloat2>();
            SuperFloat4 s_in3 = Inputs[2].GetValue<SuperFloat4>();
            SuperFloat2 s_out = new SuperFloat2();


            string NodeCount = MemoCount.ToString();
            string DefautTypeFade = "float";
            string DefaultName = "Mask2uv" + NodeCount;
            string DefaultNameVariable1 = "_Mask2uv_Fade_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
            string PreviewVariable = s_in.Result;
            string PreviewVariable2 = s_in2.Result;
            string PreviewVariable3 = s_in3.Result;

            FinalVariable = DefaultNameVariable1;


            if (PreviewVariable == null) PreviewVariable = "i.texcoord";
            if (PreviewVariable2 == null) PreviewVariable2 = "i.texcoord";

            if (s_in3.Result == null)
            {
                PreviewVariable = "float4(0,1,1,1)";
            }

            s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew + s_in3.StringPreviewNew;

            if (parametersOK)
            {
                s_out.ValueLine = "float2 " + DefaultName + " = lerp(" + PreviewVariable + "," + PreviewVariable2 + ", lerp(" + PreviewVariable3 + ".r, 1 - " + PreviewVariable3 + ".r ," + DefaultNameVariable1 + "));\n";

            }
            else
            {
                s_out.ValueLine = "float2 " + DefaultName + " = lerp(" + PreviewVariable + "," + PreviewVariable2 + ", lerp(" + PreviewVariable3 + ".r, 1 - " + PreviewVariable3 + ".r ," + Variable.ToString() + "));\n";

            }
            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.Result = DefaultName;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines + s_in3.ParametersLines;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines + s_in3.ParametersDeclarationLines;
            if (parametersOK)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersDeclarationLines += DefautTypeFade + " " + DefaultNameVariable1 + ";\n";
            }
            Outputs[0].SetValue<SuperFloat2>(s_out);
            count++;
            return true;
        }
    }
}