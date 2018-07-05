using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/Filters/Emboss")]
    public class Emboss : Node 
    {
        [HideInInspector]
        public const string ID = "Emboss";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0f;
        [HideInInspector]
        public float Variable2 = 0.3f;
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
            code += "float4 Emboss(sampler2D txt, float2 uv, float angle, float dist, float intensity, float g, float o)\n";
            code += "{\n";
            code += "angle = angle *3.1415926;\n";
            code += "intensity = intensity *0.25;\n";
            code += "#define rot(n) mul(n, float2x2(cos(angle), -sin(angle), sin(angle), cos(angle)))\n";
            code += "float m1 = 1; float m2 = 0; float m3 = 0;\n";
            code += "float m4 = 0; float m5 = 0; float m6 = 0;\n";
            code += "float m7 = 0; float m8 = 0; float m9 = -1;\n";
            code += "float Offset = 0.5;\n";
            code += "float Scale = 1;\n";
            code += "float4 r = float4(0, 0, 0, 0);\n";
            code += "dist = dist * 0.005;\n";
            code += "float4 rgb = tex2D(txt, uv);\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist, -dist))).a * m1*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(0, -dist))).a * m2*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(dist, -dist))).a * m3*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist, 0))).a * m4*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(0, 0))).a* m5*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(dist, 0))).a * m6*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(-dist, dist))).a * m7*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(0, dist))).a * m8*intensity;\n";
            code += "r += tex2D(txt, uv + rot(float2(dist, dist))).a * m9*intensity;\n";
            code += "r = lerp(r,dot(r.rgb,3),g);\n";
            code += "r = lerp(r+0.5,rgb+r,o);\n";
            code += "r = saturate(r);\n";
            code += "r.a = rgb.a;\n";
            code += "return r;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();
            Emboss node = ScriptableObject.CreateInstance<Emboss>();
            node.name = "Emboss";
            node.rect = new Rect(pos.x, pos.y, 172, 400);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_emboss.jpg");
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
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
            }
            if (GUILayout.Button("Reset"))
            {
                Variable = 0f;
                Variable2 = 8f;
                Variable3 = 1f;
                Variable4 = 1f;
                Variable5 = 1f;
            }

            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameters");

            GUILayout.Label("Angle: (-1 to 1) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, -1, 1);

            GUILayout.Label("Distance: (-4 to 4) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, -4, 4);

            GUILayout.Label("Intensity: (-2 to 2) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, -2, 2);

            GUILayout.Label("Grayfade: (0 to 1) " + Variable4.ToString("0.00"));
            Variable4 =HorizontalSlider(Variable4, 0, 1);

            GUILayout.Label("Original: (0 to 1) " + Variable5.ToString("0.00"));
            Variable5 =HorizontalSlider(Variable5, 0, 1);

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
            SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
            SuperFloat4 s_out = new SuperFloat4();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_Emboss_" + NodeCount;
            string DefaultNameVariable1 = "_Emboss_Angle_" + NodeCount;
            string DefaultNameVariable2 = "_Emboss_Distance_" + NodeCount;
            string DefaultNameVariable3 = "_Emboss_Intensity_" + NodeCount;
            string DefaultNameVariable4 = "_Emboss_grayfade_" + NodeCount;
            string DefaultNameVariable5 = "_Emboss_original_" + NodeCount;
            string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 16)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(-2, 2)) = " + Variable3.ToString();
            string DefaultParameters4 = ", Range(-2, 2)) = " + Variable4.ToString();
            string DefaultParameters5 = ", Range(-2, 2)) = " + Variable5.ToString();
            string uv = s_in.Result;
            string Source = "";

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
                s_out.ValueLine = "float4 " + DefaultName + " = Emboss(" + Source + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = Emboss(" + Source + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + ");\n";
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