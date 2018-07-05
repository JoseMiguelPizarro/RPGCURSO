using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Shape/Donuts")]
public class Generate_Donuts : Node
{
    [HideInInspector]
    public const string ID = "Generate_Donuts";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0.5f;
    [HideInInspector]
    public float Variable2 = 0.5f;
    [HideInInspector]
    public float Variable3 = 0.1f;
    [HideInInspector]
    public float Variable4 = 0.2f;
    [HideInInspector]
    public float Variable5 = 0.2f;


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
        code += "float4 Generate_Donut(float2 uv, float posx, float posy, float size, float sizedonut, float smooth, float black)\n";
        code += "{\n";
        code += "uv -= float2(posx, posy);\n";
        code += "float l = length(uv*2);\n";
        code += "float4 d = smoothstep(size, size + smooth, l);\n";
        code += "d *= smoothstep(size+sizedonut, size + sizedonut + smooth, 1-l);\n";
        code += "d.a = saturate(d + black);\n";
        code += "return d;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Donuts node = ScriptableObject.CreateInstance<Generate_Donuts>();
        node.name = "Generate RGBA Donut";
        node.rect = new Rect(pos.x, pos.y, 172, 400);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_donut.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Reset"))
        {
            Variable = 0.5f;
            Variable2 = 0.5f;
            Variable3 = 0.1f;
            Variable4 = 0.2f;
            Variable5 = 0.2f;

        }

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
        }


        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        GUILayout.Label("Pos X (-1 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -1, 2);

        GUILayout.Label("Pos Y (-1 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -1, 2);
      
        GUILayout.Label("Size (-2 to 2) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, -2, 2);

        GUILayout.Label("Size Donut (-2 to 2)" + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, -2, 2);

        GUILayout.Label("Smooth (0 to 1) " + Variable5.ToString("0.00"));
        Variable5 =HorizontalSlider(Variable5, 0f, 1f);

        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");



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
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Generate_Donut_" + NodeCount;
        string DefaultNameVariable1 = "_Generate_Donut_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_Generate_Donut_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_Generate_Donut_Size_" + NodeCount;
        string DefaultNameVariable4 = "_Generate_Donut_SizeDonut_" + NodeCount;
        string DefaultNameVariable5 = "_Generate_Donut_SizeSmooth_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 2)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(-2, 2)) = " + Variable3.ToString();
        string DefaultParameters4 = ", Range(-2, 2)) = " + Variable4.ToString();
        string DefaultParameters5 = ", Range(0, 1)) = " + Variable5.ToString();
        string uv = s_in.Result;

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
        string alphablack = "0";
        if (NoAlphaBlack) alphablack = "1";
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (AddParameters)
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Donut(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + "," + alphablack + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Donut(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + "," + alphablack + ");\n";

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

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}