using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Flip Horizontal UV")]
public class FlipUV_H : Node
{
    [HideInInspector] public const string ID = "FlipUV_H";
    [HideInInspector] public override string GetID { get { return ID; } }

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
        code += "float2 FlipUV_H(float2 uv)\n";
        code += "{\n";
        code += "uv.x = 1 - uv.x;\n";
        code += "return uv;\n";
        code += "}\n";
    }
    public override Node Create(Vector2 pos)
    {
        Function();
        FlipUV_H node = ScriptableObject.CreateInstance<FlipUV_H>();

        node.name = "FlipUV_H";
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
        tag = true;

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat2 s_out = new SuperFloat2();

        string PreviewVariable = s_in.Result;
        string uv = PreviewVariable;
        if (s_in.Result == null) uv = "i.texcoord";
        
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = "";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = "FlipUV_H(" + uv + ")";
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        Outputs[0].SetValue<SuperFloat2>(s_out);

        return true;
    }
}
}