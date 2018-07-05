using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Sprite Sheet Frame UV")]

public class SpriteSheetFrameUV : Node
{
    public const string ID = "SpriteSheetFrame";
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 4f;
    [HideInInspector] public float Variable2 = 1f;
    [HideInInspector]
    public bool AddParameters = true;

    public static int count = 1;
    public static int total = 1;
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
        code += "float2 SpriteSheetFrame(float2 uv, float size, float frame)\n";
        code += "{\n";
        code += "frame = int(frame);\n";
        code += "uv /= size;\n";
        code += "uv.x += fmod(frame,size) / size;\n";
        code += "uv.y -=1/size;\n";
        code += "uv.y += 1-floor(frame / size) / size;\n";
        code += "return uv;\n";
        code += "}\n";
    }

    public override Node Create(Vector2 pos)
    {
        Function();
        SpriteSheetFrameUV node = ScriptableObject.CreateInstance<SpriteSheetFrameUV>();
        node.name = "Sprite Sheet Frame UV";
        node.rect = new Rect(pos.x, pos.y, 172, 300);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_spritesheet.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "Link with a UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "The input UV"));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            Variable = 2;
            Variable2 = 2;
     
        }

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
    

        if (NodeEditor._Shadero_Material != null) NodeEditor._Shadero_Material.SetFloat(FinalVariable1, Variable);
        if (NodeEditor._Shadero_Material != null) NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
 
        GUILayout.Label("Size: (2 to 16) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Mathf.Round(Variable), 2, 16);
        Variable = Mathf.Round(Variable);
        total = (int)Variable*(int)Variable;
        GUILayout.Label("Total Frames: " + total);
        total -= 1;
        GUILayout.Label("Frame: (0 to "+ total.ToString()+") " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider((int)Variable2, 0, total);

       
    }

    private string FinalVariable1;
    private string FinalVariable2;
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

        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat2 s_out = new SuperFloat2();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "SpriteSheetFrameUV_";
        string DefaultNameVariable1 = DefaultName + "Size_" + NodeCount;
        string DefaultNameVariable2 = DefaultName + "Frame_" + NodeCount;
        DefaultName = DefaultName + NodeCount;
        string DefaultParameters1 = ", Range(2, 16)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(0, " + total.ToString() + ")) = " + Variable2.ToString();
        string VoidName = "SpriteSheetFrame";
        string PreviewVariable = s_in.Result;
        if (PreviewVariable == null) PreviewVariable = "i.texcoord";
   
        FinalVariable1 = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;

         if (AddParameters)
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
            + ","
            + DefaultNameVariable1 + ","
            + DefaultNameVariable2 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float2 " + DefaultName + " = " + VoidName + "(" + PreviewVariable
                 + ","
                 + Variable.ToString() + ","
                 + Variable2.ToString() + ");\n";

        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;

        s_out.ParametersLines += s_in.ParametersLines;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;


        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
            s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";

            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat2>(s_out);

        count++;

        return true;
    }
}
}