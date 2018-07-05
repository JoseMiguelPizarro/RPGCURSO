using UnityEngine;
using UnityEditor;

using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Initialize/Render Texture")]
public class RenderedTexture : Node
{
    public const string ID = "RenderTexture";
    public override string GetID { get { return ID; } }
    [HideInInspector] public RenderTexture tex;
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
        RenderedTexture node = ScriptableObject.CreateInstance<RenderedTexture>();

        node.name = "Render Texture";
        node.rect = new Rect(pos.x, pos.y, 150, 210);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");
    
        return node;
    }

    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        tex = (RenderTexture)EditorGUI.ObjectField(new Rect(8, 30, 130, 130), tex, typeof(RenderTexture),true);
        if (NodeEditor._Shadero_Material != null)
        {
            if (tex != null)
            {
                NodeEditor._Shadero_Material.SetTexture(FinalVariable, tex);
            }
        }
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
        string DefaultName = "RenderTex_" + NodeCount;
        string DefaultNameVariable1 = "_RenderTex_" + NodeCount;

        FinalVariable = DefaultNameVariable1;

        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (s_in.Result == null)
        {
            s_out.ValueLine = "float4 "+ DefaultName + " = tex2D(" + DefaultNameVariable1 + ", i.texcoord);\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = tex2D(" + DefaultNameVariable1 + "," + s_in.Result + ");\n";
        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;

        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        s_out.ParametersLines += DefaultNameVariable1 + "(\""+ DefaultName +"(RGB)\", 2D) = \"white\" { }\n";
        s_out.ParametersDeclarationLines += "sampler2D " + DefaultNameVariable1 + ";\n";
    
        Outputs[0].SetValue<SuperFloat4>(s_out);
        count++;
        return true;
    }
}
}