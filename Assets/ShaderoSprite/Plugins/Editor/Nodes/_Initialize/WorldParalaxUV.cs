using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
    [Node(false, "Initialize/World Paralax UV")]
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
    public class WorldParalaxUV : Node
    {
        public const string ID = "WorldParalaxUV";
        public override string GetID { get { return ID; } }
        [HideInInspector]
        public int SetUsed = 0;
        [HideInInspector]
        public bool Switch = false;
        [HideInInspector]
        public float Intensity = 0.1f;
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
            WorldParalaxUV node = ScriptableObject.CreateInstance<WorldParalaxUV>();

            node.name = "World Paralax UV";
            node.rect = new Rect(pos.x, pos.y, 150, 150);
            node.CreateOutput("UV", "SuperFloat2");


            return node;
        }


        protected internal override void NodeGUI()
        {
            Outputs[0].DisplayLayout(new GUIContent("UV", "The screen UV"));

            GUILayout.Label("Intensity: "+ Intensity.ToString("0.00"));
            Intensity =HorizontalSlider(Intensity, 0, 1f);
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
            Node.WorldParalaxTag = true;

            SuperFloat2 s_out = new SuperFloat2();
            s_out.StringPreviewLines = "";
            s_out.ValueLine = "";

         

            string NodeCount = MemoCount.ToString();
            string DefaultName = "_WorldParalaxUV_" + NodeCount;
       
            s_out.ValueLine = "float2 " +DefaultName+ " = i.texcoord + float2((-unity_ObjectToWorld[0][2] * " + Intensity.ToString()+"),0);\n";
                
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