using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Hdr Control Value")]
public class HdrControlValue : Node
{
    [HideInInspector]
    public const string ID = "HdrControlValue";
    [HideInInspector]
    public override string GetID { get { return ID; } }

    [HideInInspector]
    public bool parametersOK = true;

    [HideInInspector]
    public float Variable = 1;

    [HideInInspector]
    public float InternalCount = -1;

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
        code += "float4 HDRControlValue(float4 txt,float value)\n";
        code += "{\n";
        code += "return lerp(saturate(txt),txt, value);\n";
        code += "}\n";
  
    }


    public override Node Create(Vector2 pos)
    {

        Function();
        InternalCount = -1;

        HdrControlValue node = ScriptableObject.CreateInstance<HdrControlValue>();
        node.name = "Hdr Control Value";
        node.rect = new Rect(pos.x, pos.y, 172, 230);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }



    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_hdrcontrolvalue.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reset"))
        {
            Variable = 1f;
        }
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }
        GUILayout.Label("HDR Intensity");
        GUILayout.Label("(0 to 4) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 4);


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

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();


        string NodeCount = MemoCount.ToString();
        string DefautType = "float4";
        string DefautTypeFade = "float";
        string DefaultName = "HDRControlValue_" + NodeCount;
        string DefaultNameVariable1 = "_HDRControlValue_Value_" + NodeCount;
        string DefaultParameters1 = ", Range(-2, 2)) = " + Variable.ToString();
        string VoidName = "HDRControlValue";
        string Value1 = Variable.ToString();
        string PreviewVariable = s_in.Result;

        FinalVariable = DefaultNameVariable1;

        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (parametersOK)
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + Value1 + ");\n";
        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (parametersOK) s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        if (parametersOK) s_out.ParametersDeclarationLines += DefautTypeFade + " " + DefaultNameVariable1 + ";\n";

        Outputs[0].SetValue<SuperFloat4>(s_out);

        return true;
    }
}
}