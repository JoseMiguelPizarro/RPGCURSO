using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Shape/Circle")]
public class Generate_Circle : Node
{
    [HideInInspector]
    public const string ID = "Generate_Circle";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0.5f;
    [HideInInspector]
    public float Variable2 = 0.5f;
    [HideInInspector]
    public float Variable3 = 0.4f;
    [HideInInspector]
    public float Variable4 = 0.01f;

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
        code += "float4 Generate_Circle(float2 uv, float posX, float posY, float Size, float Smooth, float black)\n";
        code += "{\n";
        code += "float2 center = float2(posX, posY);\n";
        code += "float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));\n";
        code += "float4 result = float4(1,1,1,dist);\n";
        code += "if (black == 1) result = float4(dist, dist, dist, 1);\n";
        code += "return result;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Circle node = ScriptableObject.CreateInstance<Generate_Circle>();
        node.name = "Generate RGBA Circle";
        node.rect = new Rect(pos.x, pos.y, 172, 380);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_circle.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);

        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            Variable = 0.5f;
            Variable2 = 0.5f;
            Variable3 = 0.40f;
            Variable4 = 0.01f;

           
        }
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
        }
        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
        }

        GUILayout.Label("Pos X (-1 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -1, 2);

        GUILayout.Label("Pos Y (-1 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -1, 2);
     
        GUILayout.Label("Size (-1 to 1) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, -1, 1);

        GUILayout.Label("Smooth (0 to 1) " + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, 0, 1);
        //-----
        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");
       


    }
    private string FinalVariable;
    private string FinalVariable2;
    private string FinalVariable3;
    private string FinalVariable4;

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
        string DefaultName = "_Generate_Circle_" + NodeCount;
        string DefaultNameVariable1 = "_Generate_Circle_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_Generate_Circle_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_Generate_Circle_Size_" + NodeCount;
        string DefaultNameVariable4 = "_Generate_Circle_Dist_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 2)) = "+ Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 2)) = "+ Variable2.ToString();
        string DefaultParameters3 = ", Range(-1, 1)) = "+ Variable3.ToString();
        string DefaultParameters4 = ", Range(0, 1)) = "+ Variable4.ToString();
        string uv = s_in.Result;

        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;
        FinalVariable4 = DefaultNameVariable4;



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
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Circle(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + alphablack + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Circle(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + alphablack + ");\n";

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
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}