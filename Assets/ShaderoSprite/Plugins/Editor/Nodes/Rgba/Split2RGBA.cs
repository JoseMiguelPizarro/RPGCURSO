using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Channels/Channels 2 RGBA")]
public class Split2RGBA : Node
{
    [HideInInspector]
    public const string ID = "Split2RGBA";
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
        Split2RGBA node = ScriptableObject.CreateInstance<Split2RGBA>();

        node.name = "Channels\n2 RGBA";
        node.rect = new Rect(pos.x, pos.y, 130, 140);

        node.CreateInput("R", "SuperFloat");
        node.CreateInput("G", "SuperFloat");
        node.CreateInput("B", "SuperFloat");
        node.CreateInput("A", "SuperFloat");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("R", "R"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("G", "G"));
        Inputs[2].DisplayLayout(new GUIContent("B", "B"));
        Inputs[3].DisplayLayout(new GUIContent("Alpha", "Alpha"));
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

        SuperFloat s_in = Inputs[0].GetValue<SuperFloat>();
        SuperFloat s_in2 = Inputs[1].GetValue<SuperFloat>();
        SuperFloat s_in3 = Inputs[2].GetValue<SuperFloat>();
        SuperFloat s_in4 = Inputs[3].GetValue<SuperFloat>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string UsedVariable = "SplitRGBA_" + NodeCount;

      
        string R = s_in.Result;
        string G = s_in2.Result;
        string B = s_in3.Result;
        string Alpha = s_in4.Result;

        if (s_in.Result == null) R = "0";
        if (s_in2.Result == null) G = "0";
        if (s_in3.Result == null) B = "0";
        if (s_in4.Result == null) Alpha = "1";

   
        s_out.StringPreviewLines = s_in.StringPreviewNew+ s_in2.StringPreviewNew+ s_in3.StringPreviewNew+ s_in4.StringPreviewNew;
        s_out.ValueLine = "float4 "+UsedVariable + " = float4(" + R + "," + G + "," + B + "," + Alpha +");\n";
        
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = UsedVariable;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines + s_in3.ParametersLines + s_in4.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines + s_in3.ParametersDeclarationLines + s_in4.ParametersDeclarationLines;

        Outputs[0].SetValue<SuperFloat4>(s_out);
        
        count++;
        return true;
    }
}
}