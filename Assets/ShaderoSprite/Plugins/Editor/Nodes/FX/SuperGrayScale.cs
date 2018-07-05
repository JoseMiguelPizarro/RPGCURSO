using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Super Gray Scale")]
public class SuperGrayScale : Node
{
    [HideInInspector]
    public const string ID = "SuperGrayScale";
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
       code += "float4 SuperGrayScale(float4 rgba, float4 red, float4 green, float4 blue, float fade)\n";
       code += "{\n";
       code += "float3 c_r = float3(red.r, red.g, red.b);\n";
       code += "float3 c_g = float3(green.r, green.g, green.b);\n";
       code += "float3 c_b = float3(blue.r, blue.g, blue.b);\n";
       code += "float4 r = float4(dot(rgba.rgb, c_r) + red.a, dot(rgba.rgb, c_g) + green.a, dot(rgba.rgb, c_b) + blue.a, rgba.a);\n";
       code += "return lerp(rgba, r, fade);\n";
       code += "\n";
       code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        SuperGrayScale node = ScriptableObject.CreateInstance<SuperGrayScale>();

        node.name = "Super Gray Scale";

        node.rect = new Rect(pos.x, pos.y, 172, 330);
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_supergrayscale.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

            GUILayout.Label("Black & White");
            string[] test = new string[5];
            test[0] = "by Blue";
            test[1] = "by Green";
            test[2] = "by Orange";
            test[3] = "by Red";
            test[4] = "by Yellow";
            Selection = GUILayout.SelectionGrid(Selection, test,1);
          
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

    public override bool Calculate()
    {
        tag = true;

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_SuperGrayScale_" + NodeCount;
        string DefaultNameVariable1 = "_SuperGrayScale_Fade_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
             string PreviewVariable = s_in.Result;

            FinalVariable = DefaultNameVariable1;


            if (s_in.Result == null)
            {
                PreviewVariable = "float4(0,0,0,1)";
            }


            s_out.StringPreviewLines = s_in.StringPreviewNew ;
     
            string Colors = "";
            string CR = "float4(0,0,1,0)";
            string CG = "float4(0,0,1,0)";
            string CB = "float4(0,0,1,0)";

            if (Selection==0)
            {
                CR = "float4(0,0,1,0)";
                CG = "float4(0,0,1,0)";
                CB = "float4(0,0,1,0)";
            }
            if (Selection == 1)
            {
                CR = "float4(0,1,0,0)";
                CG = "float4(0,1,0,0)";
                CB = "float4(0,1,0,0)";
            }
            if (Selection == 2)
            {
                CR = "float4(0.5,0.5,0,0)";
                CG = "float4(0.5,0.5,0,0)";
                CB = "float4(0.5,0.5,0,0)";
            }
            if (Selection == 3)
            {
                CR = "float4(1,0,0,0)";
                CG = "float4(1,0,0,0)";
                CB = "float4(1,0,0,0)";
            }
            if (Selection == 4)
            {
                CR = "float4(0.34,0.66,0,0)";
                CG = "float4(0.34,0.66,0,0)";
                CB = "float4(0.34,0.66,0,0)";
            }

            Colors = CR + "," + CG + "," + CB;

         

            if (parametersOK)
            {
                s_out.ValueLine = "float4 " + DefaultName + " = SuperGrayScale(" + PreviewVariable + "," + Colors + "," + DefaultNameVariable1 + ");\n";

            }
            else
            {
                s_out.ValueLine = "float4 " + DefaultName + " = SuperGrayScale(" + PreviewVariable + "," + Colors + "," + Variable.ToString() + ");\n";
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