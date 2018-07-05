using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "Initialize/World Position UV")]

public class WorldPositionUV : Node
{
    public const string ID = "WorldPositionUV";
    public override string GetID { get { return ID; } }
    public override Node Create(Vector2 pos)
    {
        WorldPositionUV node = ScriptableObject.CreateInstance<WorldPositionUV>();

        node.name = "World Position UV";
        node.rect = new Rect(pos.x, pos.y, 150, 80);
        node.CreateOutput("UV", "SuperFloat2");
        
        
        return node;
    }

    protected internal override void NodeGUI()
    {
        Outputs[0].DisplayLayout(new GUIContent("UV", "The screen UV"));

           }

        public override bool Calculate()
    {
        Node.WorldPosTag = true;
   
        SuperFloat2 s_out = new SuperFloat2();
        s_out.StringPreviewLines = "";
        s_out.ValueLine = "";
        s_out.Result = "i.worldPos";
        s_out.StringPreviewNew = s_out.StringPreviewLines+ s_out.ValueLine;
        s_out.ParametersLines = "";
        s_out.ParametersDeclarationLines = "";
        
        Outputs[0].SetValue<SuperFloat2>(s_out);
      
        return true;
    }
}
}