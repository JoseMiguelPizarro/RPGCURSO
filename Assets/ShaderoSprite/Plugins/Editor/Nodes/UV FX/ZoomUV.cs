using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Zoom UV")]

public class ZoomUV : Node
{
    public const string ID = "ZoomUV";
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 1;
    [HideInInspector] public float Variable2 = 0.5f;
    [HideInInspector] public float Variable3 = 0.5f;
    [HideInInspector] public bool AddParameters = true;

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
      code += "float2 ZoomUV(float2 uv, float zoom, float posx, float posy)\n";
      code += "{\n";
      code += "float2 center = float2(posx, posy);\n";
      code += "uv -= center;\n";
      code += "uv = uv * zoom;\n";
      code += "uv += center;\n";
      code += "return uv;\n";
      code += "}\n";
       
    }
    public override Node Create(Vector2 pos)
    {
        Function();
        ZoomUV node = ScriptableObject.CreateInstance<ZoomUV>();
        node.name = "Zoom UV";
        node.rect = new Rect(pos.x, pos.y, 172, 300);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_zoom.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            Variable = 1;
            Variable2 = 0.5f;
            Variable3 = 0.5f;
        }
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
        }

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        GUILayout.Label("Zoom (0.2 to 4) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, 0.001f, 4);
        GUILayout.Label("Pos X (-3 to 3) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -3, 3);
        GUILayout.Label("Pos Y (-3 to 3) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, -3, 3);
      
    }

    private string FinalVariable1;
    private string FinalVariable2;
    private string FinalVariable3;
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
        SuperFloat2 s_out = new SuperFloat2();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "ZoomUV_";
        string DefaultNameVariable1 = DefaultName + "Zoom_" + NodeCount;
        string DefaultNameVariable2 = DefaultName + "PosX_" + NodeCount;
        string DefaultNameVariable3 = DefaultName + "PosY_" + NodeCount;
        DefaultName = DefaultName + NodeCount;
        string DefaultParameters1 = ", Range(0.2, 4)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-3, 3)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(-3, 3)) =" + Variable3.ToString();
        string VoidName = "ZoomUV";
        string PreviewVariable = s_in.Result;
         if (PreviewVariable == null) PreviewVariable = "i.texcoord";
   
        FinalVariable1 = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;

        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (AddParameters)
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + ");\n";
        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
            s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";

            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat2>(s_out);

        count++;

        return true;
    }
}
}