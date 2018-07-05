using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Tint Color")]
public class TintRGBA : Node
{
    [HideInInspector] public const string ID = "TintRGBA";
    [HideInInspector] public override string GetID { get { return ID; } }


    [HideInInspector]
    public bool AddParameters = true;

    [HideInInspector] public Color Variable = new Color(1,1,1,1);
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
        code += "float4 TintRGBA(float4 txt, float4 color)\n";
        code += "{\n";
        code += "float3 tint = dot(txt.rgb, float3(.222, .707, .071));\n";
        code += "tint.rgb *= color.rgb;\n";
        code += "txt.rgb = lerp(txt.rgb,tint.rgb,color.a);\n";
        code += "return txt;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {

        Function();

        TintRGBA node = ScriptableObject.CreateInstance<TintRGBA>();
        node.name = "Tint RGBA";
        node.rect = new Rect(pos.x, pos.y, 172, 190);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");
     
        return node;
    }

 

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_tint.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetColor(FinalVariable, Variable);
        }
        GUILayout.Label("Tint:");
        Variable = EditorGUILayout.ColorField("", Variable);





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
        string DefaultName = "TintRGBA_" + NodeCount;
        string DefaultNameVariable1 = "_TintRGBA_Color_" + NodeCount;
        string DefaultParameters1 = ", COLOR) = (" + Variable.r + "," + Variable.g + "," + Variable.b + "," + Variable.a + ")";
        string VoidName = "TintRGBA";
        string PreviewVariable = s_in.Result;

        FinalVariable = DefaultNameVariable1;

        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (AddParameters)
            s_out.ValueLine = "float4 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + DefaultNameVariable1 + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + ", float4(" + Variable.r.ToString() + "," + Variable.g.ToString() + "," + Variable.b.ToString() + "," + Variable.a.ToString() + ")" + ");\n";
      
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersDeclarationLines += "float4 " + DefaultNameVariable1 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}