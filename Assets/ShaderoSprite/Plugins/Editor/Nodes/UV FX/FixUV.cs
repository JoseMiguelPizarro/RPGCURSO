using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Fix Sides UV")]

public class FixUV : Node
{
    public const string ID = "FixUV";
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 0.25f;
    [HideInInspector]
    public bool AddParameters = true;

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
        code += "float2 FixSidesUV(float2 uv, float2 uv2)\n";
        code += "{\n";
        code += "float smooth = 0.08f;\n";
        code += "float r = 1 - smoothstep(0.0, smooth, uv2.x);\n";
        code += "r += smoothstep(1.- smooth, 1., uv2.x);\n";
        code += "r += 1 - smoothstep(0.0, smooth, uv2.y);\n";
        code += "r += smoothstep(1 - smooth, 1., uv2.y);\n";
        code += "r = saturate(r);\n";
        code += "uv = lerp(uv, uv2, r);\n";
        code += "return uv;\n";
        code += "}\n";
    }

    public override Node Create(Vector2 pos)
    {
        Function();
        FixUV node = ScriptableObject.CreateInstance<FixUV>();
        node.name = "Fix Sides UV";
        node.rect = new Rect(pos.x, pos.y, 172, 150);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_fixside.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
        GUILayout.EndHorizontal();

       
       
    }

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

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat2 s_out = new SuperFloat2();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "FixUV_";
         DefaultName = DefaultName + NodeCount;
        string VoidName = "FixSidesUV";
        string PreviewVariable = s_in.Result;
        if (PreviewVariable == null) PreviewVariable = "i.texcoord";
   
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + ", i.texcoord);\n";

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