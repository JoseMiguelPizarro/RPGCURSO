using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Initialize/Main Texture")]
public class MainTexture : Node
{
    public const string ID = "MainTexture";
    public override string GetID { get { return ID; } }

    [HideInInspector] public string Variable= "_MainTex_";
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
        MainTexture node = ScriptableObject.CreateInstance<MainTexture>();

        node.name = "Main Texture";
        node.rect = new Rect(pos.x, pos.y, 148, 215);
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
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/Shadero_SpriteRenderer.jpg");
        GUI.DrawTexture(new Rect(8, 30, 130, 125), preview);

        if (NodeEditor._Shadero_Material!=null)
        {
            Texture MainTex = ResourceManager.LoadTexture("Textures/previews/_maintexture.png");
            NodeEditor._Shadero_Material.SetTexture("_MainTex",MainTex);
            preview = ResourceManager.LoadTexture("Textures/previews/_maintexture.png");
            GUI.DrawTexture(new Rect(8, 30, 130, 130), preview);
         }


        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            float x = Event.current.mousePosition.x-8;
            float y = Event.current.mousePosition.y-50;
            x = x / 130;
            y = y / 130;
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

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat4 s_out = new SuperFloat4();
        string NodeCount = MemoCount.ToString();
        string DefautType = "float4";

        string DefaultVariable = "_MainTex_";
        if (Variable != "") DefaultVariable = Variable;

        string DefaultName = DefaultVariable + NodeCount;

      
        s_out.StringPreviewLines = s_in.StringPreviewNew;
        if (s_in.Result == null)
        {
            s_out.ValueLine = DefautType +" "+ DefaultName + " = tex2D(_MainTex, i.texcoord);\n";
        }
        else
        {
            s_out.ValueLine = DefautType + " " + DefaultName + " = tex2D(_MainTex," + s_in.Result + ");\n";
      
        }
  
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;

        s_out.ParametersLines += s_in.ParametersLines;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        Outputs[0].SetValue<SuperFloat4>(s_out);
        count++;

        return true;
    }
}
}