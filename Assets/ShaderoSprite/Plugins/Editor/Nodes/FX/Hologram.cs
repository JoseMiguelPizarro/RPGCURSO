using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/FX/HologramFX")]
public class Hologram : Node
{
    [HideInInspector]
    public const string ID = "HologramFX";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 1;
    [HideInInspector]
    public float Variable2 = 1;
    [HideInInspector]
  
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public bool parametersOK = true;

    public static void Init()
    {
        tag = false;
        count = 1;
    }
    public void Function()
    {
        code = "";
        code += "inline float Holo1mod(float x,float modu)\n";
        code += "{\n";
        code += "return x - floor(x * (1.0 / modu)) * modu;\n";
        code += "}\n";
        code += "\n";
        code += "inline float Holo1noise(sampler2D source,float2 p)\n";
        code += "{\n";
        code += "float _TimeX = _Time.y;\n";
        code += "float sample = tex2D(source,float2(.2,0.2*cos(_TimeX))*_TimeX*8. + p*1.).x;\n";
        code += "sample *= sample;\n";
        code += "return sample;\n";
        code += "}\n";
        code += "\n";
        code += "inline float Holo1onOff(float a, float b, float c)\n";
        code += "{\n";
        code += "float _TimeX = _Time.y;\n";
        code += "return step(c, sin(_TimeX + a*cos(_TimeX*b)));\n";
        code += "}\n";
        code += "\n";
        code += "float4 Hologram(float2 uv, sampler2D source, float value, float speed)\n";
        code += "{\n";
        code += "float alpha = tex2D(source, uv).a;\n";
        code += "float _TimeX = _Time.y * speed;\n";
        code += "float2 look = uv;\n";
        code += "float window = 1. / (1. + 20.*(look.y - Holo1mod(_TimeX / 4., 1.))*(look.y - Holo1mod(_TimeX / 4., 1.)));\n";
        code += "look.x = look.x + sin(look.y*30. + _TimeX) / (50.*value)*Holo1onOff(4., 4., .3)*(1. + cos(_TimeX*80.))*window;\n";
        code += "float vShift = .4*Holo1onOff(2., 3., .9)*(sin(_TimeX)*sin(_TimeX*20.) + (0.5 + 0.1*sin(_TimeX*20.)*cos(_TimeX)));\n";
        code += "look.y = Holo1mod(look.y + vShift, 1.);\n";
        code += "float4 video = float4(0, 0, 0, 0);\n";
        code += "float4 videox = tex2D(source, look);\n";
        code += "video.r = tex2D(source, look - float2(.05, 0.)*Holo1onOff(2., 1.5, .9)).r;\n";
        code += "video.g = videox.g;\n";
        code += "video.b = tex2D(source, look + float2(.05, 0.)*Holo1onOff(2., 1.5, .9)).b;\n";
        code += "video.a = videox.a;\n";
        code += "video = video;\n";
        code += "float vigAmt = 3. + .3*sin(_TimeX + 5.*cos(_TimeX*5.));\n";
        code += "float vignette = (1. - vigAmt*(uv.y - .5)*(uv.y - .5))*(1. - vigAmt*(uv.x - .5)*(uv.x - .5));\n";
        code += "float noi = Holo1noise(source,uv*float2(0.5, 1.) + float2(6., 3.))*value * 3;\n";
        code += "float y = Holo1mod(uv.y*4. + _TimeX / 2. + sin(_TimeX + sin(_TimeX*0.63)), 1.);\n";
        code += "float start = .5;\n";
        code += "float end = .6;\n";
        code += "float inside = step(start, y) - step(end, y);\n";
        code += "float fact = (y - start) / (end - start)*inside;\n";
        code += "float f1 = (1. - fact) * inside;\n";
        code += "video += f1*noi;\n";
        code += "video += Holo1noise(source,uv*2.) / 2.;\n";
        code += "video.r *= vignette;\n";
        code += "video *= (12. + Holo1mod(uv.y*30. + _TimeX, 1.)) / 13.;\n";
        code += "video.a = video.a + (frac(sin(dot(uv.xy*_TimeX, float2(12.9898, 78.233))) * 43758.5453))*.5;\n";
        code += "video.a = (video.a*.3)*alpha*vignette * 2;\n";
        code += "video.a *=1.2;\n";
        code += "return video;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

        Hologram node = ScriptableObject.CreateInstance<Hologram>();

        node.name = "Hologram FX";
        node.rect = new Rect(pos.x, pos.y, 172, 250);

        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("Source", "SuperSource");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_hologram.jpg");
        GUI.DrawTexture(new Rect(1, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        Inputs[1].DisplayLayout(new GUIContent("Source", "Source"));
        parametersOK = GUILayout.Toggle(parametersOK, "Add Parameter");

        if (NodeEditor._Shadero_Material != null)
        {
            NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
            NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
        }

        GUILayout.Label("Value: (-1 to 1) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable,-1, 1);
      
        GUILayout.Label("Speed: (0 to 4) " + Variable2.ToString("0.00"));
        Variable2 =HorizontalSlider(Variable2, 0, 4);
        
    }
    private string FinalVariable;
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
        SuperSource s_in2 = Inputs[1].GetValue<SuperSource>();
        SuperFloat4 s_out = new SuperFloat4();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Hologram_" + NodeCount;
        string DefaultNameVariable1 = "_Hologram_Value_" + NodeCount;
        string DefaultNameVariable2 = "_Hologram_Speed_" + NodeCount;
        string DefaultParameters1 = ", Range(-1, 1)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(0, 4)) = " + Variable2.ToString();
        string uv = s_in.Result;
        string Source = "";

        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;

        // uv
        if (s_in2.Result == null)
        {
            Source = "_MainTex";
        }
        else
        {
            Source = s_in2.Result;
        }

        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
            if (Source == "_MainTex") uv = "i.texcoord";
            if (Source == "_GrabTexture") uv = "i.screenuv";
        }
        else
        {
            uv = s_in.Result;
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew;

        if (parametersOK)
        {
            s_out.ValueLine = "float4 " + DefaultName + " = Hologram(" + uv + "," + Source + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float4 " + DefaultName + " = Hologram(" + uv + "," + Source + "," + Variable.ToString() + "," + Variable2.ToString() + ");\n";
        }
        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines;

        s_out.Result = DefaultName;

        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

        if (parametersOK)
        { 
        s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
        s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
        s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
        }
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
         return true;
    }
}
}