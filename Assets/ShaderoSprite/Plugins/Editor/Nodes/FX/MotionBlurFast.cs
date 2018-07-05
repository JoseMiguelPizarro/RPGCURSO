using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/Filters/Motion Blur Fast")]
    public class MotionBlurFast : Node 
    {
        [HideInInspector]
        public const string ID = "MotionBlurFast";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0f;
        [HideInInspector]
        public float Variable2 = 1f;
        [HideInInspector]
        public float Variable3 = 1f;
        [HideInInspector]
        public float Variable4 = 1f;
        [HideInInspector]
        public float Variable5 = 1f;



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
            code += "float4 MotionBlurFast(sampler2D txt, float2 uv, float angle, float dist)\n";
            code += "{\n";
            code += "angle = angle *3.1415926;\n";
            code += "#define rot(n) mul(n, float2x2(cos(angle), -sin(angle), sin(angle), cos(angle)))\n";
            code += "float4 r = float4(0, 0, 0, 0);\n";
            code += "dist = dist * 0.005;\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist, -dist)));\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist*2, -dist*2)));\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist*3, -dist*3)));\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist*4, -dist*4)));\n";
            code += "r += tex2D(txt, uv);\n";
            code += "r += tex2D(txt, uv + rot(float2( dist, dist)));\n";
            code += "r += tex2D(txt, uv + rot(float2( dist*2, dist*2)));\n";
            code += "r += tex2D(txt, uv + rot(float2( dist*3, dist*3)));\n";
            code += "r += tex2D(txt, uv + rot(float2( dist*4, dist*4)));\n";
            code += "r = r/9;\n";
            code += "return r;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();
            MotionBlurFast node = ScriptableObject.CreateInstance<MotionBlurFast>();
            node.name = "Motion Blur Fast";
            node.rect = new Rect(pos.x, pos.y, 172, 280);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_motionblur.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();
            Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
             }
            if (GUILayout.Button("Reset"))
            {
                Variable = 0f;
                Variable2 = 1f;
             }

            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameters");

            GUILayout.Label("Angle: (-1 to 1) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, -1, 1);

            GUILayout.Label("Distance: (-4 to 4) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, -4, 4);

      
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
            SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
            SuperFloat4 s_out = new SuperFloat4();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_MotionBlurFast_" + NodeCount;
            string DefaultNameVariable1 = "_MotionBlurFast_Angle_" + NodeCount;
            string DefaultNameVariable2 = "_MotionBlurFast_Distance_" + NodeCount;
             string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 16)) = " + Variable2.ToString();
            string uv = s_in.Result;
            string Source = "";

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
 
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

            s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

            if (parametersOK)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = MotionBlurFast(" + Source + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = MotionBlurFast(" + Source + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString() + ");\n";
            }

            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;
            s_out.Result = DefaultName;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            if (parametersOK)
            {

                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
          
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
          
            }

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}