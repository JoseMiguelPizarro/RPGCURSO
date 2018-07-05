
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Preview Texture")]
public class PreviewTexture : Node
{
    public const string ID = "PreviewTexture";
    public override string GetID { get { return ID; } }
    public static Texture2D tex;
     public static Shader shader;
    public static int count = 1;
    public static bool tag = false;
    public static string code;
    [HideInInspector]
    [Multiline(150)]
    public string result;

    [HideInInspector]
    public int backpreview=0;

    public static void Init()
    {
        tag = false;
        count = 1;
    }
    [HideInInspector]
    public string ShaderString;
    public override Node Create(Vector2 pos)
    {
        PreviewTexture node = ScriptableObject.CreateInstance<PreviewTexture>();
        node.name = "Preview Texture";
        node.rect = new Rect(pos.x, pos.y, 168, 253);
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");
        return node;

    }
    
    protected internal override void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        if (Outputs[0].GetNodeAcrossConnection()==null) Outputs[0].knobTexture= ResourceManager.GetTintedTexture("Textures/Out_Knob.png", new Color(0.50f,0.50f,0.50f,0.75f)); else
        Outputs[0].knobTexture = ResourceManager.GetTintedTexture("Textures/Out_Knob.png", Color.white);
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        if (s_in.Result != null)
        {
            if (Node.ShaderNameX != "")
            {
                if (ShaderString != "")
                {
                    if (Shader.Find(ShaderString) != null)
                    {
                        string bp = "Textures/previews/Preview_Shader.jpg";
                        if (backpreview == 1) bp = "Textures/previews/Preview_Shader2.jpg";
                        if (backpreview == 2) bp = "Textures/previews/Preview_Shader3.jpg";
                        Texture2D preview = ResourceManager.LoadTexture(bp);
                        GUI.DrawTexture(new Rect(8, 29, 152, 152), preview);
                        
                        Material mat = Instantiate(NodeEditor._Shadero_Material);
                        mat.shader = Shader.Find(ShaderString);
                        Texture tex = mat.mainTexture;
                        if (tex == null) tex = ResourceManager.LoadTexture("Textures/previews/Preview_Shader.jpg");
                        EditorGUI.DrawPreviewTexture(new Rect(9, 30, 150, 150), tex, mat);
                        DestroyImmediate(mat);
                        GUILayout.Space(157);
                        GUILayout.BeginHorizontal();
                       
                        if (GUILayout.Button("Black")) backpreview = 1;
                        if (GUILayout.Button("Gray")) backpreview = 0;
                        if (GUILayout.Button("White")) backpreview = 2;

                        GUILayout.EndHorizontal();
                    }
                }
            }

           
        }
    }
    private string FinalVariable;


    public override bool Calculate()
    {
        // RGBA
        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();

        if (s_in.Result != null)
        {
            ShaderString = "Shadero Previews/PreviewXATXQ" + count;
            result = BuildShader.BuildVertexShader(ShaderString, s_in.StringPreviewNew, s_in.Result, s_in.ParametersLines, s_in.ParametersDeclarationLines);
            string patha = Application.dataPath + "/ShaderoSprite/Plugins/Editor/Cache/previewshader" + count + ".shader";
            File.WriteAllText(patha, result);
        }

        SuperFloat4 s_out = new SuperFloat4();
        string PreviewVariable = s_in.Result;
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        s_out.ValueLine = "";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = PreviewVariable;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}