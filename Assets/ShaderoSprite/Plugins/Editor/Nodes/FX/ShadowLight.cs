using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/FX/ShadowLight")]
    public class ShadowLight : Node
    {
        [HideInInspector]
        public const string ID = "ShadowLight";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 8;
        [HideInInspector]
        public float Variable2 = 1.2f;
        [HideInInspector]
        public Color Variable3 = new Color(0, 0.7f, 1, 1);
        [HideInInspector]
        public float Variable4 = 1.5f;
        [HideInInspector]
        public float Variable5 = 0;
        [HideInInspector]
        public float Variable6 = 0;
        [HideInInspector]
        public float Variable7 = 1;

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
            code += "float4 ShadowLight(sampler2D source, float2 uv, float precision, float size, float4 color, float intensity, float posx, float posy,float fade)\n";
            code += "{\n";
            code += "int samples = precision;\n";
            code += "int samples2 = samples *0.5;\n";
            code += "float4 ret = float4(0, 0, 0, 0);\n";
            code += "float count = 0;\n";
            code += "for (int iy = -samples2; iy < samples2; iy++)\n";
            code += "{\n";
            code += "for (int ix = -samples2; ix < samples2; ix++)\n";
            code += "{\n";
            code += "float2 uv2 = float2(ix, iy);\n";
            code += "uv2 /= samples;\n";
            code += "uv2 *= size*0.1;\n";
            code += "uv2 += float2(-posx,posy);\n";
            code += "uv2 = saturate(uv+uv2);\n";
            code += "ret += tex2D(source, uv2);\n";
            code += "count++;\n";
            code += "}\n";
            code += "}\n";
            code += "ret = lerp(float4(0, 0, 0, 0), ret / count, intensity);\n";
            code += "ret.rgb = color.rgb;\n";
            code += "float4 m = ret;\n";
            code += "float4 b = tex2D(source, uv);\n";
            code += "ret = lerp(ret, b, b.a);\n";
            code += "ret = lerp(m,ret,fade);\n";
            code += "return ret;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            ShadowLight node = ScriptableObject.CreateInstance<ShadowLight>();

            node.name = "Shadow / Light";
            node.rect = new Rect(pos.x, pos.y, 172, 520);

            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {


            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_shadowlight.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();

            Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));

            if (GUILayout.Button("Reset"))
            {
                Variable = 8;
                Variable2 = 1.2f;
                Variable3 = new Color(0, 0.7f, 1, 1);
                Variable4 = 1.5f;
                Variable5 = 0;
                Variable6 = 0;
                Variable7 = 1;
            }


            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

            // Paramaters
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetColor(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable6, Variable6);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable7, Variable7);

            }
            GUILayout.Label("Precision: (1 to 32) " + Variable.ToString("0.00"));
            GUILayout.Label("*Higher > Slower*");
            Variable =HorizontalSlider(Variable, 1, 32);
            GUILayout.Label("Size: (0 to 16) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0, 16);
            GUILayout.Label("Color:");
            Variable3 = EditorGUILayout.ColorField("", Variable3);

            GUILayout.Label("Intensity: (0 to 4) " + Variable4.ToString("0.00"));
            Variable4 =HorizontalSlider(Variable4, 0, 4);

            GUILayout.Label("PosX: (-1 to 1) " + Variable5.ToString("0.00"));
            Variable5 =HorizontalSlider(Variable5, -1, 1);

            GUILayout.Label("PosY: (-1 to 1) " + Variable6.ToString("0.00"));
            Variable6 =HorizontalSlider(Variable6, -1, 1);

            GUILayout.Label("NoSprite: (0 to 1) " + Variable7.ToString("0.00"));
            Variable7 =HorizontalSlider(Variable7, 0, 1);



        }
        private string FinalVariable;
        private string FinalVariable2;
        private string FinalVariable3;
        private string FinalVariable4;
        private string FinalVariable5;
        private string FinalVariable6;
        private string FinalVariable7;
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
            string DefaultName = "_ShadowLight_" + NodeCount;
            string DefaultNameVariable1 = "_ShadowLight_Precision_" + NodeCount;
            string DefaultNameVariable2 = "_ShadowLight_Size_" + NodeCount;
            string DefaultNameVariable3 = "_ShadowLight_Color_" + NodeCount;
            string DefaultNameVariable4 = "_ShadowLight_Intensity_" + NodeCount;
            string DefaultNameVariable5 = "_ShadowLight_PosX_" + NodeCount;
            string DefaultNameVariable6 = "_ShadowLight_PosY_" + NodeCount;
            string DefaultNameVariable7 = "_ShadowLight_NoSprite_" + NodeCount;
            string DefaultParameters1 = ", Range(1, 32)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, 16)) = " + Variable2.ToString();
            string DefaultParameters3 = ", COLOR) = (" + Variable3.r + "," + Variable3.g + "," + Variable3.b + "," + Variable3.a + ")";
            string DefaultParameters4 = ", Range(0, 4)) = " + Variable4.ToString();
            string DefaultParameters5 = ", Range(-1, 1)) = " + Variable5.ToString();
            string DefaultParameters6 = ", Range(-1, 1)) = " + Variable6.ToString();
            string DefaultParameters7 = ", Range(0, 1)) = " + Variable7.ToString();
            string uv = s_in.Result;
            string Source = "";

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = DefaultNameVariable4;
            FinalVariable5 = DefaultNameVariable5;
            FinalVariable6 = DefaultNameVariable6;
            FinalVariable7 = DefaultNameVariable7;

            // uv
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
                s_out.ValueLine = "float4 " + DefaultName + " = ShadowLight(" + Source + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + "," + DefaultNameVariable6 + "," + DefaultNameVariable7 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = ShadowLight(" + Source + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + "," + Variable6.ToString() + "," + Variable7.ToString() + ");\n";
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
                s_out.ParametersLines += DefaultNameVariable6 + "(\"" + DefaultNameVariable6 + "\"" + DefaultParameters6 + "\n";
                s_out.ParametersLines += DefaultNameVariable7 + "(\"" + DefaultNameVariable7 + "\"" + DefaultParameters7 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float4 " + DefaultNameVariable3 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable6 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable7 + ";\n";
            }

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}