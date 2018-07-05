using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Initialize/World Position Plus UV")]
    /*
    public class testgui : GUILayout
    {
        public override float HorizontalSlider(float value, float min, float max)
        {
            float ret=0;
            return ret;
        }
    }
    */
    public class WorldPositionPlusUV : Node
    {
        public const string ID = "WorldPositionPlusUV";
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public int SetUsed = 0;
        [HideInInspector]
        public bool Switch = false;
        [HideInInspector]
        public float Intensity = 1;
        [HideInInspector]
        public float offsetX = 0;
        [HideInInspector]
        public float offsetY = 0;
        public static int count = 1;

        public static void Init()
        {
            count = 1;
        }

        public override Node Create(Vector2 pos)
        {
            WorldPositionPlusUV node = ScriptableObject.CreateInstance<WorldPositionPlusUV>();

            node.name = "World Position Plus UV";
            node.rect = new Rect(pos.x, pos.y, 150, 320);
            node.CreateOutput("UV", "SuperFloat2");


            return node;
        }


        protected internal override void NodeGUI()
        {
            Outputs[0].DisplayLayout(new GUIContent("UV", "The screen UV"));

            var text = new string[] { "Use X - Y", "Use X - Z", "Use Y - Z", "Use All" };
            SetUsed=GUILayout.SelectionGrid(SetUsed, text, 1, EditorStyles.radioButton);
            Switch = GUILayout.Toggle(Switch, "Switch");
            GUILayout.Label("Intensity: "+ Intensity.ToString("0.00"));
            Intensity =HorizontalSlider(Intensity, 0, 2);
            GUILayout.Label("Offset X: " + offsetX.ToString("0.00"));
            offsetX =HorizontalSlider(offsetX, -2, 2);
            GUILayout.Label("Offset Y: " + offsetY.ToString("0.00"));
            offsetY =HorizontalSlider(offsetY, -2, 2);
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
            Node.WorldPosTag = true;

            SuperFloat2 s_out = new SuperFloat2();
            s_out.StringPreviewLines = "";
            s_out.ValueLine = "";

         

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_WordPosPlus_" + NodeCount;
       
            if (Switch)
            {
                if (SetUsed == 0) s_out.ValueLine = "float2 " +DefaultName+ " = float2("+offsetX.ToString()+"+ i.worldPos.x * " + Intensity.ToString() + ", " + offsetX.ToString() + "+ i.worldPos.y * " + Intensity.ToString()+");\n";
                if (SetUsed == 1) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ i.worldPos.x * " + Intensity.ToString() + ", " + offsetX.ToString() + "+ i.worldPos.z * " + Intensity.ToString() + ");\n";
                if (SetUsed == 2) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ i.worldPos.y * " + Intensity.ToString() + ", " + offsetX.ToString() + "+ i.worldPos.z * " + Intensity.ToString() + ");\n";
                if (SetUsed == 3) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ ((i.worldPos.x+i.worldPos.y+i.worldPos.z)*0.334) * " + Intensity.ToString() + ", " + offsetX.ToString() + "+((i.worldPos.x+i.worldPos.y+i.worldPos.z)*0.4) * " + Intensity.ToString() + ");\n";
            }
            else
            {
                if (SetUsed == 0) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ i.worldPos.y * " + Intensity.ToString() + ", " + offsetY.ToString() + "+ i.worldPos.x * " + Intensity.ToString() + ");\n";
                if (SetUsed == 1) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ i.worldPos.z * " + Intensity.ToString() + "," + offsetY.ToString() + "+  i.worldPos.x * " + Intensity.ToString() + ");\n";
                if (SetUsed == 2) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ i.worldPos.z * " + Intensity.ToString() + ", " + offsetY.ToString() + "+ i.worldPos.y * " + Intensity.ToString() + ");\n";
                if (SetUsed == 3) s_out.ValueLine = "float2 " + DefaultName + "= float2(" + offsetX.ToString() + "+ ((i.worldPos.x+i.worldPos.y+i.worldPos.z*2)*0.55) * " + Intensity.ToString() + ", " + offsetX.ToString() + "+((i.worldPos.x+i.worldPos.y+i.worldPos.z*3)*0.55) * " + Intensity.ToString() + ");\n";

            }
            s_out.Result = DefaultName;
            s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
            s_out.ParametersLines = "";
            s_out.ParametersDeclarationLines = "";

            Outputs[0].SetValue<SuperFloat2>(s_out);
            count++;
            return true;
        }
    }
}