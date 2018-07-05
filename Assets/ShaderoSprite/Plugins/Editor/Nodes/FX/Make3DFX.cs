using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "RGBA/FX/Make 3D Effects")]
    public class Make3DFX : Node 
    {
        [HideInInspector]
        public const string ID = "Make3DFX";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0.5f;
        [HideInInspector]
        public float Variable2 = 8f;
        [HideInInspector]
        public float Variable3 = 0.3f;
        [HideInInspector]
        public float Variable4 = 0.2f;
        [HideInInspector]
        public float Variable5 = 0.2f;



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
            code += "float4 Make3DFX(sampler2D smp, float2 uv,float dist, float size, float x, float y, float light)\n";
            code += "{\n";
            code += "float4 overlay = float4(0, 0, 0, 1);\n";
            code += "float4 origin = tex2D(smp, uv);\n";
            code += "dist *= 0.03;\n";
            code += "for (int i = 0; i < size; i++)\n";
            code += "{\n";
            code += "uv.x += dist*x;\n";
            code += "uv.y += dist*y;\n";
            code += "float4 o= float4(0, 0, 0, 1);\n";
            code += "overlay = tex2D(smp, uv);\n";
            code += "float z = i / size;\n";
            code += "origin.rgb =  origin.rgb = lerp(origin.rgb +(light/size)*2, origin.rgb, z);\n";
            code += "origin = saturate(origin);\n";
            code += "o.a = overlay.a + origin.a * (1 - overlay.a);\n";
            code += "o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) / (o.a+0.0000001);\n";
            code += "o.a = saturate(o.a);\n";
            code += "o = lerp(origin, o, 1);\n";
            code += "origin = o;\n";
            code += "}\n";
            code += "return origin;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();
            Make3DFX node = ScriptableObject.CreateInstance<Make3DFX>();
            node.name = "Make 3D Effect";
            node.rect = new Rect(pos.x, pos.y, 172, 400);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateInput("Source", "SuperSource");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_make3D.jpg");
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
                Variable = 0.5f;
                Variable2 = 8f;
                Variable3 = 0.3f;
                Variable4 = 0.2f;
                Variable4 = 0.2f;
            }

            parametersOK = GUILayout.Toggle(parametersOK, "Add Parameters");

            GUILayout.Label("Distance: (0 to 1) " + Variable.ToString("0.00"));
            Variable =HorizontalSlider(Variable, 0, 1);

            GUILayout.Label("Size: (0 to 16) " + Variable2.ToString("0.00"));
            Variable2 =HorizontalSlider(Variable2, 0, 16);

            GUILayout.Label("Pos X: (-2 to 2) " + Variable3.ToString("0.00"));
            Variable3 =HorizontalSlider(Variable3, -2, 2);

            GUILayout.Label("Pos Y: (-2 to 2) " + Variable4.ToString("0.00"));
            Variable4 =HorizontalSlider(Variable4, -2, 2);

            GUILayout.Label("Light: (-2 to 2) " + Variable5.ToString("0.00"));
            Variable5 =HorizontalSlider(Variable5, -2, 2);

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
            string DefaultName = "_Make3DFX_" + NodeCount;
            string DefaultNameVariable1 = "_Make3DFX_Dist_" + NodeCount;
            string DefaultNameVariable2 = "_Make3DFX_Size_" + NodeCount;
            string DefaultNameVariable3 = "_Make3DFX_PosX_" + NodeCount;
            string DefaultNameVariable4 = "_Make3DFX_PosY_" + NodeCount;
            string DefaultNameVariable5 = "_Make3DFX_Light_" + NodeCount;
            string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(-1, 2)) = " + Variable2.ToString();
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
                s_out.ValueLine = "float4 " + DefaultName + " = Make3DFX(" + Source + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = Make3DFX(" + Source + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + ");\n";
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