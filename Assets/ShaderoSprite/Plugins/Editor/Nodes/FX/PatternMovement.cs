using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/With 2 RGBA/Pattern Movement")]
public class PatternMovement : Node
{
    [HideInInspector] public const string ID = "PatternMovement";
    [HideInInspector] public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 1;
    [HideInInspector] public float Variable2 = 1;
    [HideInInspector] public float Variable3 = 1;
  
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector] public bool parametersOK = true;

    public static void Init()
    {
        tag = false;
        count = 1;
    }
    public void Function()
    {
        code = "";
        code += "float4 PatternMovement(float2 uv, sampler2D source, float4 rgba, float posx, float posy, float speed)\n";
        code += "{\n";
        code += "float t = _Time * 20 * speed;\n";
        code += "uv = fmod(abs(uv+float2(posx*t, posy*t)),1);\n";
        code += "float4 result = tex2D(source, uv);\n";
        code += "result.a = result.a * rgba.a;\n";
        code += "return result;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        PatternMovement node = ScriptableObject.CreateInstance<PatternMovement>();

        node.name = "Pattern Movement";
        node.rect = new Rect(pos.x, pos.y, 172, 350);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("Source", "SuperSource");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_blur_hq.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);

        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));
        Inputs[2].DisplayLayout(new GUIContent("RGBA (Alpha)", "RGBA"));


        if (GUILayout.Button("Reset"))
        {
            Variable = 0;
            Variable2 = 0;
            Variable3 = 1;

        }

        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

        // Paramaters
        if (NodeEditor._Shadero_Material != null)
        {
           NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
        }

        GUILayout.Label("PosX: (-2 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -2, 2);

        GUILayout.Label("PosY: (-2 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -2, 2);

        GUILayout.Label("Speed: (-4 to 4) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, 0, 8);



    }
    private string FinalVariable;
    private string FinalVariable2;
    private string FinalVariable3;
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
        SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
        SuperFloat4 s_in3 = Inputs[2].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();


        string NodeCount = MemoCount.ToString();
        string DefaultName = "_PatternMovement_" + NodeCount;
        string DefaultNameVariable1 = "_PatternMovement_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_PatternMovement_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_PatternMovement_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(-2, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-2, 2)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(1, 16)) = " + Variable3.ToString();
        string uv = s_in.Result;
        string Source = "";

        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;
        string PreviewVariable = s_in3.Result;
       
        // uv
        if (s_in2.Result == null)
        {
            Source = "_MainTex";
        }
        else
        {
            Source = s_in2.Result;
        }

        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
            if (Source == "_MainTex") uv = "i.texcoord";
            if (Source == "_GrabTexture") uv = "i.screenuv";
        }
        else
        {
            uv = s_in.Result;
        }

        if (s_in3.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }


        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew + s_in3.StringPreviewNew;
        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = PatternMovement(" + uv + "," + Source + "," + PreviewVariable + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = PatternMovement(" + uv + "," + Source + "," + PreviewVariable + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + ");\n";
        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines + s_in3.ParametersLines;

        s_out.Result = DefaultName;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines + s_in3.ParametersDeclarationLines;
        if (parametersOK)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
            s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}