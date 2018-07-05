using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Effects/Starfield")]
public class Generate_Starfield : Node
{
    [HideInInspector]
    public const string ID = "Generate_Starfield";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0.5f;
    [HideInInspector]
    public float Variable2 = 0.5f;
    [HideInInspector]
    public float Variable3 = 5f;
    [HideInInspector]
    public float Variable4 = 2f;
    [HideInInspector]
    public float Variable5 = 5f;
 

    [HideInInspector]
    public bool AddParameters = true;
    [HideInInspector]
    public bool NoAlphaBlack = false;
  
    public static int count = 1;
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
            code += "float4 Generate_Starfield(float2 uv, float posx, float posy, float number, float size, float speed, float black)\n";
            code += "{\n";
            code += "float2 position = uv-float2(posx,posy);\n";
            code += "float spx = speed * _Time;\n";
            code += "float angle = atan2(position.y, position.x) / (size*3.14159265359);\n";
            code += "angle -= floor(angle);\n";
            code += "float rad = length(position);\n";
            code += "float color = 0.0;\n";
             code += "float a = angle * 360;\n";
            code += "float aF = frac(a);\n";
            code += "float aR = floor(a) + 1.;\n";
            code += "float aR1 = frac(aR*frac(aR*.7331)*54.3);\n";
            code += "float aR2 = frac(aR*frac(aR*.82345)*12.345);\n";
            code += "float t = spx + aR1*100.;\n";
            code += "float radDist = sqrt(aR2 + float(0));\n";
            code += "float adist = radDist / rad;\n";
            code += "float dist = (t + adist);\n";
            code += "dist = abs(frac(dist) - 0.15);\n";
            code += "color += max(0., .5 - dist*100. / adist)*(.5 - abs(aF - .5))*number / adist / radDist;\n";
            code += "angle = frac(angle);\n";
            code += "float4 result = color;\n";
            code += "result.a = saturate(color + black);\n";
            code += "return result;\n";
            code += "}\n";
        }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Starfield node = ScriptableObject.CreateInstance<Generate_Starfield>();
        node.name = "Generate RGBA Starfield";
        node.rect = new Rect(pos.x, pos.y, 172, 400);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");
   
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_starfield.jpg");
        GUI.DrawTexture(new Rect(1, 0, 228, 44), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
  

        if (GUILayout.Button("Reset"))
        {
            Variable = 0.5f;
            Variable2 = 0.5f;
            Variable3 = 5f;
            Variable4 = 2f;
            Variable5 = 5f;
 
        }

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable3, Variable3);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable4, Variable4);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable5, Variable5);
         }

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        GUILayout.Label("Pos X (-1 to 2) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -1, 2);
    
        GUILayout.Label("Pos Y (-1 to 2) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, -1, 2);
        
        GUILayout.Label("Number (0 to 20) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3, 0, 20f);
        
        GUILayout.Label("Size (0 to 16) " + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, 0.01f, 16);
        
        GUILayout.Label("Speed -100/100: " + Variable5.ToString("0.00"));
        Variable5 =HorizontalSlider(Variable5, -100, 100);
        
      
        NoAlphaBlack = GUILayout.Toggle(NoAlphaBlack, "No Alpha = Black");
       


    }
    private string FinalVariable;
    private string FinalVariable2;
    private string FinalVariable3;
    private string FinalVariable4;
    private string FinalVariable5;
    private string FinalVariable6;
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
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Generate_Starfield_" + NodeCount;
        string DefaultNameVariable1 = "_Generate_Starfield_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_Generate_Starfield_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_Generate_Starfield_Number_" + NodeCount;
        string DefaultNameVariable4 = "_Generate_Starfield_Size_" + NodeCount;
        string DefaultNameVariable5 = "_Generate_Starfield_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 2)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(0, 20)) = " + Variable3.ToString();
        string DefaultParameters4 = ", Range(0.01, 16)) = " + Variable4.ToString();
        string DefaultParameters5 = ", Range(-100, 100)) = " + Variable5.ToString();
        string uv = s_in.Result;
       
        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;
        FinalVariable3 = DefaultNameVariable3;
        FinalVariable4 = DefaultNameVariable4;
        FinalVariable5 = DefaultNameVariable5;
    


        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
        }
        else
        {
            uv = s_in.Result;
        }
        string alphablack = "0";
        if (NoAlphaBlack) alphablack = "1";
        s_out.StringPreviewLines = s_in.StringPreviewNew ;
        if (AddParameters)
        s_out.ValueLine = "float4 " + DefaultName + " = Generate_Starfield(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + ","+ DefaultNameVariable5 + "," + alphablack + ");\n";
        else
        s_out.ValueLine = "float4 " + DefaultName + " = Generate_Starfield(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + "," + alphablack + ");\n";

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (AddParameters)
        { 
        s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
        s_out.ParametersLines += DefaultNameVariable3 + "(\"" + DefaultNameVariable3 + "\"" + DefaultParameters3 + "\n";
        s_out.ParametersLines += DefaultNameVariable4 + "(\"" + DefaultNameVariable4 + "\"" + DefaultParameters4 + "\n";
        s_out.ParametersLines += DefaultNameVariable5 + "(\"" + DefaultNameVariable5 + "\"" + DefaultParameters5 + "\n";
   
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable3 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable4 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable5 + ";\n";
        }

        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}