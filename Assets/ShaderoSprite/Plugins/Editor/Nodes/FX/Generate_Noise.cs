using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Shape/Noise")]
public class Generate_Noise : Node
{
    [HideInInspector]
    public const string ID = "Generate_Noise";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 4f;
    
    [HideInInspector]
    public bool AddParameters = true;
    [HideInInspector]
    public bool NoAlphaBlack = false;
 
    public static int count = 1;
    public static bool tag = false;
    public static bool ForceHorizontal = false;
    public static bool Inverse = false;
    public static string code;


    public static void Init()
    {
        tag = false;
        count = 1;
    }



    public void Function()
    {
        code = "";
        code += "float4 Generate_Noise(float2 co, float black)\n";
        code += "{\n";
        code += "float4 r = frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);\n";
        code += "r.a = saturate(r.a + black);\n";
        code += "return r;\n";
        code += "}\n";



    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Noise node = ScriptableObject.CreateInstance<Generate_Noise>();
        node.name = "Generate RGBA Noise";
        node.rect = new Rect(pos.x, pos.y, 172, 150);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_noise.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");



    }
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
        string DefaultName = "_Generate_Line_" + NodeCount;
         string uv = s_in.Result;

       

        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
        }
        else
        {
            uv = s_in.Result;
        }
        string alphablack = "0";
        if (NoAlphaBlack) alphablack = "1";
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (AddParameters)
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Noise(" + uv + "," + alphablack + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Noise(" + uv + "," + alphablack + ");\n";

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
       
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}