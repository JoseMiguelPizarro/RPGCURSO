using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Shape/Shape Side")]
public class Generate_Shape : Node
{
    [HideInInspector]
    public const string ID = "Generate_Shape";
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
    public float Variable5 = 3f;
    [HideInInspector]
    public float Variable6 = 0f;


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
        code += "float4 Generate_Shape(float2 uv, float posX, float posY, float Size, float Smooth, float number, float black, float rot)\n";
        code += "{\n";
        code += "uv = uv - float2(posX, posY);\n";
        code += "float angle = rot * 0.01744444;\n";
        code += "float a = atan2(uv.x, uv.y) +angle, r = 6.28318530718 / int(number);\n";
        code += "float d = cos(floor(0.5 + a / r) * r - a) * length(uv);\n";
        code += "float dist = 1.0 - smoothstep(Size, Size + Smooth, d);\n";
        code += "float4 result = float4(1, 1, 1, dist);\n";
        code += "if (black == 1) result = float4(dist, dist, dist, 1);\n";
        code += "return result;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Shape node = ScriptableObject.CreateInstance<Generate_Shape>();
        node.name = "Generate RGBA Shape";
        node.rect = new Rect(pos.x, pos.y, 172, 450);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");
   
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_shape.jpg");
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
            Variable3 = 0.40f;
            Variable4 = 0.01f;
            Variable5 = 3f;
            Variable6 = 0f;

        }

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

      
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable6, Variable6);
        }

        GUILayout.Label("Pos X (-1 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -1, 2);
        GUILayout.Label("Pos Y (-1 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -1, 2);

        GUILayout.Label("Size (-1 to 1) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, -1, 1);

        GUILayout.Label("Smooth (0 to 1) " + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, 0, 1);

        GUILayout.Label("Side (1 to 32) " + Variable5.ToString("0.00"));
        Variable5 = Mathf.RoundToInt(GUILayout.HorizontalSlider(Variable5, 1, 32));
       
        GUILayout.Label("Rotation (-360 to 360) " + Variable6.ToString("0.00"));
        Variable6 = Mathf.RoundToInt(GUILayout.HorizontalSlider(Variable6, -360, 360));
       
        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");
       


    }
    private string FinalVariable;
    private string FinalVariable2;
    private string FinalVariable3;
    private string FinalVariable4;
    private string FinalVariable5;
    private string FinalVariable6;
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
        string DefaultName = "_Generate_Shape_" + NodeCount;
        string DefaultNameVariable1 = "_Generate_Shape_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_Generate_Shape_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_Generate_Shape_Size_" + NodeCount;
        string DefaultNameVariable4 = "_Generate_Shape_Dist_" + NodeCount;
        string DefaultNameVariable5 = "_Generate_Shape_Side_" + NodeCount;
        string DefaultNameVariable6 = "_Generate_Shape_Rotation_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 2)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(-1, 1)) = " + Variable3.ToString();
        string DefaultParameters4 = ", Range(0, 1)) = " + Variable4.ToString();
        string DefaultParameters5 = ", Range(1, 32)) = " + Variable5.ToString();
        string DefaultParameters6 = ", Range(-360, 360)) = " + Variable6.ToString();
        string uv = s_in.Result;
       
        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;
        FinalVariable4 = DefaultNameVariable4;
        FinalVariable5 = DefaultNameVariable5;
        FinalVariable6 = DefaultNameVariable6;



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
        s_out.StringPreviewLines = s_in.StringPreviewNew ;
        if (AddParameters)
        s_out.ValueLine = "float4 " + DefaultName + " = Generate_Shape(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + ","+ DefaultNameVariable5 + "," + alphablack + ","+ DefaultNameVariable6 + ");\n";
        else
        s_out.ValueLine = "float4 " + DefaultName + " = Generate_Shape(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + "," + alphablack + ","+ Variable6.ToString() + ");\n";

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
        s_out.ParametersLines += DefaultNameVariable6 + "(\"" + DefaultNameVariable6 + "\"" + DefaultParameters6 + "\n";

        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable6 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}