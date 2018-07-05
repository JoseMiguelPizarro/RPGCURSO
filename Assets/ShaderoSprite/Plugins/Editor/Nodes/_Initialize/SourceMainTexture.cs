using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "Initialize/Source Main Texture")]

public class SourceMainTexture : Node
{
    public const string ID = "SourceMainTexture";
    public override string GetID { get { return ID; } }

      public override Node Create(Vector2 pos)
    {
        SourceMainTexture node = ScriptableObject.CreateInstance<SourceMainTexture>();

        node.name = "Source Main Texture";
        node.rect = new Rect(pos.x, pos.y, 148, 210);
        node.CreateOutput("Source", "SuperSource");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Outputs[0].DisplayLayout(new GUIContent("Source", "Source"));
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Shadero_SpriteRenderer.jpg");
        GUI.DrawTexture(new Rect(8, 30, 130, 130), preview);
        if (NodeEditor._Shadero_Material != null)
        {
            Texture MainTex = ResourceManager.LoadTexture("Textures/previews/_maintexture.png");
            NodeEditor._Shadero_Material.SetTexture("_MainTex", MainTex);
            preview = ResourceManager.LoadTexture("Textures/previews/_maintexture.png");
            GUI.DrawTexture(new Rect(8, 30, 130, 130), preview);
        }
    }

    public override bool Calculate()
    {
        SuperSource s_out = new SuperSource();
        s_out.StringPreviewLines = "";
        s_out.ValueLine = "";
        s_out.Result = "_MainTex";
        s_out.StringPreviewNew = s_out.StringPreviewLines+ s_out.ValueLine;
        s_out.ParametersLines = "";
        s_out.ParametersDeclarationLines = "";
        
        Outputs[0].SetValue<SuperSource>(s_out);
      
        return true;
    }
}
}