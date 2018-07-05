using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Math/RGBA - value")]
public class RGBA_Sub : Node
{
    [HideInInspector] public const string ID = "RGBA_Sub";
    [HideInInspector] public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 1;
    [HideInInspector]
    public bool parametersOK = true;

    [HideInInspector]
    public bool R = true;
    [HideInInspector]
    public bool G = true;
    [HideInInspector]
    public bool B = true;
    [HideInInspector]
    public bool A = false;



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
        RGBA_Sub node = ScriptableObject.CreateInstance<RGBA_Sub>();

        node.name = "RGBA Sub value";
        node.rect = new Rect(pos.x, pos.y, 172, 260);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_math_min.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA (A)", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA (nA)", "RGBA"));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        R = GUILayout.Toggle(R, "for (R)ed");
        G = GUILayout.Toggle(G, "for (G)reen");
        B = GUILayout.Toggle(B, "for (B)lue");
        A = GUILayout.Toggle(A, "for (A)lpha");
        parametersOK = GUILayout.Toggle(parametersOK, "Add Fading Parameter");

        // Paramaters
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
        }
        GUILayout.Label("Value: (0 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0, 2);


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
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefautTypeFade = "float";
        string DefaultName = "RGBA_Sub_" + NodeCount;
        string DefaultNameVariable1 = "_RGBA_Sub_Fade_" + NodeCount;

        FinalVariable = DefaultNameVariable1;

        string DefaultParameters1 = ", Range(0, 2)) = " + Variable.ToString();
        string Value1 = Variable.ToString();
        string PreviewVariable = s_in.Result;
        DefaultName = s_in.Result;
        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }
   
           s_out.StringPreviewLines = s_in.StringPreviewNew;


        if (parametersOK)
        {
            string xR = "";
            string xG = "";
            string xB = "";
            string xA = "";
            string xStart = ".";
            if (R == false && G == false && B == false && A == false) xStart = "";
            if (R) xR = "r";
            if (G) xG = "g";
            if (B) xB = "b";
            if (A) xA = "a";

            s_out.ValueLine = PreviewVariable + xStart + xR + xG + xB + xA + " -= " + DefaultNameVariable1 + ";\n";
        }
        else
        {
            string xR = "";
            string xG = "";
            string xB = "";
            string xA = "";
            string xStart = ".";

            if (R == false && G == false && B == false && A == false) xStart = "";
            if (R) xR = "r";
            if (G) xG = "g";
            if (B) xB = "b";
            if (A) xA = "a";
            s_out.ValueLine = PreviewVariable + xStart + xR + xG + xB + xA + " -= " + Value1 + ";\n";
        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;

        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (parametersOK) s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        if (parametersOK) s_out.ParametersDeclarationLines += DefautTypeFade + " " + DefaultNameVariable1 + ";\n";

        Outputs[0].SetValue<SuperFloat4>(s_out);

        if (parametersOK) count++;
        return true;
    }
}
}
