using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace _ShaderoShaderEditorFramework
{
    public abstract class Node : ScriptableObject
    {
        [HideInInspector]
        public Rect rect = new Rect();
        [HideInInspector]
        internal Vector2 contentOffset = Vector2.zero;
        [HideInInspector]
        [SerializeField]
        public List<NodeKnob> nodeKnobs = new List<NodeKnob>();

        [HideInInspector]
        [SerializeField]
        public List<NodeInput> Inputs = new List<NodeInput>();
        [HideInInspector]
        [SerializeField]
        public List<NodeOutput> Outputs = new List<NodeOutput>();
        [HideInInspector]
        [NonSerialized]
        internal bool calculated = true;

        private static bool RecalculateFlag;      
        public static string ShaderNameX = "";     
        public static bool GrabPassTag = false;
        public static bool WorldPosTag = false;
        public static bool WorldParalaxTag = false;
        public static bool ErrorTag = false;
        public static bool NormalMapTag = false;
        public static bool LightingSupportTag = false;
        public static string NormalMapOutput = "";


        #region General

        public static void InitValueCount()
        {


            NormalMapTag = false;
            LerpUV.Init();
            AutomaticLerp.Init();
            WorldPositionPlusUV.Init();
            WorldParalaxUV.Init();
            HolographicParalax.Init();
            HumanBreathUV.Init();
            HumanBreathExUV.Init();
            FlipUV_H.Init();
            GenerateSOFTexture.Init();
            FlipUV_V.Init();
            RotationUV.Init();
            GrayScale.Init();
            Blur.Init();
            AnimatedPingPongOffsetUV.Init();
            DisplacementPlusUV.Init();
            AnimatedOffsetUV.Init();
            RGBA_Div.Init();
            RGBA_Add.Init();
            RGBA_Mul.Init();
            RGBA_Sub.Init();
            Generate_LightningFX.Init();
            MainTexture.Init();
            NewTexture.Init();
            SourceNewTexture.Init();
            OperationBlend.Init();
            OperationAdd.Init();
            OperationDiv.Init();
            OperationLerp.Init();
            OperationMul.Init();
            OperationSub.Init();
            MaskSprite.Init();
            MaskRGBA.Init();
            MaskChannel.Init();
            SourceRGBA.Init();
            GrabPassTag = false;
            DistortionUV.Init();
            OffsetUV.Init();
            GrabPass.Init();
            RGBA2Split.Init();
            Split2RGBA.Init();
            Desintegration.Init();
            Destroyer.Init();
            BurnFX.Init();
            PlasmaFX.Init();
            RGBA2UV.Init();
            UV2RGBA.Init();
            PreviewTexture.Init();
            OutlineFX.Init();
            UniColor.Init();
            BlurHQ.Init();
            CompressionFX.Init();
            OutlineEmpty.Init();
            Hologram.Init();
            ClippingLeft.Init();
            ClippingRight.Init();
            ClippingUp.Init();
            ClippingDown.Init();
            ColorRGBA.Init();
            TintRGBA.Init();
            InverseColor.Init();
            Threshold.Init();
            Brightness.Init();
            Darkness.Init();
            TwistUV.Init();
            AnimatedTwistUV.Init();
            ZoomUV.Init();
            PixelXYUV.Init();
            ColorHSV.Init();
            GhostFX.Init();
            CircleFade.Init();
            FourGradients.Init();
            ThresholdSmooth.Init();
            TiltUpUV.Init();
            TiltDownUV.Init();
            TiltLeftUV.Init();
            TiltRightUV.Init();
            Generate_Circle.Init();
            Generate_Shape.Init();
            Generate_Pyramid.Init();
            Generate_Checker.Init();
            Generate_Fire.Init();
            PremadeGradients.Init();
            ColorGradients.Init();
            AnimatedMouvementUV.Init();
            DisplacementUV.Init();
            OperationBlend.Init();
            PatternMovement.Init();
            OperationBlendMask.Init();
            PatternMovementMask.Init();
            KaleidoscopeUV.Init();
            TurnAlphaToBlack.Init();
            RenderedTexture.Init();
            TurnBlackToAlpha.Init();
            ResizeUV.Init();
            SpriteSheetFrameUV.Init();
            SpriteSheetAnimationUV.Init();
            Generate_Xray.Init();
            Generate_Donuts.Init();
            Generate_Line.Init();
            Generate_Noise.Init();
            PixelUV.Init();
            FishEyeUV.Init();
            PinchUV.Init();
            TurnMetal.Init(); 
            TurnGold.Init();
            TurnTransparent.Init();
            HdrControlValue.Init();
            HdrCreate.Init();
            PositionUV.Init();
            FadeToAlpha.Init();
            GenerateTexture.Init();
            SuperGrayScale.Init();
            ColorFilters.Init();
            DisplacementPack.Init();
            PatternMovementPack.Init();
            AnimatedZoomUV.Init();
            InnerGlow.Init();
            Generate_Spiral.Init();
            SimpleDisplacementUV.Init();
            SpriteSheetUVAnimPack.Init();
            MorphingPack.Init();
            MorphingPerfectPack.Init();
            AnimatedRotationUV.Init();
            AlphaAsAura.Init();
            FixUV.Init();
            AlphaIntensity.Init();
            Generate_Starfield.Init();
            MultipleUV.Init();
            TurnGB.Init();
            TurnC64.Init();
            TurnEGA.Init();
            TurnCGA.Init();
            AnimatedInfiniteZoomUV.Init();
            TurnCGA2.Init();
            Make3DFX.Init();
            BlurHQPlus.Init();
            Emboss.Init();
            EmbossFull.Init();
            Sharpness.Init();
            MotionBlurFast.Init();
            ShadowLight.Init();
            ShinyFX.Init();
            ShinyOnlyFX.Init();
            PlasmaLightFX.Init();
            DivUV.Init();
            SubUV.Init();
            AddUV.Init();
            CircleHole.Init();
            MaskAlpha.Init();
            DoodleUV.Init();
            LiquidUV.Init();
        }


        public float HorizontalSlider(float v, float a, float b)
        {
            GUILayout.BeginHorizontal();
            string txt = "";
            v = GUILayout.HorizontalSlider(v, a, b, GUILayout.Width(120));
            txt = GUILayout.TextField(v.ToString("0.000"), GUILayout.Width(42));
            float result = v;
            float.TryParse(txt, out result);
            v = result;

           
            GUILayout.EndHorizontal();
            return v;
        }

        public float HorizontalSlider(float v, float a, float b, GUILayoutOption gs)
        {
            GUILayout.BeginHorizontal();
            string txt = "";
            v = GUILayout.HorizontalSlider(v, a, b, GUILayout.Width(120));
            txt = GUILayout.TextField(v.ToString("0.000"), GUILayout.Width(42));
            float result = v;
            float.TryParse(txt, out result);
            v = result;
            GUILayout.EndHorizontal();
            return v;
        }
        public float HorizontalSlider(float v, float a, float b, GUILayoutOption gs, float numb)
        {
            GUILayout.BeginHorizontal();
            string txt = "";
            v = GUILayout.HorizontalSlider(v, a, b, GUILayout.Width(numb-42));
            txt = GUILayout.TextField(v.ToString("0.000"), GUILayout.Width(38));
            float result = v;
            float.TryParse(txt, out result);
            v = result;
            GUILayout.EndHorizontal();
            return v;
        }
        protected internal void InitBase()
        {
            NodeEditor.RecalculateFrom(this);
            if (!NodeEditor.curNodeCanvas.nodes.Contains(this))
                NodeEditor.curNodeCanvas.nodes.Add(this);
#if UNITY_EDITOR
            if (String.IsNullOrEmpty(name))
                name = UnityEditor.ObjectNames.NicifyVariableName(GetID);
#endif
            NodeEditor.RepaintClients();
        }

          public void Delete()
        {
            if (!NodeEditor.curNodeCanvas.nodes.Contains(this))
                throw new UnityException("The Node " + name + " does not exist on the Canvas " + NodeEditor.curNodeCanvas.name + "!");
            NodeEditorCallbacks.IssueOnDeleteNode(this);
            NodeEditor.curNodeCanvas.nodes.Remove(this);
            for (int outCnt = 0; outCnt < Outputs.Count; outCnt++)
            {
                NodeOutput output = Outputs[outCnt];
                while (output.connections.Count != 0)
                    output.connections[0].RemoveConnection();
                DestroyImmediate(output, true);
            }
            for (int inCnt = 0; inCnt < Inputs.Count; inCnt++)
            {
                NodeInput input = Inputs[inCnt];
                if (input.connection != null)
                    input.connection.connections.Remove(input);
                DestroyImmediate(input, true);
            }
            for (int knobCnt = 0; knobCnt < nodeKnobs.Count; knobCnt++)
            {  if (nodeKnobs[knobCnt] != null)
                    DestroyImmediate(nodeKnobs[knobCnt], true);
            }
            DestroyImmediate(this, true);
        }

         public static Node Create(string nodeID, Vector2 position)
        {
            return Create(nodeID, position, null);
        }

         public static Node Create(string nodeID, Vector2 position, NodeOutput connectingOutput)
        {
            Node node = NodeTypes.getDefaultNode(nodeID);
            if (node == null)
                throw new UnityException("Cannot create Node with id " + nodeID + " as no such Node type is registered!");

            node = node.Create(position);
            node.InitBase();

            if (connectingOutput != null)
            {   foreach (NodeInput input in node.Inputs)
                {
                    if (input.TryApplyConnection(connectingOutput))
                        break;
                }
            }

            NodeEditorCallbacks.IssueOnAddNode(node);

            return node;
        }

         internal void CheckNodeKnobMigration()
        {
            if (nodeKnobs.Count == 0 && (Inputs.Count != 0 || Outputs.Count != 0))
            {
                nodeKnobs.AddRange(Inputs.Cast<NodeKnob>());
                nodeKnobs.AddRange(Outputs.Cast<NodeKnob>());
            }
        }

        #endregion

        #region Dynamic Members

        #region Node Type Methods

        public abstract string GetID { get; }
        public abstract Node Create(Vector2 pos);
        protected internal abstract void NodeGUI();
        public virtual void DrawNodePropertyEditor() { }
        public virtual bool Calculate() { return true; }
        public virtual bool FixCalculate() { return true; }

        #endregion

        #region Node Type Properties

        public virtual bool AllowRecursion { get { return false; } }
        public virtual bool ContinueCalculation { get { return true; } }

        #endregion

        #region Protected Callbacks

        protected internal virtual void OnDelete() { }
        protected internal virtual void OnAddInputConnection(NodeInput input) { }
        protected internal virtual void OnAddOutputConnection(NodeOutput output) { }

        #endregion

        #region Additional Serialization

        public virtual ScriptableObject[] GetScriptableObjects() { return new ScriptableObject[0]; }
        protected internal virtual void CopyScriptableObjects(System.Func<ScriptableObject, ScriptableObject> replaceSerializableObject) { }
        public void SerializeInputsAndOutputs(System.Func<ScriptableObject, ScriptableObject> replaceSerializableObject) { }

        #endregion

        #endregion

        #region Drawing

#if UNITY_EDITOR
        public virtual void OnSceneGUI()
        {

        }
#endif

         protected internal virtual void DrawNode()
        {
            Rect nodeRect = rect;
            nodeRect.position += NodeEditor.curEditorState.zoomPanAdjust + NodeEditor.curEditorState.panOffset;
            contentOffset = new Vector2(0, 40);
            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, contentOffset.y);
            GUI.Label(headerRect, name, NodeEditor.curEditorState.selectedNode == this ? NodeEditorGUI.nodeBoxTitleBold : NodeEditorGUI.nodeBoxTitle);


       

            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y + contentOffset.y, nodeRect.width, nodeRect.height - contentOffset.y);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect, GUI.skin.box);
            GUI.changed = false;
            NodeGUI();
            GUILayout.EndArea();
            GUI.EndGroup();
            bodyRect = new Rect(nodeRect.x, nodeRect.y + contentOffset.y, nodeRect.width * 2, (nodeRect.height) * 2);
            bodyRect.x -= (nodeRect.width / 2);
            bodyRect.y -= (nodeRect.height / 2) + contentOffset.y;
            GUI.color = NodeEditor.curEditorState.selectedNode == this ? new Color(0.6f, 0.6f, 1f, 0.25f) : new Color(1f, 1f, 1f, 0.075f);
            GUI.DrawTexture(bodyRect, NodeEditorGUI.GUIBoxLight);
            GUI.color = Color.white;
        }

        protected internal virtual void DrawKnobs()
        {
            CheckNodeKnobMigration();
            for (int knobCnt = 0; knobCnt < nodeKnobs.Count; knobCnt++)
                nodeKnobs[knobCnt].DrawKnob();
        }
        protected internal virtual void DrawConnections()
        {
            CheckNodeKnobMigration();
            if (Event.current.type != EventType.Repaint)
                return;
            for (int outCnt = 0; outCnt < Outputs.Count; outCnt++)
            {
                NodeOutput output = Outputs[outCnt];
                Vector2 startPos = output.GetGUIKnob().center;
                Vector2 startDir = output.GetDirection();

                for (int conCnt = 0; conCnt < output.connections.Count; conCnt++)
                {
                    NodeInput input = output.connections[conCnt];
                    NodeEditorGUI.DrawConnection(startPos,
                                                    startDir,
                                                    input.GetGUIKnob().center,
                                                    input.GetDirection(),
                                                    output.typeData.Color);
                }
            }
        }

        #endregion

        #region Calculation Utility

        protected internal bool allInputsReady()
        {
            for (int inCnt = 0; inCnt < Inputs.Count; inCnt++)
            {
                if (Inputs[inCnt].connection == null || Inputs[inCnt].connection.IsValueNull)
                    return false;
            }
            return true;
        }
        protected internal bool hasUnassignedInputs()
        {
            for (int inCnt = 0; inCnt < Inputs.Count; inCnt++)
                if (Inputs[inCnt].connection == null)
                    return true;
            return false;
        }
        protected internal bool descendantsCalculated()
        {
            for (int cnt = 0; cnt < Inputs.Count; cnt++)
            {
                if (Inputs[cnt].connection != null && !Inputs[cnt].connection.body.calculated)
                    return false;
            }
            return true;
        }
        protected internal bool isInput()
        {
            for (int cnt = 0; cnt < Inputs.Count; cnt++)
                if (Inputs[cnt].connection != null)
                    return false;
            return true;
        }

        #endregion

        #region Knob Utility

        public NodeOutput CreateOutput(string outputName, string outputType)
        {
            return NodeOutput.Create(this, outputName, outputType);
        }
        public NodeOutput CreateOutput(string outputName, string outputType, NodeSide nodeSide)
        {
            return NodeOutput.Create(this, outputName, outputType, nodeSide);
        }
        public NodeOutput CreateOutput(string outputName, string outputType, NodeSide nodeSide, float sidePosition)
        {
            return NodeOutput.Create(this, outputName, outputType, nodeSide, sidePosition);
        }
        protected void OutputKnob(int outputIdx)
        {
            if (Event.current.type == EventType.Repaint)
                Outputs[outputIdx].SetPosition();
        }

        public NodeInput CreateInput(string inputName, string inputType)
        {
            return NodeInput.Create(this, inputName, inputType);
        }
        public NodeInput CreateInput(string inputName, string inputType, NodeSide nodeSide)
        {
            return NodeInput.Create(this, inputName, inputType, nodeSide);
        }
        public NodeInput CreateInput(string inputName, string inputType, NodeSide nodeSide, float sidePosition)
        {
            return NodeInput.Create(this, inputName, inputType, nodeSide, sidePosition);
        }
        protected void InputKnob(int inputIdx)
        {
            if (Event.current.type == EventType.Repaint)
                Inputs[inputIdx].SetPosition();
        }
        protected static void ReassignOutputType(ref NodeOutput output, Type newOutputType)
        {
            Node body = output.body;
            string outputName = output.name;
            // Store all valid connections that are not affected by the type change
            IEnumerable<NodeInput> validConnections = output.connections.Where((NodeInput connection) => connection.typeData.Type.IsAssignableFrom(newOutputType));
            // Delete the output of the old type
            output.Delete();
            // Create Output with new type
            NodeEditorCallbacks.IssueOnAddNodeKnob(NodeOutput.Create(body, outputName, newOutputType.AssemblyQualifiedName));
            output = body.Outputs[body.Outputs.Count - 1];
            // Restore the valid connections
            foreach (NodeInput input in validConnections)
                input.ApplyConnection(output);
        }
        protected static void ReassignInputType(ref NodeInput input, Type newInputType)
        {
            Node body = input.body;
            string inputName = input.name;
            // Store the valid connection if it's not affected by the type change
            NodeOutput validConnection = null;
            if (input.connection != null && newInputType.IsAssignableFrom(input.connection.typeData.Type))
                validConnection = input.connection;
            // Delete the input of the old type
            input.Delete();
            // Create Output with new type
            NodeEditorCallbacks.IssueOnAddNodeKnob(NodeInput.Create(body, inputName, newInputType.AssemblyQualifiedName));
            input = body.Inputs[body.Inputs.Count - 1];
            // Restore the valid connections
            if (validConnection != null)
                input.ApplyConnection(validConnection);
        }

        #endregion
        #region Node Utility

        public bool isChildOf(Node otherNode)
        {
            if (otherNode == null || otherNode == this)
                return false;
            if (BeginRecursiveSearchLoop()) return false;
            for (int cnt = 0; cnt < Inputs.Count; cnt++)
            {
                NodeOutput connection = Inputs[cnt].connection;
                if (connection != null)
                {
                    if (connection.body != startRecursiveSearchNode)
                    {
                        if (connection.body == otherNode || connection.body.isChildOf(otherNode))
                        {
                            StopRecursiveSearchLoop();
                            return true;
                        }
                    }
                }
            }
            EndRecursiveSearchLoop();
            return false;
        }

        internal bool isInLoop()
        {
            if (BeginRecursiveSearchLoop()) return this == startRecursiveSearchNode;
            for (int cnt = 0; cnt < Inputs.Count; cnt++)
            {
                NodeOutput connection = Inputs[cnt].connection;
                if (connection != null && connection.body.isInLoop())
                {
                    StopRecursiveSearchLoop();
                    return true;
                }
            }
            EndRecursiveSearchLoop();
            return false;
        }

        internal bool allowsLoopRecursion(Node otherNode)
        {
            if (AllowRecursion)
                return true;
            if (otherNode == null)
                return false;
            if (BeginRecursiveSearchLoop()) return false;
            for (int cnt = 0; cnt < Inputs.Count; cnt++)
            {
                NodeOutput connection = Inputs[cnt].connection;
                if (connection != null && connection.body.allowsLoopRecursion(otherNode))
                {
                    StopRecursiveSearchLoop();
                    return true;
                }
            }
            EndRecursiveSearchLoop();
            return false;
        }

        public void ClearCalculation()
        {
            if (BeginRecursiveSearchLoop()) return;
            calculated = false;
            for (int outCnt = 0; outCnt < Outputs.Count; outCnt++)
            {
                NodeOutput output = Outputs[outCnt];
                for (int conCnt = 0; conCnt < output.connections.Count; conCnt++)
                    output.connections[conCnt].body.ClearCalculation();
            }
            EndRecursiveSearchLoop();
        }

        #region Recursive Search Helpers

        [NonSerialized] private List<Node> recursiveSearchSurpassed;
        [NonSerialized] private Node startRecursiveSearchNode; 

        internal bool BeginRecursiveSearchLoop()
        {
            if (startRecursiveSearchNode == null || recursiveSearchSurpassed == null)
            {
                recursiveSearchSurpassed = new List<Node>();
                startRecursiveSearchNode = this;
            }

            if (recursiveSearchSurpassed.Contains(this))
                return true;
            recursiveSearchSurpassed.Add(this);
            return false;
        }

        internal void EndRecursiveSearchLoop()
        {
            if (startRecursiveSearchNode == this)
            {   recursiveSearchSurpassed = null;
                startRecursiveSearchNode = null;
            }
        }

        internal void StopRecursiveSearchLoop()
        {
            recursiveSearchSurpassed = null;
            startRecursiveSearchNode = null;
        }

        #endregion

        #endregion
    }
}
