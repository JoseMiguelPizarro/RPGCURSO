using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Threshold Smooth")]
public class ThresholdSmooth : Node
{
    [HideInInspector]
    public const string ID = "ThresholdSmooth";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 1.0f;
    [HideInInspector]
    public float Variable2 = 0.7f;
    [HideInInspector]
    public float Variable3 = 0.7f;
    [HideInInspector]
    public float Variable4 = 0.7f;
    [HideInInspector]
    public float Variable5 = 0.7f;

    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public bool parametersOK = true;

    public static void Init()
    {
        tag = false;
        count = 1;
    }





    public void Function()
    {
        code = "";
        code += "float4 ThresholdSmooth(float4 txt, float value, float smooth)\n";
        code += "{\n";
        code += "float l = (txt.x + txt.y + txt.z) * 0.33;\n";
        code += "txt.rgb = smoothstep(value, value + smooth, l);\n";
        code += "return txt;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        ThresholdSmooth node = ScriptableObject.CreateInstance<ThresholdSmooth>();

        node.name = "Threshold Smooth";

        node.rect = new Rect(pos.x, pos.y, 185, 250);
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_threshold_smooth.jpg");
        GUI.DrawTexture(new Rect(2, 0, 182, 44), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }

        if (GUILayout.Button("Reset"))
        {
            Variable = 0.5f;
            Variable2 = 0.5f;
            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            }
        }
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");
        GUILayout.Label("Value: (0 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 1);

        GUILayout.Label("Smooth: (0 to 1) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, 0, 1);


    }
    private string FinalVariable;
    private string FinalVariable2;
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
        string DefaultName = "_ThresholdSmooth_" + NodeCount;

        string DefaultNameVariable1 = "_ThresholdSmooth_Value_" + NodeCount;
        string DefaultNameVariable2 = "_ThresholdSmooth_Smooth_" + NodeCount;

        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(0, 1)) = " + Variable2.ToString();

        string rgba = s_in.Result;

        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;


        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = ThresholdSmooth(" + rgba + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = ThresholdSmooth(" + rgba + "," + Variable.ToString() + "," + Variable2.ToString() + ");\n";
        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;

        s_out.Result = DefaultName;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (parametersOK)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
        }
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}