using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Pack/Pattern Movement Pack")]
    public class PatternMovementPack : Node
    {
        [HideInInspector]
        public const string ID = "PatternMovementPack";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 0;
        [HideInInspector]
        public float Variable2 = 0;
        [HideInInspector]
        public float Variable3 = 1;
        [HideInInspector]
        public float Variable4 = 1;
        [HideInInspector]
        public float Variable5 = 1;
        public static int count = 1;
        public static bool tag = false;
        public static string code;
        [HideInInspector]
        public Texture2D tex;
        [HideInInspector]
        public Texture2D tex2;

        [HideInInspector]
        public bool AddParameters = true;
        [HideInInspector]
        public bool AddMask = true;
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
            code += "float4 PatternMovementPack(float2 uv, sampler2D source, sampler2D source2, float posx, float posy, float speed, float v1, float v2)\n";
            code += "{\n";
            code += "float t = _Time * 20 * speed;\n";
            code += "float2 mov =float2(posx*t,posy*t);\n";
            code += "float2 muv = fmod(uv+mov,1);\n";
            code += "float2 muv2 = fmod(uv+mov*0.7,1);\n";
            code += "float4 rgba=tex2D(source, muv);\n";
            code += "float4 mask=tex2D(source2, muv2);\n";
            code += "uv = fmod(abs(uv+float2(posx*t, posy*t)),1);\n";
            code += "float4 result = tex2D(source, uv);\n";
            code += "result.a = lerp(0,result.a,v1) * rgba.a * lerp(mask.a,mask.r,v2);\n";
            code += "return result;\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {
            Function();

            PatternMovementPack node = ScriptableObject.CreateInstance<PatternMovementPack>();
            node.name = "PatternMovement Pack";
            node.rect = new Rect(pos.x, pos.y, 342, 410);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_patternmovement.jpg");
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
                NodeEditor._Shadero_Material.SetFloat(FinalVariable6, Variable4);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable7, Variable5);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable4, tex);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable5, tex2);
            }


      

            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");

            if (GUILayout.Button("Reset", GUILayout.Width(190)))
            {
                Variable = 0;
                Variable2 = 0;
                Variable3 = 1;
                Variable4 = 1;
                Variable5 = 1;
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
            GUILayout.Label(" ", GUILayout.Width(140));
            GUILayout.Label("Pos Y: (-1 to 1) " + Variable2.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable2 = HorizontalSlider(Variable2, -1, 1, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            AddMask = GUILayout.Toggle(AddMask, "Add Mask", GUILayout.Width(190));          
             GUILayout.Label("Speed: (-3 to 3) " + Variable3.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable3 = HorizontalSlider(Variable3, -3, 3, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label(" ");
            GUILayout.Label("Intensity: (0 to 1) " + Variable4.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable4 = HorizontalSlider(Variable4, 0, 1, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ");
            GUILayout.Label("Mask Intensity: (0 to 1) " + Variable5.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable5 = HorizontalSlider(Variable5, 0, 1, GUILayout.Width(190),190);
            GUILayout.EndHorizontal();
            if (AddMask) tex2 = (Texture2D)EditorGUI.ObjectField(new Rect(8, 227, 130, 130), tex2, typeof(Texture2D), true);


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
            SuperFloat4 s_out = new SuperFloat4();

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_PatternMovementPack_" + NodeCount;
            string SourceName = "PatternMovementPack_" + NodeCount;
            string SourceName2 = "PatternMovementPackMask_" + NodeCount;
            string DefaultNameVariable1 = "_PatternMovementPack_ValueX_" + NodeCount;
            string DefaultNameVariable2 = "_PatternMovementPack_ValueY_" + NodeCount;
            string DefaultNameVariable3 = "_PatternMovementPack_Speed_" + NodeCount;
            string DefaultNameVariable4 = "_PatternMovementPack_Intensity_" + NodeCount;
            string DefaultNameVariable5 = "_PatternMovementPack_MaskIntensity_" + NodeCount;
            string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(-1, 1)) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(-3, 3)) = " + Variable3.ToString();
            string DefaultParameters4 = ", Range(0, 1)) = " + Variable.ToString();
            string DefaultParameters5 = ", Range(0, 1)) = " + Variable.ToString();
            string uv = s_in.Result;
            string Source = SourceName;
            string Source2 = SourceName2;

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = SourceName;
            FinalVariable5 = SourceName2;
            FinalVariable6 = DefaultNameVariable4;
            FinalVariable7 = DefaultNameVariable5;


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

            if (!AddMask) Source2 = Source;


            if (AddParameters)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = PatternMovementPack(" + uv + "," + Source + "," + Source2 + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = PatternMovementPack(" + uv + "," + Source + "," + Source2 + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + ");\n";
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
                s_out.ParametersLines += DefaultNameVariable4 + "(\"" + DefaultNameVariable4 + "\"" + DefaultParameters4 + "\n";
                s_out.ParametersLines += DefaultNameVariable5 + "(\"" + DefaultNameVariable5 + "\"" + DefaultParameters5 + "\n";
                 s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
              }
            s_out.ParametersLines += SourceName + "(\"" + SourceName + "(RGB)\", 2D) = \"white\" { }\n";
            if (AddMask) s_out.ParametersLines += SourceName2 + "(\"" + SourceName2 + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName + ";\n";
            if (AddMask) s_out.ParametersDeclarationLines += "sampler2D " + SourceName2 + ";\n";

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
    }
}