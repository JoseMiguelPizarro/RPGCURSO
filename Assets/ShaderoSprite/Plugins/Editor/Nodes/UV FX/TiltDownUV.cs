using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/Tilt/Tilt Down")]

public class TiltDownUV : Node
{
    public const string ID = "TiltDownUV";
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 0;
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
        code += "float2 TiltDownUV(float2 uv, float offsetX)\n";
        code += "{\n";
        code += "uv += float2(offsetX*(1-uv.y), 0);\n";
        code += "return uv;\n";
        code += "}\n";
    }
    public override Node Create(Vector2 pos)
    {
        Function();
        TiltDownUV node = ScriptableObject.CreateInstance<TiltDownUV>();
        node.name = "Tilt Down UV";
        node.rect = new Rect(pos.x, pos.y, 172, 210);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_tilt_down.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
        GUILayout.EndHorizontal();
     
        if (GUILayout.Button("Reset"))
        {
            Variable = 0;
        }
        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        if (NodeEditor._Shadero_Material != null) NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
        GUILayout.Label("Tilt: (-2 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -2, 2);

    }

    private string FinalVariable1;
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
        string DefaultName = "TiltDownUV_";
        string DefaultNameVariable1 = DefaultName + "TiltDownUV_Value_" + NodeCount;
        DefaultName = DefaultName + NodeCount;
        string DefaultParameters1 = ", Range(-2, 2)) = " + Variable.ToString();
        string PreviewVariable = s_in.Result;
        if (PreviewVariable == null) PreviewVariable = "i.texcoord";

        FinalVariable1 = DefaultNameVariable1;

         s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (AddParameters)
        {
            s_out.ValueLine = "float2 " + DefaultName + " = TiltDownUV(" + PreviewVariable + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float2 " + DefaultName + " = TiltDownUV(" + PreviewVariable + "," + Variable.ToString() + ");\n";
        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat2>(s_out);

        count++;

        return true;
    }
}
}