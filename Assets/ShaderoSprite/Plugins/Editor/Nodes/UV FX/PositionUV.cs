using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Position UV")]

public class PositionUV : Node
{
    public const string ID = "PositionUV";
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 0;
    [HideInInspector] public float Variable2 = 0;
    [HideInInspector] public float Variable3 = 1;
    [HideInInspector] public float Variable4 = 1;
     [HideInInspector] public bool ActiveClamp = false;
    [HideInInspector]
    public bool AddParameters = true;

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
        code += "float2 PositionUV(float2 uv, float offsetx, float offsety)\n";
        code += "{\n";
        code += "uv += float2(offsetx, offsety);\n";
        code += "return uv;\n";
        code += "}\n";
        code += "\n";
    }
   
   
    public override Node Create(Vector2 pos)
    {
        Function();
        PositionUV node = ScriptableObject.CreateInstance<PositionUV>();

        node.name = "Position UV";
        node.rect = new Rect(pos.x, pos.y, 172, 250);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_positionuv.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
        GUILayout.EndHorizontal();
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
          }
        if (GUILayout.Button("Reset"))
        {
            Variable = 0;
            Variable2 = 0;
      
        }
        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        GUILayout.Label("Pos X (-2 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -2, 2);
        GUILayout.Label("Pos Y (-2 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -2, 2);

  
        
    }

    private string FinalVariable1;
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
        SuperFloat2 s_out = new SuperFloat2();


        string NodeCount = MemoCount.ToString();
        string DefaultName = "PositionUV_";
        string DefaultNameVariable1 = DefaultName+"X_" + NodeCount;
        string DefaultNameVariable2 = DefaultName+"Y_" + NodeCount;
         DefaultName = DefaultName + NodeCount;
        string DefaultParameters1 = ", Range(-2, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-2, 2)) = " + Variable2.ToString(); 
        string VoidName = "PositionUV";
        string PreviewVariable = s_in.Result;

        if (PreviewVariable == null) PreviewVariable = "i.texcoord";


      
        FinalVariable1 = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
      
        // Ajoute l'ancienne ligne complete du input dans celui du output ( il n'y a pas encore la nouvelle ligne )
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (AddParameters)
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
            + ","
            + DefaultNameVariable1 + ","
            + DefaultNameVariable2 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
                + ","
                + Variable.ToString() + ","
                + Variable2.ToString() + ");\n";
        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
         }


        Outputs[0].SetValue<SuperFloat2>(s_out);

        count++;

        return true;
    }
}
}