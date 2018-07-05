
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Tools/Generate a Sprite")]
public class GenerateTexture : Node
{
    public const string ID = "GenerateTexture";
    public override string GetID { get { return ID; } }
    public static Texture2D tex;
     public static Shader shader;
    public static int count = 1;
    public static bool tag = false;
    public static string code;
    public static int Used=2;
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
        GenerateTexture node = ScriptableObject.CreateInstance<GenerateTexture>();
        node.name = "Generate Sprite";
        node.rect = new Rect(pos.x, pos.y, 198, 350);
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
                        GUI.DrawTexture(new Rect(8, 29, 182, 182), preview);
                        
                        Material mat = Instantiate(NodeEditor._Shadero_Material);
                        mat.shader = Shader.Find(ShaderString);
                        Texture tex = mat.mainTexture;
                        if (tex == null) tex = ResourceManager.LoadTexture("Textures/previews/Preview_Shader.jpg");
                        EditorGUI.DrawPreviewTexture(new Rect(9, 30, 178, 178), tex, mat);
                       GUILayout.Space(193);
                        GUILayout.BeginHorizontal();

                            if (GUILayout.Button("Black")) backpreview = 1;
                            if (GUILayout.Button("Gray")) backpreview = 0;
                            if (GUILayout.Button("White")) backpreview = 2;
                            GUILayout.EndHorizontal();
                            string[] test = new string[5];
                            test[0] = "128";
                            test[1] = "256";
                            test[2] = "512";
                            test[3] = "1024";
                            test[4] = "2048";
                            int TextureSize = 1024;
                            Used = GUILayout.Toolbar(Used, test, GUILayout.Height(30));
                            if (Used == 0) { TextureSize = 128; }
                            if (Used == 1) { TextureSize = 256; }
                            if (Used == 2) { TextureSize = 512; }
                            if (Used == 3) { TextureSize = 1024; }
                            if (Used == 4) { TextureSize = 2048; }
                            if (GUILayout.Button(new GUIContent("Save to Sprite", "Save the result to a Sprite file")))
                            {
                                string path = EditorUtility.SaveFilePanelInProject("Generate Sprite", "NewSprite", "png", "", NodeEditor.editorPath + "Shadero_Projects/");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    RenderTexture rt = new RenderTexture(TextureSize, TextureSize, 26);
                                    Graphics.Blit(tex, rt, mat);
                                    Texture2D destination_texture = new Texture2D(TextureSize, TextureSize);
                                    RenderTexture.active = rt;
                                    destination_texture.ReadPixels(new Rect(0, 0, TextureSize, TextureSize), 0, 0);
                                    destination_texture.Apply();
                                    byte[] bs = destination_texture.EncodeToPNG();
                                    File.WriteAllBytes(path, bs);
                                    DestroyImmediate(destination_texture);
                                }
                            }
                            DestroyImmediate(mat);


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
            ShaderString = "Shadero Previews/GenerateXATXQ" + count;
            result = BuildShader.BuildVertexShader(ShaderString, s_in.StringPreviewNew, s_in.Result, s_in.ParametersLines, s_in.ParametersDeclarationLines);
            string patha = Application.dataPath + "/ShaderoSprite/Plugins/Editor/Cache/generatetextureshader" + count + ".shader";
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