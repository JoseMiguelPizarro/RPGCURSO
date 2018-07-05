using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/FX/Desintegration")]
public class Desintegration : Node
{
    [HideInInspector]
    public const string ID = "Desintegration";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0.5f;
    [HideInInspector]
    public float Variable2 = 0.5f;
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
        code += "float DGFXr (float2 c, float seed)\n";
        code += "{\n";
        code += "return frac(43.*sin(c.x+7.*c.y)*seed);\n";
        code += "}\n";
        code += "\n";
        code += "float DGFXn (float2 p, float seed)\n";
        code += "{\n";
        code += "float2 i = floor(p), w = p-i, j = float2 (1.,0.);\n";
        code += "w = w*w*(3.-w-w);\n";
        code += "return lerp(lerp(DGFXr(i, seed), DGFXr(i+j, seed), w.x), lerp(DGFXr(i+j.yx, seed), DGFXr(i+1., seed), w.x), w.y);\n";
        code += "}\n";
        code += "\n";
        code += "float DGFXa (float2 p, float seed)\n";
        code += "{\n";
        code += "float m = 0., f = 2.;\n";
        code += "for ( int i=0; i<9; i++ ){ m += DGFXn(f*p, seed)/f; f+=f; }\n";
        code += "return m;\n";
        code += "}\n";
        code += "\n";
        code += "float4 DesintegrationFX(float4 txt, float2 uv, float value, float seed)\n";
        code += "{\n";
        code += "\n";
        code += "float t = frac(value*0.9999);\n";
        code += "float4 c = smoothstep(t / 1.2, t + .1, DGFXa(3.5*uv, seed));\n";
        code += "c = txt*c;\n";
        code += "c.r = lerp(c.r, c.r*(1 - c.a), value);\n";
        code += "c.g = lerp(c.g, c.g*(1 - c.a), value);\n";
        code += "c.b = lerp(c.b, c.b*(1 - c.a), value);\n";
        code += "return c;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        Desintegration node = ScriptableObject.CreateInstance<Desintegration>();

        node.name = "Desintegration";
        node.rect = new Rect(pos.x, pos.y, 172, 250);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");
         
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_desintegration.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));

        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
        }
        GUILayout.Label("Value: (0 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 1);
        GUILayout.Label("Seed: (-8 to 8) " + Variable2.ToString("0.00"));
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

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Desintegration_" + NodeCount;
        string DefaultNameVariable1 = "_Desintegration_Value_" + NodeCount;
        string DefaultNameVariable2 = "_Desintegration_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(0, 1)) = " + Variable2.ToString();
        string uv = s_in.Result;
        string rgba = s_in2.Result;
   
        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;

       

        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
         }
        else
        {
            uv = s_in.Result;
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

   

        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = DesintegrationFX(" + rgba + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";

        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = DesintegrationFX(" + rgba + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString() + ");\n";

        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

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