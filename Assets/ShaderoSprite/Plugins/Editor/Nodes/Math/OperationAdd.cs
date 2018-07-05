using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/With 2 RGBA/Addition")]
public class OperationAdd : Node
{
    [HideInInspector] public const string ID = "OperationAdd";
    [HideInInspector] public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 1;
    [HideInInspector]
    public bool parametersOK = true;
 
    public static int count = 1;
    public static bool tag = false;
    public static string code;
    [HideInInspector]
    public bool Include_RGBA_Alpha = false;
    [HideInInspector]
    public bool Include_RGBA_Alpha2 = false;

    public static void Init()
    {
        tag = false;
        count = 1;
    }

    public override Node Create(Vector2 pos)
    {
        OperationAdd node = ScriptableObject.CreateInstance<OperationAdd>();

        node.name = "Add = A with B";
        node.rect = new Rect(pos.x, pos.y, 172, 290);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_rgba_addition.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA (A)", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA (nA)", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("RGBA (B)", "RGBA"));
        GUILayout.Space(10);
        Include_RGBA_Alpha = GUILayout.Toggle(Include_RGBA_Alpha, "Include RGBA Alpha");
        Include_RGBA_Alpha2 = GUILayout.Toggle(Include_RGBA_Alpha2, "Include RGBA 2 Alpha");
        if (GUILayout.Button("Reset"))
        {
            Variable = 1.0f;
          }
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }
        GUILayout.Label("Fade: (0 to 4) " + Variable.ToString("0.00"));
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
  
        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefautTypeFade = "float";
        string DefaultName = "OperationAdd_" + NodeCount;
        string DefaultNameVariable1 = "_Add_Fade_" + NodeCount;

        FinalVariable = DefaultNameVariable1;

        string DefaultParameters1 = ", Range(0, 4)) = "+ Variable.ToString();
        string Value1 = Variable.ToString();
        string PreviewVariable = s_in.Result;
        string PreviewVariable2 = s_in2.Result;
        DefaultName = s_in.Result;
        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
         }
        if (s_in2.Result == null)
        {
            PreviewVariable2 = "float4(0,0,0,1)";
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

        string AddAlpha="";
        if (Include_RGBA_Alpha) AddAlpha = " * " + PreviewVariable + ".a";
        string AddAlpha2 = "";
        if (Include_RGBA_Alpha2) AddAlpha = " * " + PreviewVariable2 + ".a";


        if (parametersOK)
        {
            s_out.ValueLine = PreviewVariable + " = lerp(" + PreviewVariable + "," + PreviewVariable + "*" + PreviewVariable + ".a + " + PreviewVariable2 + "*" + PreviewVariable2 + ".a," + DefaultNameVariable1 + AddAlpha + AddAlpha2 + ");\n";
        }
        else
        {
            s_out.ValueLine = PreviewVariable + " = lerp(" + PreviewVariable + "," + PreviewVariable + "*" + PreviewVariable + ".a + " + PreviewVariable2 + "*" + PreviewVariable2 + ".a," + Value1 + AddAlpha + AddAlpha2 + ");\n";
        }

       
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;

        s_out.ParametersLines += s_in.ParametersLines+ s_in2.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines+ s_in2.ParametersDeclarationLines;
        if (parametersOK) s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        if (parametersOK) s_out.ParametersDeclarationLines += DefautTypeFade + " " + DefaultNameVariable1 + ";\n";

        Outputs[0].SetValue<SuperFloat4>(s_out);
        count++;

         return true;
    }
}
}