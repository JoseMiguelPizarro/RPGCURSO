using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;


namespace _ShaderoShaderEditorFramework
{
[Node(false, "Initialize/Source Grab Pass")]

public class SourceGrabPass : Node
{
    public const string ID = "SourceGrabPass";
    public override string GetID { get { return ID; } }

    public override Node Create(Vector2 pos)
    {
        SourceGrabPass node = ScriptableObject.CreateInstance<SourceGrabPass>();
        node.name = "Source Grabpass";
        node.rect = new Rect(pos.x, pos.y, 148, 210);
        node.CreateOutput("Source", "SuperSource");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Outputs[0].DisplayLayout(new GUIContent("Source", "Source"));
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Shadero_GrabPass.jpg");
        GUI.DrawTexture(new Rect(8, 30, 130, 130), preview);
    }

    public override bool Calculate()
    {
        SuperSource s_out = new SuperSource();
        Node.GrabPassTag = true;
        s_out.StringPreviewLines = "";
        s_out.ValueLine = "";
        s_out.Result = "_GrabTexture"; 
        s_out.StringPreviewNew = s_out.StringPreviewLines+ s_out.ValueLine;
        s_out.ParametersLines = "";
        s_out.ParametersDeclarationLines = "";
        Outputs[0].SetValue<SuperSource>(s_out);
      
        return true;
    }
}
}