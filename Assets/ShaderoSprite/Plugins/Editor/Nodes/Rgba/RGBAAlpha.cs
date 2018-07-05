using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Alpha/Force RGBA Alpha from Alpha")]
public class RGBAAlpha : Node
{
    [HideInInspector]
    public const string ID = "RGBAAlpha";
    [HideInInspector]
    public override string GetID { get { return ID; } }

     [HideInInspector] public float Variable = 1;
   
     public override Node Create(Vector2 pos)
    {
        RGBAAlpha node = ScriptableObject.CreateInstance<RGBAAlpha>();

        node.name = "Force RGBA Alpha from Alpha";
        node.rect = new Rect(pos.x, pos.y, 180, 120);

        node.CreateInput("Alpha", "SuperFloat");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
         Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Inputs[0].DisplayLayout(new GUIContent("Alpha ", "Alpha"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
    }
    private string FinalVariable;
    public override bool Calculate()
    {
   
        SuperFloat s_in = Inputs[0].GetValue<SuperFloat>();
        SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string DefaultName = s_in.Result;
        string Alpha = s_in.Result;
        string RGBA = s_in2.Result;
        DefaultName = RGBA;

        if (s_in.Result == null)
        {
            Alpha = "1";
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;
        s_out.ValueLine = RGBA + ".a = "+ Alpha + ";\n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines+ s_in2.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines+ s_in2.ParametersDeclarationLines;
        
        Outputs[0].SetValue<SuperFloat4>(s_out);
        return true;
    }
}
}
