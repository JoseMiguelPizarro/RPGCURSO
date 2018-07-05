using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/FX/Inner Glow")]
    public class InnerGlow : Node
    {
        [HideInInspector]
        public const string ID = "InnerGlow";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 2;
        [HideInInspector]
        public float Variable2 = 3;
        [HideInInspector]
        public Color Variable3 = new Color(1, 1, 0, 1);

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
            code += "float InnerGlowAlpha(sampler2D source, float2 uv)\n";
            code += "{\n";
            code += "return (1 - tex2D(source, uv).a);\n";
            code += "}\n";
            code += "float4 InnerGlow(float2 uv, sampler2D source, float Intensity, float size, float4 color)\n";
            code += "{\n";
            code += "float step1 = 0.00390625f * size*2;\n";
            code += "float step2 = step1 * 2;\n";
            code += "float4 result = float4 (0, 0, 0, 0);\n";
            code += "float2 texCoord = float2(0, 0);\n";
            code += "texCoord = uv + float2(-step2, -step2); result += InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step1, -step2); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(0, -step2); result += 6.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step1, -step2); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step2, -step2); result += InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step2, -step1); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step1, -step1); result += 16.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(0, -step1); result += 24.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step1, -step1); result += 16.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step2, -step1); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step2, 0); result += 6.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step1, 0); result += 24.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv; result += 36.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step1, 0); result += 24.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step2, 0); result += 6.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step2, step1); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step1, step1); result += 16.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(0, step1); result += 24.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step1, step1); result += 16.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step2, step1); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step2, step2); result += InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(-step1, step2); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(0, step2); result += 6.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step1, step2); result += 4.0 * InnerGlowAlpha(source, texCoord);\n";
            code += "texCoord = uv + float2(step2, step2); result += InnerGlowAlpha(source, texCoord);\n";
            code += "result = result*0.00390625;\n";
            code += "result = lerp(tex2D(source,uv),color*Intensity,result*color.a);\n";
            code += "result.a = tex2D(source, uv).a;\n";
            code += "return saturate(result);\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            InnerGlow node = ScriptableObject.CreateInstance<InnerGlow>();

            node.name = "Inner Glow FX";
            node.rect = new Rect(pos.x, pos.y, 172, 320);

            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {


            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_innerglow.jpg");
            GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();

            Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));

            if (GUILayout.Button("Reset"))
            {
                Variable = 2;
                Variable2 = 3;
                Variable3 = new Color(1, 1, 0, 1);

            }

 
            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

            // Paramaters
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetColor(FinalVariable3, Variable3);
            }
            GUILayout.Label("Intensity: (0 to 16) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0, 16);
            GUILayout.Label("Size: (0 to 16) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0, 16);
            GUILayout.Label("Color:");
            Variable3 = EditorGUILayout.ColorField("", Variable3);




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
            SuperFloat4 s_out = new SuperFloat4();


            string NodeCount = MemoCount.ToString();
            string DefaultName = "_InnerGlowHQ_" + NodeCount;
            string DefaultNameVariable1 = "_InnerGlowHQ_Intensity_" + NodeCount;
            string DefaultNameVariable2 = "_InnerGlowHQ_Size_" + NodeCount;
            string DefaultNameVariable3 = "_InnerGlowHQ_Color_" + NodeCount;
            string DefaultParameters1 = ", Range(1, 16)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(1, 16)) = " + Variable2.ToString();
            string DefaultParameters3 = ", COLOR) = (" + Variable3.r + "," + Variable3.g + "," + Variable3.b + "," + Variable3.a + ")";
            string uv = s_in.Result;
            string Source = "";

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;

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
                s_out.ValueLine = "float4 " + DefaultName + " = InnerGlow(" + uv + "," + Source + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = InnerGlow(" + uv + "," + Source + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + ");\n";
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
                s_out.ParametersDeclarationLines += "float4 " + DefaultNameVariable3 + ";\n";
            }

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}