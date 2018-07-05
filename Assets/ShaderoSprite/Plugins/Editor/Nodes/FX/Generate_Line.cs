using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Shape/Lines")]
public class Generate_Line : Node
{
    [HideInInspector]
    public const string ID = "Generate_Line";
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
        code += "float4 Generate_Lines(float uvs, float size, float black)\n";
        code += "{\n";
        code += "float4 r = step(0.5 / size, fmod(uvs, 1 / size));\n";
        code += "r.a = saturate(r.a + black);\n";
        code += "return r;\n";
        code += "}\n";


    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Line node = ScriptableObject.CreateInstance<Generate_Line>();
        node.name = "Generate RGBA Lines";
        node.rect = new Rect(pos.x, pos.y, 172, 260);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_lines.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Reset"))
        {
            Variable = 4f;
        }

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
          }


        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        ForceHorizontal = GUILayout.Toggle(ForceHorizontal, " Force Horizontal");
        Inverse = GUILayout.Toggle(Inverse, " Inverse");

        GUILayout.Label("Line Number: " + Variable.ToString("0.00"));
        Variable = Mathf.RoundToInt(GUILayout.HorizontalSlider(Variable, 1, 64));
   
    
        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");



    }
    private string FinalVariable;

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
        string DefaultNameVariable1 = "_Generate_Line_Size_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 64)) = " + Variable.ToString();
        string uv = s_in.Result;

        FinalVariable = DefaultNameVariable1;
      

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

        string tempvar = Variable.ToString();

        string tempvar2 = "";
        if (Inverse) tempvar2 = "1-";

        if (AddParameters) tempvar = DefaultNameVariable1;
        
        if (ForceHorizontal) 
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Lines(" + tempvar2 + uv + ".y," + tempvar + "," + alphablack + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Lines(" + tempvar2 + uv + ".x," + tempvar + "," + alphablack + ");\n";

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (AddParameters)
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