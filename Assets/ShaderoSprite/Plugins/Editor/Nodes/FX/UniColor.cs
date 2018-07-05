using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "RGBA/Color/Fill Color")]
public class UniColor : Node
{
    [HideInInspector] public const string ID = "UniColor";
    [HideInInspector] public override string GetID { get { return ID; } }

    [HideInInspector] public bool AddParameters = true;

    [HideInInspector] public Color Variable = new Color(1,1,1,1);

    [HideInInspector]
    public bool MoreOption;

    public static int count = 1;
    public static bool tag = false;
    public static string code;
    private Color SelectedHue;

    [HideInInspector]
    public float MoreOptionAnm;
    public static void Init()
    {
        tag = false;
        count = 1;
    }
    public void Function()
    {
        code = "";
        code += "float4 UniColor(float4 txt, float4 color)\n";
        code += "{\n";
        code += "txt.rgb = lerp(txt.rgb,color.rgb,color.a);\n";
        code += "return txt;\n";
        code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        SelectedHue = new Color(1, 0, 0, 1);
        Function();

        UniColor node = ScriptableObject.CreateInstance<UniColor>();
        node.name = "Fill Color";
        node.rect = new Rect(pos.x, pos.y, 172, 190);

        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("RGBA", "SuperFloat4");

        return node;
    }
    public float getHue(float red, float green, float blue)
    {

        float min = Mathf.Min(Mathf.Min(red, green), blue);
        float max = Mathf.Max(Mathf.Max(red, green), blue);

        float hue = 0f;
        if (max == red)
        {
            hue = (green - blue) / (max - min);

        }
        else if (max == green)
        {
            hue = 2f + (blue - red) / (max - min);

        }
        else
        {
            hue = 4f + (red - green) / (max - min);
        }

        hue = hue * 60;
        if (hue < 0) hue = hue + 360;
        hue = Mathf.Round((float)hue);
        hue /= 360;
        return hue;
    }

