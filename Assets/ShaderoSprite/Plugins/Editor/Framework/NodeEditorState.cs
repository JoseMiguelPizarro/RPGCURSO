using UnityEngine;
using System;
using System.Collections.Generic;
using _ShaderoShaderEditorFramework;

namespace _ShaderoShaderEditorFramework 
{
	public class NodeEditorState : ScriptableObject 
	{	public NodeCanvas canvas;
		public NodeEditorState parentEditor;

		public bool drawing = true; 

		public Node selectedNode;
		[NonSerialized] public Node focusedNode; 
		[NonSerialized] public NodeKnob focusedNodeKnob;

        public string LivePreviewShaderPath = "";
        public string ShaderName = "";
    
        public Shader ShaderInMemory;


        public Vector2 panOffset = new Vector2 (); 
		public float zoom = 1; 

		[NonSerialized] public NodeOutput connectOutput; 
		[NonSerialized] public bool dragNode; 
		[NonSerialized] public bool panWindow; 
		[NonSerialized] public Vector2 dragStart; 
		[NonSerialized] public Vector2 dragPos;
		[NonSerialized] public Vector2 dragOffset;
		[NonSerialized] public bool navigate; 

		public Vector2 zoomPos { get { return canvasRect.size/2; } } 
		[NonSerialized] public Rect canvasRect; 
		[NonSerialized] public Vector2 zoomPanAdjust; 
		[NonSerialized] public List<Rect> ignoreInput = new List<Rect> (); 

      
    }
}