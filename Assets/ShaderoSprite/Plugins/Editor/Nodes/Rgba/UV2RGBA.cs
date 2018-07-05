using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/UV (RGBA)/UV to RGBA")]
public class UV2RGBA : Node
{
    [HideInInspector] public const string ID = "UV2RGBA";
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
        UV2RGBA node = ScriptableObject.CreateInstance<UV2RGBA>();
        node.name = "UV to RGBA";
        node.rect = new Rect(pos.x, pos.y, 150, 80);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");
     
        return node;
    }

 

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
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
        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat4 s_out = new SuperFloat4();
        
        
        string NodeCount = MemoCount.ToString();
        string DefaultName = "uv_rgba_" + NodeCount;
         string PreviewVariable = s_in.Result;

 
        if (s_in.Result == null)
        {
            PreviewVariable = "i.texcoord";
        }
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = "float4 "+DefaultName + " =  float4(" + PreviewVariable + ".r,"+ PreviewVariable + ".g,0,1);\n";
     
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
    
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}