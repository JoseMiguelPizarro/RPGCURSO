using UnityEngine;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "Tools/Lighting Support")]
public class LightingSupport : Node
{
    [HideInInspector]
    public const string ID = "LightingSupport";
    [HideInInspector]
    public override string GetID { get { return ID; } }

    [HideInInspector]
    public bool parametersOK = true;

    [HideInInspector]
    public float Variable = 1;

        [HideInInspector]
        public bool Light = false;

        [HideInInspector]
    public float InternalCount = -1;

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

     
            LightingSupport node = ScriptableObject.CreateInstance<LightingSupport>();

            node.name = "Lighting Support";
            node.rect = new Rect(pos.x, pos.y, 200, 300);

            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateInput("RGBA", "SuperFloat4");
            node.CreateOutput("RGBA", "SuperFloat4");

            return node;
    }



    protected internal override void NodeGUI()
    {
        
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_lightingsupport.jpg");
        GUI.DrawTexture(new Rect(2, 0, 200, 70), preview);
        GUILayout.Space(80);
     
            GUILayout.BeginHorizontal();
            Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
            GUILayout.EndHorizontal();
            Inputs[1].DisplayLayout(new GUIContent("Normal Map", "RGBA"));
  
            
            if (Outputs[0].GetNodeAcrossConnection() == null)
            {
                Light = false;
            }
            else
            {
                Light = true;
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
            NormalMapTag = true;
            LightingSupportTag = Light;
            SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
            SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
            SuperFloat4 s_out = new SuperFloat4();

            string PreviewVariable2 = s_in2.Result;

            if (s_in2.Result == null) PreviewVariable2 = "float4(1,1,0,1)";

         

            s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;
   
            if (s_in.Result == null)
            {
                Node.NormalMapOutput = "";
            }
            else
            {
                Node.NormalMapOutput = "o.Normal = UnpackNormal(" + PreviewVariable2 + ");\n";
            }

            s_out.Result = s_in.Result;
            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;
            s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            Outputs[0].SetValue<SuperFloat4>(s_out);

        return true;
    }
}
}