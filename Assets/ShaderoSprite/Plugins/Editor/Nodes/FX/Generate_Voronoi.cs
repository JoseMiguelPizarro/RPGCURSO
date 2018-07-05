using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Effects/Voronoi")]
public class Generate_Voronoi : Node
{
    [HideInInspector]
    public const string ID = "Generate_Voronoi";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 8f;
    [HideInInspector]
    public float Variable2 = 1f;
  

    [HideInInspector]
    public bool AddParameters = true;
    [HideInInspector]
    public bool NoAlphaBlack = false;

    
    public static int count = 1;
    public static bool tag = false;
    public static string code;


    public static void Init()
    {
        tag = false;
        count = 1;
    }



    public void Function()
    {
        code = "";
        code += "float3 Generate_Voronoi_hash3(float2 p, float seed)\n";
        code += "{\n";
        code += "float3 q = float3(dot(p, float2(127.1, 311.7)),\n";
        code += "dot(p, float2(269.5, 183.3)),\n";
        code += "dot(p, float2(419.2, 371.9)));\n";
        code += "return frac(sin(q) * 43758.5453 * seed);\n";
        code += "}\n";
        code += "float4 Generate_Voronoi(float2 uv, float size,float seed, float black)\n";
        code += "{\n";
        code += "float2 p = floor(uv*size);\n";
        code += "float2 f = frac(uv*size);\n";
        code += "float k = 1.0 + 63.0*pow(1.0, 4.0);\n";
        code += "float va = 0.0;\n";
        code += "float wt = 0.0;\n";
        code += "for (int j = -2; j <= 2; j++)\n";
        code += "{\n";
        code += "for (int i = -2; i <= 2; i++)\n";
        code += "{\n";
        code += "float2 g = float2(float(i), float(j));\n";
        code += "float3 o = Generate_Voronoi_hash3(p + g, seed)*float3(1.0, 1.0, 1.0);\n";
        code += "float2 r = g - f + o.xy;\n";
        code += "float d = dot(r, r);\n";
        code += "float ww = pow(1.0 - smoothstep(0.0, 1.414, sqrt(d)), k);\n";
        code += "va += o.z*ww;\n";
        code += "wt += ww;\n";
        code += "}\n";
        code += "}\n";
        code += "float4 result = saturate(va / wt);\n";
        code += "result.a = saturate(result.a + black);\n";
        code += "return result;\n";
        code += "}\n";

    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Voronoi node = ScriptableObject.CreateInstance<Generate_Voronoi>();
        node.name = "Generate Voronoi";
        node.rect = new Rect(pos.x, pos.y, 172, 270);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_voronoi.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Reset"))
        {
            Variable = 8f;
            Variable2 = 1f;
   
        }

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);

         }
        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
        GUILayout.Label("Size (0 to 128) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 128);

        //-----

        GUILayout.Label("Seed (1 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, 1, 2);
        //-----

     


        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");



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
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Generate_Voronoi_" + NodeCount;
        string DefaultNameVariable1 = "_Generate_Voronoi_Size_" + NodeCount;
        string DefaultNameVariable2 = "_Generate_Voronoi_Seed_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 128)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(1, 2)) = " + Variable2.ToString();
          string uv = s_in.Result;

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
        string alphablack = "0";
        if (NoAlphaBlack) alphablack = "1";
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (AddParameters)
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Voronoi(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + alphablack + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Voronoi(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + alphablack + ");\n";

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (AddParameters)
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