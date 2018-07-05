using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Fish Eye UV")]

public class FishEyeUV : Node
{
    public const string ID = "FishEyeUV";
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
        code += "float2 FishEyeUV(float2 uv, float size)\n";
        code += "{\n";
        code += "float2 m = float2(0.5, 0.5);\n";
        code += "float2 d = uv - m;\n";
        code += "float r = sqrt(dot(d, d));\n";
        code += "float power = (2.0 * 3.141592 / (2.0 * sqrt(dot(m, m)))) * (size+0.001);\n";
        code += "float bind = sqrt(dot(m, m));\n";
        code += "uv = m + normalize(d) * tan(r * power) * bind / tan(bind * power);\n";
        code += "return uv;\n";
        code += "}\n";
    }

    public override Node Create(Vector2 pos)
    {
        Function();
        FishEyeUV node = ScriptableObject.CreateInstance<FishEyeUV>();
        node.name = "FishEye UV";
        node.rect = new Rect(pos.x, pos.y, 172, 220);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_fisheye.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            Variable = 0.2f;
     
        }

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
    

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
        }
        GUILayout.Label("Intensity: ");
        GUILayout.Label("(0 to 0.5) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0f, 0.5f);
    
       
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
        string DefaultName = "FishEyeUV_";
        string DefaultNameVariable1 = DefaultName + "Size_" + NodeCount;
         DefaultName = DefaultName + NodeCount;
        string DefaultParameters1 = ", Range(0, 0.5)) = " + Variable.ToString();
        string VoidName = "FishEyeUV";
        string PreviewVariable = s_in.Result;
        if (PreviewVariable == null) PreviewVariable = "i.texcoord";
   
        FinalVariable1 = DefaultNameVariable1;
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (AddParameters)
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + DefaultNameVariable1 +");\n";
        }
        else
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + Variable.ToString() +");\n";

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
