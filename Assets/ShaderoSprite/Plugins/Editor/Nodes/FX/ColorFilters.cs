using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Color Filters")]
public class ColorFilters : Node
{
    [HideInInspector]
    public const string ID = "ColorFilters";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 1f;
    [HideInInspector]
    [Multiline(15)]
    public string result;

    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public bool HDR=false;
    [HideInInspector]
    public float HDRvalue=1;


    [HideInInspector]
    public bool parametersOK = true;

    private int Selection = 0;
    
    public static void Init()
    {
        tag = false;
        count = 1;
    }


    public void Function()
    {
       code = "";
       code += "float4 ColorFilters(float4 rgba, float4 red, float4 green, float4 blue, float fade)\n";
       code += "{\n";
       code += "float3 c_r = float3(red.r, red.g, red.b);\n";
       code += "float3 c_g = float3(green.r, green.g, green.b);\n";
       code += "float3 c_b = float3(blue.r, blue.g, blue.b);\n";
       code += "float4 r = float4(dot(rgba.rgb, c_r) + red.a, dot(rgba.rgb, c_g) + green.a, dot(rgba.rgb, c_b) + blue.a, rgba.a);\n";
       code += "return lerp(rgba, saturate(r), fade);\n";
       code += "\n";
       code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        ColorFilters node = ScriptableObject.CreateInstance<ColorFilters>();

        node.name = "Color Filters";

        node.rect = new Rect(pos.x, pos.y, 222, 420);
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_colorfilters.jpg");
        GUI.DrawTexture(new Rect(1, 0, 222, 56), preview);
        GUILayout.Space(58);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

            string[] test = new string[18];
            test[0] = "Blue Lagoon";
            test[1] = "Blue Moon";
            test[2] = "Red White";
            test[3] = "Nash Ville";
            test[4] = "Vintage Yellow";
            test[5] = "Golden Pink";
            test[6] = "Dark Pink";
            test[7] = "Pop Rocket";
            test[8] = "Red Soft Light";
            test[9] = "Yellow Sun Set";
            test[10] = "Walden";
            test[11] = "White Shine";
            test[12] = "Fluo";
            test[13] = "Mars Sun Rise";
            test[14] = "Amelie";
            test[15] = "Blue Jeans";
            test[16] = "Night Vision";
            test[17] = "Blue Paradise";
            Selection = GUILayout.SelectionGrid(Selection, test,2);
          
         if (NodeEditor._Shadero_Material != null)
         {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
         }

        GUILayout.Label("Fade: (0 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 1);
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
    string ConvertColors(float a1, float a2, float a3, float a4, float a5, float a6, float a7, float a8, float a9, float a10, float a11, float a12)
    {
            string Colors = "";
            string CR = "float4(0,0,1,0)";
            string CG = "float4(0,0,1,0)";
            string CB = "float4(0,0,1,0)";

            CR = "float4("+a1/100+","+a2 / 100 + ","+a3 / 100 + ","+a10 / 100 + ")";
            CG = "float4("+a4/100+","+a5 / 100 + ","+a6 / 100 + ","+a11 / 100 + ")";
            CB = "float4("+a7 / 100 + ","+a8 / 100 + ","+a9 / 100 + ","+a12 / 100 + ")";
            Colors = CR + "," + CG + "," + CB;
            return Colors;
        }
    public override bool Calculate()
    {
        tag = true;

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_ColorFilters_" + NodeCount;
        string DefaultNameVariable1 = "_ColorFilters_Fade_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
            string PreviewVariable = s_in.Result;

            FinalVariable = DefaultNameVariable1;


            if (s_in.Result == null)
            {
                PreviewVariable = "float4(0,0,0,1)";
            }


            s_out.StringPreviewLines = s_in.StringPreviewNew ;
      
            string Colors = "";
           
            if (Selection == 0) Colors = ConvertColors(100, 102, 0, 18, 100, 4, 28, -26, 100, -64, 0, 12);
            if (Selection == 1) Colors = ConvertColors(200, 98, -116, 24, 100, 2, 30, 52, 20, -48, -20, 12);
            if (Selection == 2) Colors = ConvertColors(-42, 68, 108, -96, 100, 116, -92, 104, 96, 0, 2, 4);
            if (Selection == 3) Colors = ConvertColors(130, 8, 7, 19, 89, 3, -1, 11, 57, 10, 19, 47);
            if (Selection == 4) Colors = ConvertColors(200, 109, -104, 42, 126, -1, -40, 121, -31, -48, -20, 12);
            if (Selection == 5) Colors = ConvertColors(70, 200, 0, 0, 100, 8, 6, 12, 110, 0, 0, -6);
            if (Selection == 6) Colors = ConvertColors(60, 112, 36, 24, 100, 2, 0, -26, 100, -56, -20, 12);
            if (Selection == 7) Colors = ConvertColors(100, 6, -17, 0, 107, 0, 10, 21, 100, 40, 0, 8);
            if (Selection == 8) Colors = ConvertColors(-4, 200, -30, -58, 200, -30, -58, 200, -30, -11, 0, 0);
            if (Selection == 9) Colors = ConvertColors(117, -6, 53, -68, 135, 19, -146, -61, 200, 0, 0, 0);
            if (Selection == 10) Colors = ConvertColors(99, 2, 13, 100, 1, 40, 50, 8, 71, 0, 2, 7);
            if (Selection == 11) Colors = ConvertColors(190, 24, -33, 2, 200, -6, -10, 27, 200, -6, -13, 15);
            if (Selection == 12) Colors = ConvertColors(100, 0, 0, 0, 113, 0, 200, -200, -200, 0, 0, 36);
            if (Selection == 13) Colors = ConvertColors(50, 141, -81, -17, 62, 29, -159, -137, -200, 7, -34, -6);
            if (Selection == 14) Colors = ConvertColors(100, 11, 39, 1, 63, 53, -24, 71, 20, -25, -10, -24);
            if (Selection == 15) Colors = ConvertColors(181, 11, 15, 40, 40, 20, 40, 40, 20, -59, 0, 0);
            if (Selection == 16) Colors = ConvertColors(200, -200, -200, 195, 4, -160, 200, -200, -200, -200, 10, -200);
            if (Selection == 17) Colors = ConvertColors(66, 200, -200, 25, 38, 36, 30, 150, 114, 17, 0, 65);




            if (parametersOK)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = ColorFilters(" + PreviewVariable + "," + Colors + "," + DefaultNameVariable1 + ");\n";

            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = ColorFilters(" + PreviewVariable + "," + Colors + "," + Variable.ToString() + ");\n";
            }

            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines;

            s_out.Result = DefaultName;

            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines ;

            if (parametersOK)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            }

            Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
}
}