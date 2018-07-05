using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
	public class NodeEditorUserCache
	{
		public NodeCanvas nodeCanvas;
		public NodeEditorState editorState;
		public void AssureCanvas () { if (nodeCanvas == null) NewNodeCanvas (); if (editorState == null) NewEditorState (); }

		private string cachePath;
		private bool useCache;
		private const string MainEditorStateIdentifier = "MainEditorState";

		private string lastSessionPath { get { return cachePath + "/LastSession.asset"; } }

		public string openedCanvasPath = "";

		public NodeEditorUserCache (NodeCanvas loadedCanvas)
		{
			useCache = false;
			SetCanvas (loadedCanvas);
		}

		public NodeEditorUserCache ()
		{
			useCache = false;
		}

		#if UNITY_EDITOR
		public NodeEditorUserCache (string CachePath, NodeCanvas loadedCanvas)
		{
			useCache = true;
			cachePath = CachePath;
			SetCanvas (loadedCanvas);
		}

		public NodeEditorUserCache (string CachePath)
		{
			useCache = true;
			cachePath = CachePath;
		}
		#endif

		#if UNITY_EDITOR

		#region Cache

		public void SetupCacheEvents () 
		{ 
			if (!useCache)
				return;

		
			EditorLoadingControl.lateEnteredPlayMode -= LoadCache;
			EditorLoadingControl.lateEnteredPlayMode += LoadCache;
			EditorLoadingControl.justOpenedNewScene -= LoadCache;
			EditorLoadingControl.justOpenedNewScene += LoadCache;

			
			NodeEditorCallbacks.OnAddNode -= SaveNewNode;
			NodeEditorCallbacks.OnAddNode += SaveNewNode;
			NodeEditorCallbacks.OnAddNodeKnob -= SaveNewNodeKnob;
			NodeEditorCallbacks.OnAddNodeKnob += SaveNewNodeKnob;

			LoadCache ();
		}

		public void ClearCacheEvents () 
		{
			EditorLoadingControl.lateEnteredPlayMode -= LoadCache;
			EditorLoadingControl.justLeftPlayMode -= LoadCache;
			EditorLoadingControl.justOpenedNewScene -= LoadCache;
			NodeEditorCallbacks.OnAddNode -= SaveNewNode;
			NodeEditorCallbacks.OnAddNodeKnob -= SaveNewNodeKnob;
		}

		private void SaveNewNode (Node node) 
		{
			if (!useCache)
				return;
			if (nodeCanvas.livesInScene)
			{
				DeleteCache ();
				return;
			}
			if (!nodeCanvas.nodes.Contains (node))
				return;

			CheckCurrentCache ();

			NodeEditorSaveManager.AddSubAsset (node, lastSessionPath);
			foreach (ScriptableObject so in node.GetScriptableObjects ())
				NodeEditorSaveManager.AddSubAsset (so, node);

			foreach (NodeKnob knob in node.nodeKnobs)
			{
				NodeEditorSaveManager.AddSubAsset (knob, node);
				foreach (ScriptableObject so in knob.GetScriptableObjects ())
					NodeEditorSaveManager.AddSubAsset (so, knob);
			}

			UpdateCacheFile ();
		}

		private void SaveNewNodeKnob (NodeKnob knob) 
		{
			if (!useCache)
				return;
			if (nodeCanvas.livesInScene) 
			{
				DeleteCache ();
				return;
			}
			if (!nodeCanvas.nodes.Contains (knob.body))
				return;

			CheckCurrentCache ();

			NodeEditorSaveManager.AddSubAsset (knob, knob.body);
			foreach (ScriptableObject so in knob.GetScriptableObjects ())
				NodeEditorSaveManager.AddSubAsset (so, knob);

			UpdateCacheFile ();
		}

	
		private void SaveCache () 
		{
			if (!useCache)
				return;
			if (nodeCanvas.livesInScene)
			{
				DeleteCache ();
				return;
			}

			nodeCanvas.editorStates = new NodeEditorState[] { editorState };
			NodeEditorSaveManager.SaveNodeCanvas (lastSessionPath, nodeCanvas, false);

			CheckCurrentCache ();
		}

		
		private void LoadCache () 
		{
			if (!useCache)
			{
				NewNodeCanvas ();
				return;
			}
			
			if (!File.Exists (lastSessionPath) || (nodeCanvas = NodeEditorSaveManager.LoadNodeCanvas (lastSessionPath, false)) == null)
			{
				NewNodeCanvas ();
				return;
			}

			
			editorState = NodeEditorSaveManager.ExtractEditorState (nodeCanvas, MainEditorStateIdentifier);
			if (!UnityEditor.AssetDatabase.Contains (editorState))
				NodeEditorSaveManager.AddSubAsset (editorState, lastSessionPath);

			CheckCurrentCache ();

			NodeEditor.RecalculateAll (nodeCanvas);
			NodeEditor.RepaintClients ();
		}

		private void CheckCurrentCache () 
		{
			if (!nodeCanvas.livesInScene && UnityEditor.AssetDatabase.GetAssetPath (nodeCanvas) != lastSessionPath)
				throw new UnityException ("Cache system error: Current Canvas is not saved as the temporary cache!");
		}

		private void DeleteCache () 
		{
			UnityEditor.AssetDatabase.DeleteAsset (lastSessionPath);
			UnityEditor.AssetDatabase.Refresh ();
		
		}

		private void UpdateCacheFile () 
		{
			UnityEditor.EditorUtility.SetDirty (nodeCanvas);
			UnityEditor.AssetDatabase.SaveAssets ();
			UnityEditor.AssetDatabase.Refresh ();
		}

		#endregion

		#endif

		#region Save/Load

		public void SetCanvas (NodeCanvas canvas)
		{
			nodeCanvas = canvas;
			editorState = NodeEditorSaveManager.ExtractEditorState (nodeCanvas, MainEditorStateIdentifier);
			NodeEditor.RepaintClients ();
		}

		
		public void SaveSceneNodeCanvas (string path) 
		{
			nodeCanvas.editorStates = new NodeEditorState[] { editorState };
			NodeEditorSaveManager.SaveSceneNodeCanvas (path, ref nodeCanvas, true);
			editorState = NodeEditorSaveManager.ExtractEditorState (nodeCanvas, MainEditorStateIdentifier);
		#if UNITY_EDITOR
			if (useCache)
				DeleteCache ();
		#endif
			NodeEditor.RepaintClients ();
		}

		
		public void LoadSceneNodeCanvas (string path) 
		{
		#if UNITY_EDITOR
			if (useCache)
				DeleteCache ();
		#endif
			
			if ((nodeCanvas = NodeEditorSaveManager.LoadSceneNodeCanvas (path, true)) == null)
			{
				NewNodeCanvas ();
				return;
			}
			editorState = NodeEditorSaveManager.ExtractEditorState (nodeCanvas, MainEditorStateIdentifier);

			openedCanvasPath = path;
			NodeEditor.RecalculateAll (nodeCanvas);
			NodeEditor.RepaintClients ();
		}

		
		public void SaveNodeCanvas (string path) 
		{
			nodeCanvas.editorStates = new NodeEditorState[] { editorState };
			NodeEditorSaveManager.SaveNodeCanvas (path, nodeCanvas, true);
			NodeEditor.RepaintClients ();
		}

	
		public void LoadNodeCanvas (string path) 
		{
		
			if (!File.Exists (path) || (nodeCanvas = NodeEditorSaveManager.LoadNodeCanvas (path, true)) == null)
			{
				NewNodeCanvas ();
				return;
			}
			editorState = NodeEditorSaveManager.ExtractEditorState (nodeCanvas, MainEditorStateIdentifier);

			openedCanvasPath = path;
		#if UNITY_EDITOR
			if (useCache)
				SaveCache ();
		#endif
			NodeEditor.RecalculateAll (nodeCanvas);
			NodeEditor.RepaintClients ();
		}

	
		public void NewNodeCanvas (Type canvasType = null) 
		{
			if (canvasType != null && canvasType.IsSubclassOf (typeof(NodeCanvas)))
				nodeCanvas = ScriptableObject.CreateInstance(canvasType) as NodeCanvas;
			else
				nodeCanvas = ScriptableObject.CreateInstance<NodeCanvas>();
			nodeCanvas.name = "New " + nodeCanvas.canvasName;

            NodeEditor.NewCanvasWasCalled = true;
		
			NewEditorState ();
			openedCanvasPath = "";
		#if UNITY_EDITOR
			if (useCache)
				SaveCache ();
		#endif
		}

		
		public void NewEditorState () 
		{
			editorState = ScriptableObject.CreateInstance<NodeEditorState> ();
			editorState.canvas = nodeCanvas;
			editorState.name = MainEditorStateIdentifier;
			nodeCanvas.editorStates = new NodeEditorState[] { editorState };
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty (nodeCanvas);
			#endif
		}

		#endregion
	}

}