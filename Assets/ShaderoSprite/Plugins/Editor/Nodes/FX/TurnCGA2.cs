using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Colorify/TurnCGA2")]
public class TurnCGA2 : Node
{
    [HideInInspector] public const string ID = "TurnCGA2";
    [HideInInspector] public override string GetID { get { return ID; } }
    [HideInInspector] public bool parametersOK = true;
 
    [HideInInspector] public float Variable = 1f;
 
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
            code ="";
            code += "float4 TurnCGA2(float4 txt, float value)\n";
            code += "{\n";
            code += "float3 a = float3(0, 0, 0);\n";
            code += "#define Turn(n) a = lerp(n, a, step (length (a-txt.rgb), length (n-txt.rgb)));\n";
            code += "Turn(float3(0, 0, 0));\n";
            code += "Turn(float3(0.33, 1, 0.33));\n";
            code += "Turn(float3(1, 0.33, 0.33));\n";
            code += "Turn(float3(1, 1, 0.33));\n";
            code += "a = lerp(txt.rgb,a,value);\n";
            code += "return float4(a, txt.a);\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
        {

        Function();
        TurnCGA2 node = ScriptableObject.CreateInstance<TurnCGA2>();
        node.name = "Turn CGA2";
        node.rect = new Rect(pos.x, pos.y, 180, 180);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");
     
        return node;
        }

 

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_turn_cga2.jpg");
        GUI.DrawTexture(new Rect(2, 0, 178, 44), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        parametersOK = GUILayout.Toggle(parametersOK,"Add Parameter");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }

        GUILayout.Label("Value: (0 to 1) " + Variable.ToString("0.00"));
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
        string DefaultName = "TurnCGA2_" + NodeCount;
        string DefaultNameVariable1 = "_TurnCGA2_Fade_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 1)) = "+Variable.ToString(); 
        string VoidName = "TurnCGA2";
        string Value1 = Variable.ToString(); 
        string PreviewVariable = s_in.Result;

        FinalVariable = DefaultNameVariable1;

        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (parametersOK)
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + DefaultNameVariable1 + ");\n";
        }
        else
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + Value1 + ");\n";
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