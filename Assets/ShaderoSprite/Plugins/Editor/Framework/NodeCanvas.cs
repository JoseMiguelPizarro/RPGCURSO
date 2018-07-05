using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace _ShaderoShaderEditorFramework 
{
	[NodeCanvasType("Default")]
	public class NodeCanvas : ScriptableObject 
	{   public virtual string canvasName { get { return "Calculation Canvas"; } }
        public List<Node> nodes = new List<Node> ();
 		public NodeEditorState[] editorStates = new NodeEditorState[0];
        public bool livesInScene = false;
        public virtual void BeforeSavingCanvas () { }
        public void Validate () 
		{
            
            if (nodes == null)
			{
				nodes = new List<Node> ();
			}
			for (int nodeCnt = 0; nodeCnt < nodes.Count; nodeCnt++) 
			{
				Node node = nodes[nodeCnt];
				if (node == null)
				{
					nodes.RemoveAt (nodeCnt);
					nodeCnt--;
					continue;
				}
				for (int knobCnt = 0; knobCnt < node.nodeKnobs.Count; knobCnt++) 
				{
					NodeKnob nodeKnob = node.nodeKnobs[knobCnt];
					if (nodeKnob == null)
					{
						node.nodeKnobs.RemoveAt (knobCnt);
						knobCnt--;
						continue;
					}

					if (nodeKnob is NodeInput)
					{
						NodeInput input = nodeKnob as NodeInput;
						if (input.connection != null && input.connection.body == null)
						{
                            input.connection = null;
						}
					}
					else if (nodeKnob is NodeOutput)
					{
						NodeOutput output = nodeKnob as NodeOutput;
						for (int conCnt = 0; conCnt < output.connections.Count; conCnt++) 
						{
							NodeInput con = output.connections[conCnt];
							if (con == null || con.body == null)
							{
								output.connections.RemoveAt (conCnt);
								conCnt--;
							}
						}
					}
				}
			}

			if (editorStates == null)
			{
				editorStates = new NodeEditorState[0];
			}
			editorStates = editorStates.Where ((NodeEditorState state) => state != null).ToArray ();
			foreach (NodeEditorState state in editorStates)
			{
				if (!nodes.Contains (state.selectedNode))
					state.selectedNode = null;
			}
       
        }
    }
}