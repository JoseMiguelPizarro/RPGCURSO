using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using _ShaderoShaderEditorFramework;

namespace _ShaderoShaderEditorFramework 
{
	public static class NodeTypes
	{
		public static Dictionary<Node, NodeData> nodes;
		public static void FetchNodes() 
		{
			nodes = new Dictionary<Node, NodeData> ();

			IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly"));
			foreach (Assembly assembly in scriptAssemblies) 
			{
				foreach (Type type in assembly.GetTypes().Where(T => T.IsClass && !T.IsAbstract && T.IsSubclassOf(typeof(Node))))
				{
					object[] nodeAttributes = type.GetCustomAttributes(typeof(NodeAttribute), false);                    
					NodeAttribute attr = nodeAttributes[0] as NodeAttribute;
                    if (attr == null || !attr.hide)
					{
                        Node node = ScriptableObject.CreateInstance ("_ShaderoShaderEditorFramework." + type.Name) as Node;
						node = node.Create (Vector2.zero); 
						nodes.Add (node, new NodeData (attr == null? node.name : attr.contextText, attr.limitToCanvasTypes));
					}
				}
			}
		}

		public static NodeData getNodeData (Node node)
		{
			return nodes [getDefaultNode (node.GetID)];
		}

		public static Node getDefaultNode (string nodeID)
		{
			return nodes.Keys.Single<Node> ((Node node) => node.GetID == nodeID);
		}

		public static T getDefaultNode<T> () where T : Node
		{
			return nodes.Keys.Single<Node> ((Node node) => node.GetType () == typeof (T)) as T;
		}

		public static List<Node> getCompatibleNodes (NodeOutput nodeOutput)
		{
			if (nodeOutput == null)
				throw new ArgumentNullException ("nodeOutput");
			List<Node> compatibleNodes = new List<Node> ();
			foreach (Node node in NodeTypes.nodes.Keys)
			{ 
				for (int inputCnt = 0; inputCnt < node.Inputs.Count; inputCnt++)
				{ 
					NodeInput input = node.Inputs[inputCnt];
					if (input == null)
						throw new UnityException ("Input " + inputCnt + " is null!");
					if (input.typeData.Type.IsAssignableFrom (nodeOutput.typeData.Type))
					{
						compatibleNodes.Add (node);
						break;
					}
				}
			}
			return compatibleNodes;
		}
	}

	public struct NodeData 
	{
		public string adress;
		public Type[] limitToCanvasTypes;

		public NodeData (string name, Type[] limitedCanvasTypes)
		{
			adress = name;
			limitToCanvasTypes = limitedCanvasTypes;
		}
	}

	public class NodeAttribute : Attribute 
	{
		public bool hide { get; private set; }
		public string contextText { get; private set; }
		public Type[] limitToCanvasTypes { get; private set; }

		public NodeAttribute (bool HideNode, string ReplacedContextText)
		{
			hide = HideNode;
			contextText = ReplacedContextText;
			limitToCanvasTypes = null;
		}

		public NodeAttribute (bool HideNode, string ReplacedContextText, Type[] limitedCanvasTypes)
		{
			hide = HideNode;
			contextText = ReplacedContextText;
			limitToCanvasTypes = limitedCanvasTypes;
		}
	}
}