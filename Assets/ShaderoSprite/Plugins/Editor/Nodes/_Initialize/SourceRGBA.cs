using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Source/to RGBA")]
public class SourceRGBA : Node
{
    [HideInInspector] public const string ID = "SourceRGBA";
    [HideInInspector] public override string GetID { get { return ID; } }

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

        SourceRGBA node = ScriptableObject.CreateInstance<SourceRGBA>();
        node.name = "Source RGBA";
        node.rect = new Rect(pos.x, pos.y, 110, 100);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("Source", "SuperSource");
        node.CreateOutput("RGBA", "SuperFloat4");
     
        return node;
    }

 

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));
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
        tag = true;

        SuperFloat2 s_in2 = Inputs[0].GetValue<SuperFloat2>();
        SuperSource s_in = Inputs[1].GetValue<SuperSource>();
        SuperFloat4 s_out = new SuperFloat4();
        
        
        string NodeCount = MemoCount.ToString();
        string DefaultName = "SourceRGBA_" + NodeCount;
         string source = "";
        string uv = "";
        // uv
        if (s_in.Result == null)
        {
            source = "_MainTex";
        }
        else
        {
            source = s_in.Result;
        }

        // source
        if (s_in2.Result == null)
        {
            uv = "i.texcoord";
            if (source == "_MainTex") uv = "i.texcoord";
            if (source == "_GrabTexture") uv = "i.screenuv";
        }
        else
        {
            uv = s_in2.Result;
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew+ s_in2.StringPreviewNew;
        s_out.ValueLine = "float4 " + DefaultName + " = tex2D(" + source + ", " + uv + ");\n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}