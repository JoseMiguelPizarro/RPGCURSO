using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Initialize/Grab Pass")]
public class GrabPass : Node
{
    public const string ID = "GrabPass";
    public override string GetID { get { return ID; } }
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    public static void Init()
    {
        tag = false;
        count = 1;
    }

    public override Node Create(Vector2 pos)
    { 
        GrabPass node = ScriptableObject.CreateInstance<GrabPass>();

        node.name = "Grab Pass Texture";
        node.rect = new Rect(pos.x, pos.y, 150, 210);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }
    
    protected internal override void NodeGUI()
    {

        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Shadero_GrabPass.jpg");
        GUI.DrawTexture(new Rect(8, 30, 130, 130), preview);
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
      
        Node.GrabPassTag = true;

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat4 s_out = new SuperFloat4();
        string NodeCount = MemoCount.ToString();
        string DefaultName = "_GrabTexture_" + NodeCount;

        // Ajoute l'ancienne ligne complete du input dans celui du output ( il n'y a pas encore la nouvelle ligne )
        s_out.StringPreviewLines = s_in.StringPreviewNew;

              if (s_in.Result == null)
              {
                  s_out.ValueLine = "float4 " + DefaultName + " = tex2D(_GrabTexture,  i.screenuv);\n";
              }
              else
              {
                  s_out.ValueLine = "float4 " + DefaultName + " = tex2D(_GrabTexture," + s_in.Result + ");\n";

              }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;

        // Ajoute les parametres en début du shader dans un string avec les anciens;
        s_out.ParametersLines += s_in.ParametersLines;

        // Ajoute les déclarations du shader dans un string avec les anciens;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        Outputs[0].SetValue<SuperFloat4>(s_out);
        count++;
     return true;
      
    }
}
}