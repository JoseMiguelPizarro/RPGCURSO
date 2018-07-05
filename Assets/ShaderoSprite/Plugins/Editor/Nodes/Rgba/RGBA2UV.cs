using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/UV (RGBA)/RGBA to UV")]
public class RGBA2UV : Node
{
    [HideInInspector] public const string ID = "RGBA2UV";
    [HideInInspector] public override string GetID { get { return ID; } }

   
    [HideInInspector] public float Variable = 1;
   
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
        RGBA2UV node = ScriptableObject.CreateInstance<RGBA2UV>();
        node.name = "RGBA to UV";
        node.rect = new Rect(pos.x, pos.y, 150, 80);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("UV", "SuperFloat2");
     
        return node;
    }

 

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        GUILayout.EndHorizontal();
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
        SuperFloat2 s_out = new SuperFloat2();
        
        
        string NodeCount = MemoCount.ToString();
        string DefaultName = "rgba_uv_" + NodeCount;
        string PreviewVariable = s_in.Result;

 
        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = "float2 " + DefaultName + " = float2(" + PreviewVariable + ".r,"+ PreviewVariable + ".g);\n";
     
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
    
        Outputs[0].SetValue<SuperFloat2>(s_out);

        count++;
        return true;
    }
}
}