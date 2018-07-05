using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Alpha/Force RGBA Alpha from RGBA")]
public class RGBAAlpha2 : Node
{
    [HideInInspector]
    public const string ID = "RGBAAlpha2";
    [HideInInspector]
    public override string GetID { get { return ID; } }

    [HideInInspector] public float Variable = 1;



    public override Node Create(Vector2 pos)
    {
        RGBAAlpha2 node = ScriptableObject.CreateInstance<RGBAAlpha2>();

        node.name = "Force RGBA Alpha from RGBA";
        node.rect = new Rect(pos.x, pos.y, 180, 120);

        node.CreateInput("RGBA(Alpha)", "SuperFloat4");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
         Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Inputs[0].DisplayLayout(new GUIContent("RGBA(Alpha)", "RGBA(Alpha)"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
    }
    private string FinalVariable;
    public override bool Calculate()
    {
   
        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
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
        s_out.ValueLine = RGBA + ".a = "+ Alpha + ".a;\n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines+ s_in2.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines+ s_in2.ParametersDeclarationLines;
        
        Outputs[0].SetValue<SuperFloat4>(s_out);
        return true;
    }
}
}
