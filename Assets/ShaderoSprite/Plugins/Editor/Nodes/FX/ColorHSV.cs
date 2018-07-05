using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/HSV")]
public class ColorHSV : Node
{
    [HideInInspector]
    public const string ID = "ColorHSV";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 180f;
    [HideInInspector] public float Variable2 = 1f;
    [HideInInspector] public float Variable3 = 1f;

    [HideInInspector] public bool parametersOK = true;

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
        code += "float4 ColorHSV(float4 RGBA, float HueShift, float Sat, float Val)\n";
        code += "{\n";
        code += "\n";
        code += "float4 RESULT = float4(RGBA);\n";
        code += "float a1 = Val*Sat;\n";
        code += "float a2 = HueShift*3.14159265 / 180;\n";
        code += "float VSU = a1*cos(a2);\n";
        code += "float VSW = a1*sin(a2);\n";
        code += "\n";
        code += "RESULT.x = (.299*Val + .701*VSU + .168*VSW)*RGBA.x\n";
        code += "+ (.587*Val - .587*VSU + .330*VSW)*RGBA.y\n";
        code += "+ (.114*Val - .114*VSU - .497*VSW)*RGBA.z;\n";
        code += "\n";
        code += "RESULT.y = (.299*Val - .299*VSU - .328*VSW)*RGBA.x\n";
        code += "+ (.587*Val + .413*VSU + .035*VSW)*RGBA.y\n";
        code += "+ (.114*Val - .114*VSU + .292*VSW)*RGBA.z;\n";
        code += "\n";
        code += "RESULT.z = (.299*Val - .3*VSU + 1.25*VSW)*RGBA.x\n";
        code += "+ (.587*Val - .588*VSU - 1.05*VSW)*RGBA.y\n";
        code += "+ (.114*Val + .886*VSU - .203*VSW)*RGBA.z;\n";
        code += "\n";
        code += "return RESULT;\n";
        code += "}\n";

    }


    public override Node Create(Vector2 pos)
    {
        Function();

        ColorHSV node = ScriptableObject.CreateInstance<ColorHSV>();

        node.name = "Color HSV";

        node.rect = new Rect(pos.x, pos.y, 172, 300);
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_hsv.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            Variable = 180;
            Variable2 = 1;
            Variable3 = 1;
        }

        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

        // Paramaters
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
        }
        GUILayout.Label("Hue: (0 to 360) " + Variable.ToString("0."));
        Variable =HorizontalSlider(Variable, 0, 360);
       
        GUILayout.Label("Sat.: (0 to 2) " + Variable2.ToString("0.0"));
        Variable2 =HorizontalSlider(Variable2, 0, 2);

        GUILayout.Label("Bright.: (0 to 2) " + Variable3.ToString("0.0"));
        Variable3 =HorizontalSlider(Variable3, 0, 2);



    }
    private string FinalVariable;
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

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_ColorHSV_" + NodeCount;
        string DefaultNameVariable1 = "_ColorHSV_Hue_" + NodeCount;
        string DefaultNameVariable2 = "_ColorHSV_Saturation_" + NodeCount;
        string DefaultNameVariable3 = "_ColorHSV_Brightness_" + NodeCount;
        string DefaultParameters1 = ", Range(0, 360)) = 180";
        string DefaultParameters2 = ", Range(0, 2)) = 1";
        string DefaultParameters3 = ", Range(0, 2)) = 1";
        string rgba = s_in.Result;
 
        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;


        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = "float4 " + DefaultName + " = ColorHSV(" + rgba + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;

        s_out.Result = DefaultName;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
        s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}