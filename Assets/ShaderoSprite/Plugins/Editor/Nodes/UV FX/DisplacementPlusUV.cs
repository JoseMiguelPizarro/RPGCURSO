using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Displacement Plus UV")]
public class DisplacementPlusUV : Node
{
    [HideInInspector]
    public const string ID = "DisplacementPlusUV";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0;
    [HideInInspector]
    public float Variable2 = 0;
    [HideInInspector]
    public float Variable3 = 1;
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public bool AddParameters = true;

    public static void Init()
    {
        tag = false;
        count = 1;
    }

    public void Function()
    {
        code = "";
        code += "float4 DisplacementPlusUV(float2 uv,sampler2D source,float4 rgba ,float4 rgba2,float x, float y, float value)\n";
        code += "{\n";
        code += "float r=(rgba2.r+rgba2.g+rgba2.b)/3;\n";
        code += "r*=rgba2.a;\n";
        code += "return tex2D(source,lerp(uv,uv+float2(rgba.r*x,rgba.g*y),value*r));\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        DisplacementPlusUV node = ScriptableObject.CreateInstance<DisplacementPlusUV>();

        node.name = "Displacement Plus UV FX";
        node.rect = new Rect(pos.x, pos.y, 172, 360);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("Source", "SuperSource");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_displacement.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));
        Inputs[2].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Inputs[3].DisplayLayout(new GUIContent("RGBA (Mask)", "RGBA"));
        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
        }

        if (GUILayout.Button("Reset"))
        {
            Variable = 0;
            Variable2 = 0;
            Variable3 = 1;
          

        }
        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
      
   
        GUILayout.Label("Pos X: (-1 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -1, 1);
       
        GUILayout.Label("Pos Y: (-1 to 1) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -1, 1);

        GUILayout.Label("Value: (-3 to 3) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, -3, 3);


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
        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
        SuperFloat4 s_in3 = Inputs[2].GetValue<SuperFloat4>();
        SuperFloat4 s_in4 = Inputs[3].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Displacement_Plus_" + NodeCount;
        string DefaultNameVariable1 = "_Displacement_Plus_ValueX_" + NodeCount;
        string DefaultNameVariable2 = "_Displacement_Plus_ValueY_" + NodeCount;
        string DefaultNameVariable3 = "_Displacement_Plus_Size_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 1)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(-3, 3)) = " + Variable3.ToString();
        string uv = s_in.Result;
        string RGBA = s_in3.Result;
        string RGBA2 = s_in4.Result;
        string Source = "";

        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;

        // uv
        if (s_in2.Result == null)
        {
            Source = "_MainTex";
        }
        else
        {
            Source = s_in2.Result;
        }

        if (s_in3.Result == null)
        {
            RGBA = "float4(0,0,0,1)";
        }
        else
        {
            RGBA = s_in3.Result;
        }

        if (s_in4.Result == null)
        {
            RGBA2 = "float4(1,1,1,1)";
        }
        else
        {
            RGBA2 = s_in4.Result;
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

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew + s_in3.StringPreviewNew + s_in4.StringPreviewNew;

        if (AddParameters)
        {
                s_out.ValueLine = "float4 " + DefaultName + " = DisplacementPlusUV(" + uv + "," + Source + "," + RGBA + "," + RGBA2 + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + ");\n";
        }
        else
        {
                s_out.ValueLine = "float4 " + DefaultName + " = DisplacementPlusUV(" + uv + "," + Source + "," + RGBA + "," + RGBA2 + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + ");\n";
        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines + s_in3.ParametersLines + s_in4.ParametersLines;

        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines + s_in3.ParametersDeclarationLines + s_in4.ParametersDeclarationLines;

        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
            s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
        }
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}