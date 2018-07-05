using UnityEngine;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Sprite Sheet Animation UV")]

public class SpriteSheetAnimationUV : Node
{
    public const string ID = "SpriteSheetAnimationUV";
    public override string GetID { get { return ID; } }
    [HideInInspector] public float Variable = 4f;
    [HideInInspector] public float Variable2 = 32f;
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
        code += "float2 SpriteSheetAnimationUV(float2 uv, float size, float speed)\n";
        code += "{\n";
        code += "uv /= size;\n";
        code += "uv.x += floor(fmod(_Time * speed, 1.0) * size) / size;\n";
        code += "uv.y -= 1/size;\n";
        code += "uv.y += (1 - floor(fmod(_Time * speed / size, 1.0) * size) / size);\n";
        code += "return uv;\n";
        code += "}\n";
    }

    public override Node Create(Vector2 pos)
    {
        Function();
        SpriteSheetAnimationUV node = ScriptableObject.CreateInstance<SpriteSheetAnimationUV>();
        node.name = "Sprite Sheet Animation UV";
        node.rect = new Rect(pos.x, pos.y, 172, 300);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("UV", "SuperFloat2");
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_spritesheetanimation.jpg");
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
        GUILayout.Label("Image Per Second: (1 to 120) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider((int)Variable2, 1, 120);

       
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
        string DefaultName = "SpriteSheetAnimationUV_";
        string DefaultNameVariable1 = DefaultName + "Size_" + NodeCount;
        string DefaultNameVariable2 = DefaultName + "Frame_" + NodeCount;
        DefaultName = DefaultName + NodeCount;
        string DefaultParameters1 = ", Range(2, 16)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(1, 120)) = " + Variable2.ToString();
        string VoidName = "SpriteSheetAnimationUV";
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