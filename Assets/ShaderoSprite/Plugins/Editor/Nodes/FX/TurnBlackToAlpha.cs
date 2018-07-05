using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Turn Black To Alpha")]
public class TurnBlackToAlpha : Node
{
    [HideInInspector] public const string ID = "TurnBlackToAlpha";
    [HideInInspector] public override string GetID { get { return ID; } }

    [HideInInspector] public bool parametersOK = true;
  [HideInInspector] public bool ForceColorBlack = false;
 
    [HideInInspector] public float Variable = 1;
  
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
        code = "\n";
        code += "float4 TurnBlackToAlpha(float4 txt, float force, float fade)\n";
        code += "{\n";
        code += "float3 gs = dot(txt.rgb, float3(1., 1., 1.));\n";
        code += "gs=saturate(gs);\n";
        code += "return lerp(txt,float4(force*txt.rgb, gs.r), fade);\n";
        code += "}\n";
        code += "\n";
    }

    public override Node Create(Vector2 pos)
    {

        Function();

        TurnBlackToAlpha node = ScriptableObject.CreateInstance<TurnBlackToAlpha>();
        node.name = "Turn Black To Alpha";
        node.rect = new Rect(pos.x, pos.y, 172, 200);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");
     
        return node;
    }

 

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_blackToAlpha.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        parametersOK = GUILayout.Toggle(parametersOK,"Add Parameter");
        ForceColorBlack = GUILayout.Toggle(ForceColorBlack, "Change color to Black");

        // Paramaters
                if (NodeEditor._Shadero_Material != null)
                {
                    NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                }
            GUILayout.Label("Fading: (0 to 1) " + Variable.ToString("0.00"));
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
        tag = true;

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();
        
        
        string NodeCount = MemoCount.ToString();
        string DefautType = "float4";
        string DefautTypeFade = "float";
        string DefaultName = "TurnBlackToAlpha_" + NodeCount;
        string DefaultNameVariable1 = "_TurnBlackToAlpha_Fade_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
        string VoidName = "TurnBlackToAlpha";
        string Value1 = Variable.ToString(); 
        string PreviewVariable = s_in.Result;

        FinalVariable = DefaultNameVariable1;

        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }
        string forced = "1";
        if (ForceColorBlack) forced = "0";
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (parametersOK)
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + forced + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + forced + "," + Value1 + ");\n";
        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (parametersOK) s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\""+ DefaultParameters1+"\n";
        if (parametersOK) s_out.ParametersDeclarationLines += DefautTypeFade + " " + DefaultNameVariable1 + ";\n";

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
         return true;
    }
}
}