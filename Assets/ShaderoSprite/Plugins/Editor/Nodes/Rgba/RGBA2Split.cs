using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Channels/RGBA 2 Channels")]
public class RGBA2Split : Node
{
    [HideInInspector]
    public const string ID = "RGBA2Split";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 1;
     public static int count = 1;
    public static bool tag = false;
    public static string code;

    public static void Init()
    {
        tag = false;
        count = 1;
    }
    public override Node Create(Vector2 pos)
    {
        RGBA2Split node = ScriptableObject.CreateInstance<RGBA2Split>();

        node.name = "RGBA\n2 Channels";
        node.rect = new Rect(pos.x, pos.y, 130, 140);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("R", "SuperFloat");
        node.CreateOutput("G", "SuperFloat");
        node.CreateOutput("B", "SuperFloat");
        node.CreateOutput("A", "SuperFloat");
   
        return node;
    }

    protected internal override void NodeGUI()
    {

        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("R", "R"));
        GUILayout.EndHorizontal();
        Outputs[1].DisplayLayout(new GUIContent("G", "G"));
        Outputs[2].DisplayLayout(new GUIContent("B", "B"));
        Outputs[3].DisplayLayout(new GUIContent("Alpha", "Alpha"));
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

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat s_outR = new SuperFloat();
        SuperFloat s_outG = new SuperFloat();
        SuperFloat s_outB = new SuperFloat();
        SuperFloat s_outAlpha = new SuperFloat();

        string RGBA = s_in.Result;
        if (RGBA == null) RGBA = "float4(1,1,1,1)";

        string NodeCount = MemoCount.ToString();
        string UsedVariable = "RGBASplit_" + NodeCount;
       
        s_outR.StringPreviewLines = s_in.StringPreviewNew;
        s_outR.ValueLine = "float4 " + UsedVariable + " = " + RGBA + ";\n";
        s_outR.StringPreviewNew = s_outR.StringPreviewLines + s_outR.ValueLine;
        s_outR.ParametersLines += s_in.ParametersLines;
        s_outR.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        s_outG.StringPreviewLines = s_in.StringPreviewNew;
        s_outG.ValueLine = "float4 " + UsedVariable + " = " + RGBA + ";\n";
        s_outG.StringPreviewNew = s_outG.StringPreviewLines + s_outG.ValueLine;
        s_outG.ParametersLines += s_in.ParametersLines;
        s_outG.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        s_outB.StringPreviewLines = s_in.StringPreviewNew;
        s_outB.ValueLine = "float4 " + UsedVariable + " = " + RGBA + ";\n";
        s_outB.StringPreviewNew = s_outB.StringPreviewLines + s_outB.ValueLine;
        s_outB.ParametersLines += s_in.ParametersLines;
        s_outB.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        s_outAlpha.StringPreviewLines = s_in.StringPreviewNew;
        s_outAlpha.ValueLine = "float4 " + UsedVariable + " = " + RGBA + ";\n";
        s_outAlpha.StringPreviewNew = s_outAlpha.StringPreviewLines + s_outAlpha.ValueLine;
        s_outAlpha.ParametersLines += s_in.ParametersLines;
        s_outAlpha.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        s_outR.Result = UsedVariable + ".r";
        s_outG.Result = UsedVariable + ".g";
        s_outB.Result = UsedVariable + ".b";
        s_outAlpha.Result = UsedVariable + ".a";
      


        Outputs[0].SetValue<SuperFloat>(s_outR);
        Outputs[1].SetValue<SuperFloat>(s_outG);
        Outputs[2].SetValue<SuperFloat>(s_outB);
        Outputs[3].SetValue<SuperFloat>(s_outAlpha);
        count++;

        return true;
    }
}
}