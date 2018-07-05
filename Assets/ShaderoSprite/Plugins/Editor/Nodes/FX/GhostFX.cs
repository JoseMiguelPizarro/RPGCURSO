using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/FX/Ghost FX")]
public class GhostFX : Node
{
    [HideInInspector]
    public const string ID = "GhostFX";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 1.0f;
    [HideInInspector]
    public float Variable2 = 0.7f;
    [HideInInspector]
    public float Variable3 = 0.7f;
    [HideInInspector]
    public float Variable4 = 0.7f;
    [HideInInspector]
    public float Variable5 = 0.7f;
   
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
        code += "float4 GhostFX(float4 txt, float2 uv, float smooth, float up, float down, float left, float right)\n";
        code += "{\n";
        code += "float a = txt.a;\n";
        code += "float c1 = 1;\n";
        code += "float noffset = smooth;\n";
        code += "if (uv.y > up)      c1 = saturate(((1 + noffset) / (1 - up))*(1 - uv.y) - noffset);\n";
        code += "if (uv.y < 1 - down) c1 *= saturate(((1 + noffset) / (1 - down))*uv.y - noffset);\n";
        code += "if (uv.x > right)  c1 *= saturate(((1 + noffset) / (1 - right))*(1 - uv.x) - noffset);\n";
        code += "if (uv.x < 1 - left) c1 *= saturate(((1 + noffset) / (1 - left))*uv.x - noffset);\n";
        code += "txt.a = a*c1;\n";
        code += "return txt;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        GhostFX node = ScriptableObject.CreateInstance<GhostFX>();

        node.name = "Ghost FX";

        node.rect = new Rect(pos.x, pos.y, 180, 400);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_ghost.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
        }

        if (GUILayout.Button("Reset"))
        {
            Variable = 1.0f;
            Variable2 = 0.7f;
            Variable3 = 0.7f;
            Variable4 = 0.7f;
            Variable5 = 0.7f;
            
        }
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");
        GUILayout.Label("Offset: (0 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 1);
        
        GUILayout.Label("ClipUp: (0 to 1) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, 0, 1);
     
        GUILayout.Label("ClipDown: (0 to 1) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, 0, 1);

        GUILayout.Label("ClipLeft: (0 to 1) " + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, 0, 1);

        GUILayout.Label("ClipRight: (0 to 1) " + Variable5.ToString("0.00"));
        Variable5 =HorizontalSlider(Variable5, 0, 1);
       
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
        SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_GhostFX_" + NodeCount;
        string DefaultNameVariable1 = "_GhostFX_Smooth_" + NodeCount;
        string DefaultNameVariable2 = "_GhostFX_ClipUp_" + NodeCount;
        string DefaultNameVariable3 = "_GhostFX_ClipDown_" + NodeCount;
        string DefaultNameVariable4 = "_GhostFX_ClipLeft_" + NodeCount;
        string DefaultNameVariable5 = "_GhostFX_ClipRight_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 1)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(0, 1)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(0, 1)) = " + Variable3.ToString();
        string DefaultParameters4 = ", Range(0, 1)) = " + Variable4.ToString();
        string DefaultParameters5 = ", Range(0, 1)) = " + Variable5.ToString();
        string uv = s_in.Result;
        string rgba = s_in2.Result;

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

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = GhostFX(" + rgba + "," + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + "," + DefaultNameVariable5 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = GhostFX(" + rgba + "," + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + ");\n";
        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

        s_out.Result = DefaultName;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;
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