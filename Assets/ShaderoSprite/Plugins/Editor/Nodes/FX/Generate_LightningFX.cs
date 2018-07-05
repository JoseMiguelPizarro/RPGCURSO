using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Effects/Lightning FX")]
public class Generate_LightningFX : Node
{
     [HideInInspector]
    public const string ID = "Generate_LightningFX";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0.5f;
    [HideInInspector]
    public float Variable2 = 0.5f;
    [HideInInspector]
    public float Variable3 = 4f;
    [HideInInspector]
    public float Variable4 = 4f;
    [HideInInspector]
    public float Variable5 = 1f;
    [HideInInspector]
    [Multiline(15)]
    public string result;

    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public bool Black=false;
    [HideInInspector]
    public bool LeftRight = false;


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
        code += "float Lightning_Hash(float2 p)\n";
        code += "{\n";
        code += "float3 p2 = float3(p.xy, 1.0);\n";
        code += "return frac(sin(dot(p2, float3(37.1, 61.7, 12.4)))*3758.5453123);\n";
        code += "}\n";
        code += "\n";
        code += "float Lightning_noise(in float2 p)\n";
        code += "{\n";
        code += "float2 i = floor(p);\n";
        code += "float2 f = frac(p);\n";
        code += "f *= f * (1.5 - .5*f);\n";
        code += "return lerp(lerp(Lightning_Hash(i + float2(0., 0.)), Lightning_Hash(i + float2(1., 0.)), f.x),\n";
        code += "lerp(Lightning_Hash(i + float2(0., 1.)), Lightning_Hash(i + float2(1., 1.)), f.x),\n";
        code += "f.y);\n";
        code += "}\n";
        code += "\n";
        code += "float Lightning_fbm(float2 p)\n";
        code += "{\n";
        code += "float v = 0.0;\n";
        code += "v += Lightning_noise(p*1.0)*.5;\n";
        code += "v += Lightning_noise(p*2.)*.25;\n";
        code += "v += Lightning_noise(p*4.)*.125;\n";
        code += "v += Lightning_noise(p*8.)*.0625;\n";
        code += "return v;\n";
        code += "}\n";
        code += "\n";
        code += "float4 Generate_Lightning(float2 uv, float2 uvx, float posx, float posy, float size, float number, float speed, float black)\n";
        code += "{\n";
        code += "uv -= float2(posx, posy);\n";
        code += "uv *= size;\n";
        code += "uv -= float2(posx, posy);\n";
        code += "float rot = (uv.x*uvx.x + uv.y*uvx.y);\n";
        code += "float time = _Time * 20 * speed;\n";
        code += "float4 r = float4(0, 0, 0, 0);\n";
        code += "for (int i = 1; i < number; ++i)\n";
        code += "{\n";
        code += "float t = abs(.750 / ((rot + Lightning_fbm(uv + (time*5.75) / float(i)))*65.));\n";
        code += "r += t *0.5;\n";
        code += "}\n";
        code += "r = saturate(r);\n";
        code += "r.a = saturate(r.r + black);\n";
        code += "return r;\n";
        code += "\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        Generate_LightningFX node = ScriptableObject.CreateInstance<Generate_LightningFX>();

        node.name = "Generate_Lightning FX";

        node.rect = new Rect(pos.x, pos.y, 172, 450);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_burn.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Reset"))
        {
            Variable = 0.5f;
            Variable2 = 0.5f;
            Variable3 = 4f;
            Variable4 = 4f;
            Variable5 = 1f;

        }

        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
        }

        GUILayout.Label("Pos X: (-2 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -2, 2);

        GUILayout.Label("Pos Y: (-2 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -2, 2);
        
        GUILayout.Label("Size : (1 to 8) " + Variable2.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, 1, 8);

        GUILayout.Label("Number: (2 to 16) " + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, 2, 16);
        
        GUILayout.Label("Speed: (0 to 8) " + Variable5.ToString("0.00"));
        Variable5 =HorizontalSlider(Variable5, 0, 8);

        LeftRight = GUILayout.Toggle(LeftRight, "Left Right");
        Black = GUILayout.Toggle(Black, "Force Black");


    }
    private string FinalVariable;
    private string FinalVariable2;
    private string FinalVariable3;
    private string FinalVariable4;
    private string FinalVariable5;
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
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_GenerateLightning_" + NodeCount;
        string DefaultNameVariable1 = "_GenerateLightning_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_GenerateLightning_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_GenerateLightning_Size_" + NodeCount;
        string DefaultNameVariable4 = "_GenerateLightning_Number_" + NodeCount;
        string DefaultNameVariable5 = "_GenerateLightning_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(-2, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-2, 2)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range( 1, 8)) = " + Variable3.ToString();
        string DefaultParameters4 = ", Range(2, 16)) = " + Variable4.ToString();
        string DefaultParameters5 = ", Range( 0, 8)) = " + Variable5.ToString();
        string uv = s_in.Result;
       
        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;
        FinalVariable4 = DefaultNameVariable4;
        FinalVariable5 = DefaultNameVariable5;



        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
        }
        else
        {
            uv = s_in.Result;
        }
   
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        string Direction = "float2(0,1)";
        string black = "0";

        if (LeftRight) Direction = "float2(1,0)";
        if (Black) black = "1";

        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Lightning(" + uv + ","+ Direction+"," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + ","+ black + ");\n";

        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = Generate_Lightning(" + uv + "," + Direction + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + "," + black + ");\n";

        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;

        s_out.Result = DefaultName;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (parametersOK)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
                s_out.ParametersLines += DefaultNameVariable4 + "(\"" + DefaultNameVariable4 + "\"" + DefaultParameters4 + "\n";
                s_out.ParametersLines += DefaultNameVariable5 + "(\"" + DefaultNameVariable5 + "\"" + DefaultParameters5 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

            count++;
            return true;
        }
}
}