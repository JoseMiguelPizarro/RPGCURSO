using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Gradient/Premade Gradients")]
public class PremadeGradients : Node
{
    [HideInInspector]
    public const string ID = "PremadeGradients";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public Color Variable = new Color(0.5f, 0.5f, 0.5f, 1);
    [HideInInspector]
    public Color Variable2 = new Color(0.5f, 0.5f, 0.5f, 1);
    [HideInInspector]
    public Color Variable3 = new Color(0.8f, 0.8f, 0.8f, 1);
    [HideInInspector]
    public Color Variable4 = new Color(0.0f, 0.33f, 0.67f, 1);
    [HideInInspector]
    public float Variable5 = 0;
    [HideInInspector]
    public float Variable6 = 1;
    [HideInInspector]
    public float Variable7 = 0;
    [HideInInspector]
    public bool AddParameters = true;
    [HideInInspector]
    public int CurrentPremade;
 
   
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public string CurrentTexture = "Textures/previews/nid_premadegradient.jpg";
    public static void Init()
    {
        tag = false;
        count = 1;
    }

   

    public void Function()
    {
        code = "";
        code += "float4 Color_PreGradients(float4 rgba, float4 a, float4 b, float4 c, float4 d, float offset, float fade, float speed)\n";
        code += "{\n";
        code += "float gray = (rgba.r + rgba.g + rgba.b) / 3;\n";
        code += "gray += offset+(speed*_Time*20);\n";
        code += "float4 result = a + b * cos(6.28318 * (c * gray + d));\n";
        code += "result.a = rgba.a;\n";
        code += "result.rgb = lerp(rgba.rgb, result.rgb, fade);\n";
        code += "return result;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();
        PremadeGradients node = ScriptableObject.CreateInstance<PremadeGradients>();
        node.name = "Premade Gradients";
        node.rect = new Rect(pos.x, pos.y, 172, 530);
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        GradInit(
               new Vector3(0.5f, 0.5f, 0.5f),
               new Vector3(0.5f, 0.5f, 0.5f),
               new Vector3(0.8f, 0.8f, 0.8f),
               new Vector3(0.0f, 0.33f, 0.67f));


        return node;
    }
    public void GradInit( Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
             Variable = new Color(v1.x, v1.y, v1.z, 1);
            Variable2 = new Color(v2.x, v2.y, v2.z, 1);
            Variable3 = new Color(v3.x, v3.y, v3.z, 1);
            Variable4 = new Color(v4.x, v4.y, v4.z, 1);
            Variable5 = 0;
            Variable6 = 1;
        Variable7 = 0;

    }
    public void GradButton(string txt, Vector3 v1, Vector3 v2,Vector3 v3, Vector3 v4)
    {
        Texture2D preview = ResourceManager.LoadTexture(txt);
        if (GUILayout.Button(preview,GUIStyle.none, GUILayout.Width(162), GUILayout.Height(20)))
        {
            Variable = new Color(v1.x, v1.y, v1.z, 1);
            Variable2 = new Color(v2.x, v2.y, v2.z, 1);
            Variable3 = new Color(v3.x, v3.y, v3.z, 1);
            Variable4 = new Color(v4.x, v4.y, v4.z, 1);
            Variable5 = 0;
            Variable6 = 1;
            Variable7 = 0;
            CurrentTexture = txt;
        }

    }
    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture(CurrentTexture);
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            GradInit(
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.8f, 0.8f, 0.8f),
                new Vector3(0.0f, 0.33f, 0.67f));
        }

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        if (NodeEditor._Shadero_Material != null) NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
        if (NodeEditor._Shadero_Material != null) NodeEditor._Shadero_Material.SetFloat(FinalVariable6, Variable6);
        if (NodeEditor._Shadero_Material != null) NodeEditor._Shadero_Material.SetFloat(FinalVariable7, Variable7);
      
        GUILayout.Label("Offset: (-1 to 1) " + Variable5.ToString("0.00"));
        Variable5 =HorizontalSlider(Variable5, -1, 1);

        GUILayout.Label("Fade: (0 to 1) " + Variable6.ToString("0.00"));
        Variable6 =HorizontalSlider(Variable6, 0, 1);

        GUILayout.Label("Spped: (-2 to 2) " + Variable7.ToString("0.00"));
        Variable7 =HorizontalSlider(Variable7, -2, 2);

        GradButton("Textures/previews/nid_grd_rainbow.jpg", 
            new Vector3(0.5f, 0.5f, 0.5f), 
            new Vector3(0.5f, 0.5f, 0.5f), 
            new Vector3(0.8f, 0.8f, 0.8f), 
            new Vector3(0.0f, 0.33f, 0.67f));
        GradButton("Textures/previews/nid_grd_rainbow2.jpg",
             new Vector3(0.55f, 0.4f, 0.3f),
             new Vector3(0.5f, 0.51f, 0.35f),
             new Vector3(0.8f, 0.75f, 0.8f),
             new Vector3(0.075f, 0.33f, 0.67f));
        GradButton("Textures/previews/nid_grd_rainbowhard.jpg",
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(1.0f, 1.0f, 1.0f),
                  new Vector3(0.0f, 0.33f, 0.67f));
       GradButton("Textures/previews/nid_grd_redblue.jpg",
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.9f, 0.9f, 0.9f),
                  new Vector3(0.3f + 0.31f, 0.2f + 0.31f, 0.2f+0.31f));
        GradButton("Textures/previews/nid_grd_redyellow.jpg",
                  new Vector3(0.8f, 0.5f, 0.4f),
                  new Vector3(0.2f, 0.4f, 0.2f),
                  new Vector3(2.0f, 1.0f, 1.0f),
                  new Vector3(0.0f, 0.25f, 0.25f));
        GradButton("Textures/previews/nid_grd_yellowblue.jpg",
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(1.0f, 1.0f, 0.5f),
                  new Vector3(0.8f, 0.9f, 0.3f));
        GradButton("Textures/previews/nid_grd_bluelight.jpg",
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.55f, 0.55f, 0.55f),
                  new Vector3(0.45f, 0.45f, 0.45f),
                  new Vector3(0.0f, 0.10f, 0.20f));
        GradButton("Textures/previews/nid_grd_blueorange.jpg",
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.5f, 0.5f, 0.5f),
                  new Vector3(0.9f, 0.9f, 0.9f),
                  new Vector3(0.0f + 0.47f, 0.10f + 0.47f, 0.20f + 0.47f));
        GradButton("Textures/previews/nid_grd_bluewater.jpg",
                  new Vector3(0.55f, 0.55f, 0.55f),
                  new Vector3(0.8f, 0.8f, 0.8f),
                  new Vector3(0.29f, 0.29f, 0.29f),
                  new Vector3(0.00f + 0.54f, 0.05f + 0.54f, 0.15f + 0.54f) );
        GradButton("Textures/previews/nid_grd_fireflame.jpg",
                        new Vector3(1.0f, 0.0f, 0.13f),
                        new Vector3(0.42f, 0.95f, 0.0f),
                        new Vector3(0.99f, 0.68f, 0.99f),
                        new Vector3(0.39f, 0.39f, 1.0f));
        GradButton("Textures/previews/nid_grd_pinkdarkorange.jpg",
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(1.0f, 0.7f, 0.4f),
                        new Vector3(0.0f, 0.15f, 0.20f));
        GradButton("Textures/previews/nid_grd_pinkpastel.jpg",
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(2.0f, 1.0f, 0.0f),
                        new Vector3(0.5f, 0.2f, 0.25f));
  



        GUILayout.Space(10);
     


    }

    private string FinalVariable5;
    private string FinalVariable6;
    private string FinalVariable7;
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

        SuperFloat4 s_in= Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();

        string DefaultName = "_PremadeGradients_" + NodeCount;

        string DefaultNameVariablec1 = "";
        string DefaultNameVariablec2 = "";
        string DefaultNameVariablec3 = "";
        string DefaultNameVariablec4 = "";
        string DefaultNameVariable5 = "_PremadeGradients_Offset_" + NodeCount;
        string DefaultNameVariable6 = "_PremadeGradients_Fade_" + NodeCount;
        string DefaultNameVariable7 = "_PremadeGradients_Speed_" + NodeCount;
        string DefaultParameters5 = ", Range(-1, 1)) =" + Variable5.ToString();
        string DefaultParameters6 = ", Range(0, 1)) =" + Variable6.ToString();
        string DefaultParameters7 = ", Range(-2, 2)) =" + Variable7.ToString();

        string rgba = s_in.Result;

        FinalVariable5 = DefaultNameVariable5;
        FinalVariable6 = DefaultNameVariable6;
        FinalVariable7 = DefaultNameVariable7;


        if (s_in.Result == null)
        {
            rgba = "float4(0,0,0,1)";
        }
      

        s_out.StringPreviewLines = s_in.StringPreviewNew;

        string tmpVariable5 = DefaultNameVariable5;
        string tmpVariable6 = DefaultNameVariable6;
        string tmpVariable7 = DefaultNameVariable7;
        if (!AddParameters)
        {
           tmpVariable5 = Variable5.ToString();
           tmpVariable6 = Variable6.ToString();
           tmpVariable7 = Variable7.ToString();
        }


        DefaultNameVariablec1 = "float4(" + Variable.r + "," + Variable.g + "," + Variable.b + "," + Variable.a + ")";
        DefaultNameVariablec2 = "float4(" + Variable2.r + "," + Variable2.g + "," + Variable2.b + "," + Variable2.a + ")";
        DefaultNameVariablec3 = "float4(" + Variable3.r + "," + Variable3.g + "," + Variable3.b + "," + Variable3.a + ")";
        DefaultNameVariablec4 = "float4(" + Variable4.r + "," + Variable4.g + "," + Variable4.b + "," + Variable4.a + ")";

        s_out.ValueLine = "float4 " + DefaultName + " = Color_PreGradients(" + rgba + "," + DefaultNameVariablec1 + "," + DefaultNameVariablec2 + "," + DefaultNameVariablec3 + "," + DefaultNameVariablec4 + "," + tmpVariable5.ToString()+ "," + tmpVariable6.ToString()+ "," + tmpVariable7.ToString() + ");\n";
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;

        if (AddParameters)
        {
            s_out.ParametersLines += DefaultNameVariable5 + "(\"" + DefaultNameVariable5 + "\"" + DefaultParameters5 + "\n";
            s_out.ParametersLines += DefaultNameVariable6 + "(\"" + DefaultNameVariable6 + "\"" + DefaultParameters6 + "\n";
            s_out.ParametersLines += DefaultNameVariable7 + "(\"" + DefaultNameVariable7 + "\"" + DefaultParameters7 + "\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable6 + ";\n";
            s_out.ParametersDeclarationLines += "float " + DefaultNameVariable7 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}