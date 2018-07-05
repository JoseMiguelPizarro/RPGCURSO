using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/With 2 RGBA/Subtraction")]
public class OperationSub : Node
{
    [HideInInspector]
    public const string ID = "OperationSub";
    [HideInInspector]
    public override string GetID { get { return ID; } }

      [HideInInspector] public float Variable = 1;
     [HideInInspector]
    public bool parametersOK = true;
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    public static void Init()
    {
        tag = false;
        count = 1;
    }
    public override Node Create(Vector2 pos)
    {
        OperationSub node = ScriptableObject.CreateInstance<OperationSub>();

        node.name = "Sub = A with B";
        node.rect = new Rect(pos.x, pos.y, 172, 210);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
           Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_rgba_sub.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
       GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA A", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("RGBA B", "RGBA"));
        parametersOK = GUILayout.Toggle(parametersOK, "Add Fading Parameter");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }
        GUILayout.Label("Fade: (0 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 1);



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
        string DefaultName = "OperationSub_" + NodeCount;
        string DefaultNameVariable1 = "_Sub_Fade_" + NodeCount;

        FinalVariable = DefaultNameVariable1;

        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
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
      
        if (parametersOK)
        {
            s_out.ValueLine = PreviewVariable + " = lerp(" + PreviewVariable + "," + PreviewVariable + " - " + PreviewVariable2 + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = PreviewVariable + " = lerp(" + PreviewVariable + "," + PreviewVariable + " - " + PreviewVariable2 + "," + Value1 + ");\n";
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
