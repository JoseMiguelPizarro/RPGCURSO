using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/Math (UV)/Saturate UV")]
public class SaturateUV : Node
{
    [HideInInspector] public const string ID = "SaturateUV";
    [HideInInspector] public override string GetID { get { return ID; } }
     public override Node Create(Vector2 pos)
    {
        SaturateUV node = ScriptableObject.CreateInstance<SaturateUV>();

        node.name = "SaturateUV";
        node.rect = new Rect(pos.x, pos.y, 120, 80);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
     
        return node;
    }

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        GUILayout.EndHorizontal();
   }

    public override bool Calculate()
    {
    
        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat2 s_out = new SuperFloat2();
        
        string PreviewVariable = s_in.Result;

        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = PreviewVariable + " = saturate(" + PreviewVariable + "); \n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = s_in.Result;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        Outputs[0].SetValue<SuperFloat2>(s_out);

        return true;
    }
}
}
