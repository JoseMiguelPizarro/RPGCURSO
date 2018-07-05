using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/FX/Compression FX")]
public class CompressionFX : Node
{
    [HideInInspector] public const string ID = "CompressionFX";
    [HideInInspector] public override string GetID { get { return ID; } }
     [HideInInspector] public float Variable = 1;
    [HideInInspector] [Multiline(15)] public string result;

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
        code += "float CMPFXrng2(float2 seed)\n";
        code += "{\n";
        code += "return frac(sin(dot(seed * floor(50 + (_Time + 0.1) * 12.), float2(127.1, 311.7))) * 43758.5453123);\n";
        code += "}\n";
        code += "\n";
        code += "float CMPFXrng(float seed)\n";
        code += "{\n";
        code += "return CMPFXrng2(float2(seed, 1.0));\n";
        code += "}\n";
        code += "\n";
        code += "float4 CompressionFX(float2 uv, sampler2D source,float Value)\n";
        code += "{\n";
        code += "float2 blockS = floor(uv * float2(24., 19.))*4.0;\n";
        code += "float2 blockL = floor(uv * float2(38., 14.))*4.0;\n";
        code += "float r = CMPFXrng2(uv);\n";
        code += "float lineNoise = pow(CMPFXrng2(blockS), 3.0) *Value* pow(CMPFXrng2(blockL), 3.0);\n";
        code += "float4 col1 = tex2D(source, uv + float2(lineNoise * 0.02 * CMPFXrng(2.0), 0));\n";
        code += "float4 result = float4(float3(col1.x, col1.y, col1.z), 1.0);\n";
        code += "result.a = col1.a;\n";
        code += "return result;\n";
        code += "}\n";

    }


    public override Node Create(Vector2 pos)
    {
        Function();

        CompressionFX node = ScriptableObject.CreateInstance<CompressionFX>();

        node.name = "Compression FX";
        node.rect = new Rect(pos.x, pos.y, 172, 250);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("Source", "SuperSource");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_compression.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));

        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }
        GUILayout.Label("(0 to 16) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 16);



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

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
        SuperFloat4 s_out = new SuperFloat4();


        string NodeCount = MemoCount.ToString();
        string DefaultName = "_CompressionFX_" + NodeCount;
        string DefaultNameVariable1 = "__CompressionFX_Value_" + NodeCount;
        string DefaultParameters1 = ", Range(1, 16)) = " + Variable.ToString();
        string uv = s_in.Result;
        string Source = "";

        FinalVariable = DefaultNameVariable1;

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

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = CompressionFX(" + uv + "," + Source + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = CompressionFX(" + uv + "," + Source + "," + Variable.ToString() + ");\n";

        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

        if (parametersOK)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        result = s_out.StringPreviewNew;
        return true;
    }
}
}