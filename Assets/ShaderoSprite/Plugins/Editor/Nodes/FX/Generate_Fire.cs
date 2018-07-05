using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Generate/Effects/Fire")]
public class Generate_Fire : Node
{
    [HideInInspector]
    public const string ID = "Generate_Fire";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0f;
    [HideInInspector]
    public float Variable2 = 0f;
    [HideInInspector]
    public float Variable3 = 0.05f;
    [HideInInspector]
    public float Variable4 = 0.5f;
    [HideInInspector]
    public float Variable5 = 1f;
 

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
        code += "float Generate_Fire_hash2D(float2 x)\n";
        code += "{\n";
        code += "return frac(sin(dot(x, float2(13.454, 7.405)))*12.3043);\n";
        code += "}\n";
        code += "\n";
        code += "float Generate_Fire_voronoi2D(float2 uv, float precision)\n";
        code += "{\n";
        code += "float2 fl = floor(uv);\n";
        code += "float2 fr = frac(uv);\n";
        code += "float res = 1.0;\n";
        code += "for (int j = -1; j <= 1; j++)\n";
        code += "{\n";
        code += "for (int i = -1; i <= 1; i++)\n";
        code += "{\n";
        code += "float2 p = float2(i, j);\n";
        code += "float h = Generate_Fire_hash2D(fl + p);\n";
        code += "float2 vp = p - fr + h;\n";
        code += "float d = dot(vp, vp);\n";
        code += "res += 1.0 / pow(d, 8.0);\n";
        code += "}\n";
        code += "}\n";
        code += "return pow(1.0 / res, precision);\n";
        code += "}\n";
        code += "\n";
        code += "float4 Generate_Fire(float2 uv, float posX, float posY, float precision, float smooth, float speed, float black)\n";
        code += "{\n";
        code += "uv += float2(posX, posY);\n";
        code += "float t = _Time*60*speed;\n";
        code += "float up0 = Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(0, -t), precision);\n";
        code += "float up1 = 0.5 + Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(42, -t ) + 30.0, precision);\n";
        code += "float finalMask = up0 * up1  + (1.0 - uv.y);\n";
        code += "finalMask += (1.0 - uv.y)* 0.5;\n";
        code += "finalMask *= 0.7 - abs(uv.x - 0.5);\n";
        code += "float4 result = smoothstep(smooth, 0.95, finalMask);\n";
        code += "result.a = saturate(result.a + black);\n";
        code += "return result;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();
        Generate_Fire node = ScriptableObject.CreateInstance<Generate_Fire>();
        node.name = "Generate RGBA Fire";
        node.rect = new Rect(pos.x, pos.y, 172, 400);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateOutput("RGBA", "SuperFloat4");
   
        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_fire.jpg");
        GUI.DrawTexture(new Rect(1, 0, 228, 44), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();
  

        if (GUILayout.Button("Reset"))
        {
            Variable = 0f;
            Variable2 = 0f;
            Variable3 = 0.05f;
            Variable4 = 0.5f;
            Variable5 = 1f;
 
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
        
        GUILayout.Label("Precision (0 to 0.5) " + Variable3.ToString("0.00"));
        Variable3 =HorizontalSlider(Variable3,-0, 0.5f);
        
        GUILayout.Label("Smooth (0 to 1) " + Variable4.ToString("0.00"));
        Variable4 =HorizontalSlider(Variable4, 0, 1);
        
        GUILayout.Label("Speed (-2 to 2) " + Variable5.ToString("0.00"));
        Variable5 =HorizontalSlider(Variable5, -2, 2);
        
      
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
        string DefaultName = "_Generate_Fire_" + NodeCount;
        string DefaultNameVariable1 = "_Generate_Fire_PosX_" + NodeCount;
        string DefaultNameVariable2 = "_Generate_Fire_PosY_" + NodeCount;
        string DefaultNameVariable3 = "_Generate_Fire_Precision_" + NodeCount;
        string DefaultNameVariable4 = "_Generate_Fire_Smooth_" + NodeCount;
        string DefaultNameVariable5 = "_Generate_Fire_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 2)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 2)) = " + Variable2.ToString();
        string DefaultParameters3 = ", Range(0, 1)) = " + Variable3.ToString();
        string DefaultParameters4 = ", Range(0, 1)) = " + Variable4.ToString();
        string DefaultParameters5 = ", Range(-2, 2)) = " + Variable5.ToString();
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
        s_out.ValueLine = "float4 " + DefaultName + " = Generate_Fire(" + uv + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + "," + DefaultNameVariable3 + "," + DefaultNameVariable4 + ","+ DefaultNameVariable5 + "," + alphablack + ");\n";
        else
        s_out.ValueLine = "float4 " + DefaultName + " = Generate_Fire(" + uv + "," + Variable.ToString() + "," + Variable2.ToString() + "," + Variable3.ToString() + "," + Variable4.ToString() + "," + Variable5.ToString() + "," + alphablack + ");\n";

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