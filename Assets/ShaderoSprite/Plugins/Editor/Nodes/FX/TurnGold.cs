using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Turn Gold")]
public class TurnGold : Node
{
    [HideInInspector]
    public const string ID = "TurnGold";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 1;
    [HideInInspector]
    public float Variable2 = 1;
    [HideInInspector]
  
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
        code += "float4 ColorTurnGold(float2 uv, sampler2D txt, float speed)\n";
        code += "{\n";
        code += "float4 txt1=tex2D(txt,uv);\n";
        code += "float lum = dot(txt1.rgb, float3 (0.2126, 0.2152, 0.4722));\n";
        code += "float3 metal = float3(lum,lum,lum);\n";
        code += "metal.r = lum * pow(1.46*lum, 4.0);\n";
        code += "metal.g = lum * pow(1.46*lum, 4.0);\n";
        code += "metal.b = lum * pow(0.86*lum, 4.0);\n";
        code += "float2 tuv = uv;\n";
        code += "uv *= 2.5;\n";
        code += "float time = (_Time/4)*speed;\n";
        code += "float a = time * 50;\n";
        code += "float n = sin(a + 2.0 * uv.x) + sin(a - 2.0 * uv.x) + sin(a + 2.0 * uv.y) + sin(a + 5.0 * uv.y);\n";
        code += "n = fmod(((5.0 + n) / 5.0), 1.0);\n";
        code += "n += tex2D(txt, tuv).r * 0.21 + tex2D(txt, tuv).g * 0.4 + tex2D(txt, tuv).b * 0.2;\n";
        code += "n=fmod(n,1.0);\n";
        code += "float tx = n * 6.0;\n";
        code += "float r = clamp(tx - 2.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);\n";
        code += "float4 sortie=float4(1.0, 1.0, 1.0,r);\n";
        code += "sortie.rgb=metal.rgb+(1-sortie.a);\n";
        code += "sortie.rgb=sortie.rgb/2+dot(sortie.rgb, float3 (0.1126, 0.4552, 0.1722));\n";
        code += "sortie.rgb-=float3(0.0,0.1,0.45);\n";
        code += "sortie.rg+=0.025;\n";
        code += "sortie.a=txt1.a;\n";
        code += "return sortie; \n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        TurnGold node = ScriptableObject.CreateInstance<TurnGold>();

        node.name = "Turn Gold FX";
        node.rect = new Rect(pos.x, pos.y, 172, 200);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("Source", "SuperSource");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_turntogold.jpg");
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

        GUILayout.Label("Speed: (-8 to 8) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable,-8, 8);
      
           
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
        string DefaultName = "_TurnGold_" + NodeCount;
        string DefaultNameVariable1 = "_TurnGold_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(-8, 8)) = " + Variable.ToString();
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
            s_out.ValueLine = "float4 " + DefaultName + " = ColorTurnGold(" + uv + "," + Source + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = ColorTurnGold(" + uv + "," + Source + "," + Variable.ToString() + ");\n";
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
         return true;
    }
}
}