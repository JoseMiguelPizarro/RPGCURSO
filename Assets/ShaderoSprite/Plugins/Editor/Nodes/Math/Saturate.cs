using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Math/Saturate")]
public class Saturate : Node
{
    [HideInInspector] public const string ID = "Saturate";
    [HideInInspector] public override string GetID { get { return ID; } }

   
    public override Node Create(Vector2 pos)
    {
        Saturate node = ScriptableObject.CreateInstance<Saturate>();

        node.name = "Saturate";
        node.rect = new Rect(pos.x, pos.y, 120, 80);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");
     
        return node;
    }

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
    }

    public override bool Calculate()
    {
    
        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();
        
        string PreviewVariable = s_in.Result;

        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = PreviewVariable + " = saturate(" + PreviewVariable + "); \n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = s_in.Result;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        Outputs[0].SetValue<SuperFloat4>(s_out);

        return true;
    }
}
}