    public Color ColorPanel(Rect colorPanelRect, Color value)
    {
        float HueValue = 1-getHue(value.r, value.g, value.b);
         float alpha=value.a;
        // Create Big Palette
        Texture2D colorpicker = new Texture2D(2, 2);
        colorpicker.filterMode = FilterMode.Bilinear;
        colorpicker.wrapMode = TextureWrapMode.Clamp;
        Color[] rainbow = { new Color(1, 0, 0, 1),
                            new Color(1, 1, 0, 1),
                            new Color(0, 1, 0, 1),
                            new Color(0, 1, 1, 1),
                            new Color(0, 0, 1, 1),
                            new Color(1, 0, 1, 1),
                            new Color(1, 0.1f, 0.1f, 1)};
        //Color OtherColor = new Color(1, 1, 1, 1);
        colorpicker.SetPixel(0, 1, new Color(1, 1, 1, 1));
        colorpicker.SetPixel(1, 1, SelectedHue);
        colorpicker.SetPixel(0, 0, new Color(0, 0, 0, 1));
        colorpicker.SetPixel(1, 0, new Color(0, 0, 0, 1));
        colorpicker.Apply();

        // Picker color
        if (GUI.RepeatButton(colorPanelRect, ""))
        {
            Vector2 pickpos = Event.current.mousePosition;
            float aaa = pickpos.x - colorPanelRect.x;
            float bbb = pickpos.y - colorPanelRect.y;
            float aaa2 = aaa / colorPanelRect.width;
            float bbb2 = bbb / colorPanelRect.height;
            Color col = colorpicker.GetPixelBilinear(aaa2, 1-bbb2);
            value = col;
           
        }
        GUI.DrawTextureWithTexCoords(colorPanelRect, colorpicker, new Rect(0.2f, 0.25f, 0.8f, 0.5f));
        DestroyImmediate(colorpicker);

        // Create Small Palette
        //OtherColor = new Color(0, 0, 0, 1);
        // Right Color
        Texture2D GradientColor = new Texture2D(1, 7);
        GradientColor.filterMode = FilterMode.Bilinear;
        GradientColor.wrapMode = TextureWrapMode.Clamp;
        for (int p = 0; p < 7; p++)
        {
            GradientColor.SetPixel(0, p, rainbow[p]);
        }

        GradientColor.Apply();

        Rect NewRect = new Rect(colorPanelRect.x + colorPanelRect.width + colorPanelRect.width / 10, colorPanelRect.y, colorPanelRect.width / 8, colorPanelRect.height);
        // Picker color
        if (GUI.RepeatButton(NewRect, ""))
        {
            Vector2 pickpos = Event.current.mousePosition;
            float aaa = pickpos.x - NewRect.x;
            float bbb = pickpos.y - NewRect.y;
            float aaa2 = aaa / NewRect.width;
            float bbb2 = bbb / NewRect.height;
            Color col = GradientColor.GetPixelBilinear(aaa2, 1-bbb2);
            value = col;
            SelectedHue = col;
        }

        GUI.DrawTextureWithTexCoords(NewRect, GradientColor, new Rect(0f, 0f, 1f, 1f));
        DestroyImmediate(GradientColor);
        GUILayout.Space(120);
        GUILayout.Label("Alpha = " + (alpha*100).ToString("0.")+"%");
        alpha =HorizontalSlider(alpha, 0, 1);
        value.a = alpha;

        Texture2D pick1 = ResourceManager.LoadTexture("Textures/picker-1.png");
        Texture2D pick2 = ResourceManager.LoadTexture("Textures/picker-2.png");

        NewRect = new Rect(colorPanelRect.x + colorPanelRect.width + colorPanelRect.width / 10-10, colorPanelRect.y-4,34, 10);
        NewRect.y += HueValue * colorPanelRect.height;
        GUI.DrawTexture(NewRect, pick2);
        NewRect = new Rect(colorPanelRect.x - 5, colorPanelRect.y-5, 16, 16);
        GUI.DrawTexture(NewRect, pick1);


        return value;
    }
    protected internal override void NodeGUI()
    {
        tag = true;
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_fillcolor.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);

        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        Outputs[0].DisplayLayout(new GUIContent("RGBA", "RGBA"));
        GUILayout.EndHorizontal();

        AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");
     
             if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetColor(FinalVariable, Variable);
            }
         GUILayout.Label("Color:");
         Variable = EditorGUILayout.ColorField("", Variable);
       
       
    }

    private string FinalVariable;

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

        SuperFloat4 s_in = Inputs[0].GetValue<SuperFloat4>();
        SuperFloat4 s_out = new SuperFloat4();



        string NodeCount = MemoCount.ToString();
        string DefaultName = "FillColor_" + NodeCount;
        string DefaultNameVariable1 = "_FillColor_Color_" + NodeCount;
        string DefaultParameters1 = ", COLOR) = (" + Variable.r + "," + Variable.g + "," + Variable.b + "," + Variable.a + ")";
        string VoidName = "UniColor";
        string PreviewVariable = s_in.Result;

        FinalVariable = DefaultNameVariable1;

        if (s_in.Result == null)
        {
            PreviewVariable = "float4(0,0,0,1)";
        }
    
        s_out.StringPreviewLines = s_in.StringPreviewNew;

        if (AddParameters)
            s_out.ValueLine = "float4 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + "," + DefaultNameVariable1 + ");\n";
        else
            s_out.ValueLine = "float4 " + DefaultName + " = " + VoidName + "(" + PreviewVariable + ", float4(" + Variable.r.ToString() + "," + Variable.g.ToString() + "," + Variable.b.ToString() + "," + Variable.a.ToString() + ")" + ");\n";

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.Result = DefaultName;
        s_out.ParametersLines += s_in.ParametersLines;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines;
        if (AddParameters)
        { 
        s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\""+ DefaultParameters1 +"\n";
        s_out.ParametersDeclarationLines += "float4 " + DefaultNameVariable1 + ";\n";
        }
        Outputs[0].SetValue<SuperFloat4>(s_out);

        count++;
        return true;
    }
}
}