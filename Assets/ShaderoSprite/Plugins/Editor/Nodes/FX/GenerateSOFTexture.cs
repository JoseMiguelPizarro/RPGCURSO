using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Organic Shader/Generate SOF Texture")]
public class GenerateSOFTexture : Node
{
    [HideInInspector]
    public const string ID = "GenerateSOFTexture";
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
            code += "float4 GenerateSOFTexture(float4 t1, float4 t2, float4 t3)\n";
            code += "{\n";
            code += "return float4(t1.r, t2.r, t3.r, 1);\n";
            code += "}\n";
        }


        public override Node Create(Vector2 pos)
    {
        Function();

            GenerateSOFTexture node = ScriptableObject.CreateInstance<GenerateSOFTexture>();

        node.name = "Generate SOF Texture";

        node.rect = new Rect(pos.x, pos.y, 180, 200);
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_ghost.jpg");
        GUI.DrawTexture(new Rect(2, 0, 180, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGB Breath", "RGBA"));
        Inputs[1].DisplayLayout(new GUIContent("RGB Side Left", "RGBA"));
        Inputs[2].DisplayLayout(new GUIContent("RGB Side Right", "RGBA"));
            if (GUI.Button(new Rect(110, 0, 60, 30), "Help"))
            {
                Application.OpenURL("https://forum.vetasoft.store/post/179");
            }
        }
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
        SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
        SuperFloat4 s_in3 = Inputs[2].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_GhostFX_" + NodeCount;
        string rgba = s_in.Result;
        string rgba2 = s_in2.Result;
        string rgba3 = s_in3.Result;
   
        if (s_in.Result == null) rgba = "float4(0,0,0,1)";
        if (s_in2.Result == null) rgba2 = "float4(0,0,0,1)";
        if (s_in3.Result == null) rgba3 = "float4(0,0,0,1)";
 
        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew + s_in3.StringPreviewNew;

        s_out.ValueLine = "float4 " + DefaultName + " = GenerateSOFTexture(" + rgba + "," + rgba2 + "," + rgba3 + ");\n";
      
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines + s_in3.ParametersLines;
        s_out.Result = DefaultName;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines + s_in3.ParametersDeclarationLines;
     
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}