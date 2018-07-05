using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Pack/Sprite Sheet UV Anim Pack")]
    public class SpriteSheetUVAnimPack : Node
    {
        [HideInInspector]
        public const string ID = "SpriteSheetUVAnimPack";
        [HideInInspector]
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public float Variable = 4;
        [HideInInspector]
        public float Variable2 = 0;
        [HideInInspector]
        public float Variable3 = 1;
        [HideInInspector]
        public float Variable4 = 1;
        public static int count = 1;
        public static bool tag = false;
        public static string code;
        [HideInInspector]
        public Texture2D tex;

        public static int total = 1;
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
            code += "float2 SpriteSheetUVAnimPack(float2 uv,sampler2D source,float size, float frame1, float frame2, float blend)\n";
            code += "{\n";
            code += "frame1 = int(frame1);\n";
            code += "frame2 = int(frame2);\n";
            code += "uv /= size;\n";
            code += "uv.y -=1/size;\n";
            code += "float2 uv2=uv;\n";
            code += "uv.x += fmod(frame1,size) / size;\n";
            code += "uv.y += 1-floor(frame1 / size) / size;\n";
            code += "uv2.x += fmod(frame2,size) / size;\n";
            code += "uv2.y += 1-floor(frame2 / size) / size;\n";
            code += "uv = tex2D(source, uv).rg;\n";
            code += "uv2 = tex2D(source, uv2).rg;\n";
            code += "uv = lerp(uv,uv2,blend);\n";
            code += "return uv;\n";
            code += "}\n";

        }


        public override Node Create(Vector2 pos)
        {
            Function();

            SpriteSheetUVAnimPack node = ScriptableObject.CreateInstance<SpriteSheetUVAnimPack>();
            node.name = "SpriteSheet UV Anim Pack";
            node.rect = new Rect(pos.x, pos.y, 342, 350);
            node.CreateInput("UV", "SuperFloat2");
            node.CreateOutput("UV", "SuperFloat2");

            return node;
        }

        protected internal override void NodeGUI()
        {
            Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_displacement.jpg");
            GUI.DrawTexture(new Rect(2, 0, 342, 52), preview);
            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            Outputs[0].DisplayLayout(new GUIContent("UV", "UV"));
            GUILayout.EndHorizontal();
            tex = (Texture2D)EditorGUI.ObjectField(new Rect(8, 78, 130, 130), tex, typeof(Texture2D), true);

            if (GUI.Button(new Rect(276, 0, 60, 30), "Help"))
            {
                Application.OpenURL("https://forum.vetasoft.store/post/116");
            }

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
                NodeEditor._Shadero_Material.SetTexture(FinalVariable4, tex);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable4);
            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");

            if (GUILayout.Button("Reset", GUILayout.Width(190)))
            {
                Variable = 0;
                Variable2 = 0;
                Variable3 = 1;
                Variable4 = 1;
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
            GUILayout.Label("Size: (2 to 16) " + Variable.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable = HorizontalSlider(Mathf.Round(Variable), 2, 16, GUILayout.Width(190), 190);
            Variable = Mathf.Round(Variable);
            total = (int)Variable * (int)Variable;
            total -= 1;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("     ");
            GUILayout.Label("     ");
            GUILayout.Label("Frame: (0 to " + total.ToString() + ") " + Variable2.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable2 = HorizontalSlider((int)Variable2, 0, total, GUILayout.Width(190), 190);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            AddMotion = GUILayout.Toggle(AddMotion, "Fix Side UV", GUILayout.Width(120));
            GUILayout.Label(" ");
            GUILayout.Label("Frame 2: (0 to " + total.ToString() + ") " + Variable3.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ", GUILayout.Width(140));
            Variable3 = HorizontalSlider((int)Variable3, 0, total, GUILayout.Width(190), 190);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ");
            GUILayout.Label("Blend : (0 to 1) " + Variable4.ToString("0.00"), GUILayout.Width(190));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(" ",GUILayout.Width(140));
            Variable4 = HorizontalSlider(Variable4, 0, 1, GUILayout.Width(190), 190);
            GUILayout.EndHorizontal();



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
            SuperFloat2 s_out = new SuperFloat2();

            if (AddMotion) FixUV.tag = true;
            string NodeCount = MemoCount.ToString();
            string DefaultName = "_SpriteSheetUVAnimPack_" + NodeCount;
            string SourceName = "SpriteSheetUVAnimPack_" + NodeCount;
            string DefaultNameVariable1 = "_SpriteSheetUVAnimPack_Size_" + NodeCount;
            string DefaultNameVariable2 = "_SpriteSheetUVAnimPack_Frame1_" + NodeCount;
            string DefaultNameVariable3 = "_SpriteSheetUVAnimPack_Frame2_" + NodeCount;
            string DefaultNameVariable4 = "_SpriteSheetUVAnimPack_Blend_" + NodeCount;
            string DefaultParameters1 = ", Range(2, 16)) = " + Variable.ToString();
            string DefaultParameters2 = ", Range(0, " + total.ToString() + ")) = " + Variable2.ToString();
            string DefaultParameters3 = ", Range(0, " + total.ToString() + ")) = " + Variable3.ToString();
            string DefaultParameters4 = ", Range(0, 1)) = " + Variable4.ToString();
            string uv = s_in.Result;
            string Source = SourceName;

            FinalVariable = DefaultNameVariable1;
            FinalVariable2 = DefaultNameVariable2;
            FinalVariable3 = DefaultNameVariable3;
            FinalVariable4 = SourceName;
            FinalVariable5 = DefaultNameVariable4;



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


            if (AddParameters)
            {
                s_out.ValueLine = "float2 " + DefaultName + " = SpriteSheetUVAnimPack(" + uv + "," + Source + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + ");\n";
            }
            else
            {
                s_out.ValueLine = "float2 " + DefaultName + " = SpriteSheetUVAnimPack(" + uv + "," + Source + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + ");\n";
            }
            if (AddMotion) s_out.ValueLine += DefaultName + " = FixSidesUV(" + DefaultName + ", i.texcoord);\n";

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
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
            }
            s_out.ParametersLines += SourceName + "(\"" + SourceName + "(RGB)\", 2D) = \"white\" { }\n";
            s_out.ParametersDeclarationLines += "sampler2D " + SourceName + ";\n";
            Outputs[0].SetValue<SuperFloat2>(s_out);

            count++;
            return true;
        }
    }
}