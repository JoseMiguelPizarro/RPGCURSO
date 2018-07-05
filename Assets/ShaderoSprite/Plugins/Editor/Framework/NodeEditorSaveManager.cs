using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
    public static class NodeEditorSaveManager
    {
        #region Scene Saving

        private static GameObject sceneSaveHolder;

        private static void FetchSceneSaveHolder()
        {
            if (sceneSaveHolder == null)
            {
                sceneSaveHolder = GameObject.Find("NodeEditor_SceneSaveHolder");
                if (sceneSaveHolder == null)
                    sceneSaveHolder = new GameObject("NodeEditor_SceneSaveHolder");
                sceneSaveHolder.hideFlags = HideFlags.None;
            }
        }

        public static string[] GetSceneSaves()
        {
            FetchSceneSaveHolder();
            return sceneSaveHolder.GetComponents<NodeCanvasSceneSave>().Select(((NodeCanvasSceneSave save) => save.savedNodeCanvas.name)).ToArray();
        }


        private static NodeCanvasSceneSave FindSceneSave(string saveName)
        {
            FetchSceneSaveHolder();
            return sceneSaveHolder.GetComponents<NodeCanvasSceneSave>().ToList().Find((NodeCanvasSceneSave save) => save.savedNodeCanvas.name == saveName);
        }


        public static void SaveSceneNodeCanvas(string saveName, ref NodeCanvas nodeCanvas, bool createWorkingCopy)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogError("Cannot save Canvas to scene: No save name specified!");
                return;
            }

            if (!nodeCanvas.livesInScene
#if UNITY_EDITOR
            || UnityEditor.AssetDatabase.Contains(nodeCanvas)
#endif
            )
            {

                nodeCanvas = CreateWorkingCopy(nodeCanvas, true);
            }
            else
                nodeCanvas.Validate();

            nodeCanvas.livesInScene = true;
            nodeCanvas.name = saveName;

#if UNITY_EDITOR
            nodeCanvas.BeforeSavingCanvas();
#endif

            NodeCanvas savedCanvas = nodeCanvas;

            ProcessCanvas(ref savedCanvas, createWorkingCopy);


            NodeCanvasSceneSave sceneSave = FindSceneSave(saveName);
            if (sceneSave == null)
                sceneSave = sceneSaveHolder.AddComponent<NodeCanvasSceneSave>();
            sceneSave.savedNodeCanvas = savedCanvas;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(sceneSaveHolder);
#endif
        }


        public static NodeCanvas LoadSceneNodeCanvas(string saveName, bool createWorkingCopy)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogError("Cannot load Canvas from scene: No save name specified!");
                return null;
            }

            NodeCanvasSceneSave sceneSave = FindSceneSave(saveName);
            if (sceneSave == null)
                return null;


            NodeCanvas savedCanvas = sceneSave.savedNodeCanvas;
            savedCanvas.livesInScene = true;


            ProcessCanvas(ref savedCanvas, createWorkingCopy);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(sceneSaveHolder);
#endif

            return savedCanvas;
        }

        #endregion

        #region Asset Saving


        public static void SaveNodeCanvas(string path, NodeCanvas nodeCanvas, bool createWorkingCopy)
        {
#if !UNITY_EDITOR
			throw new System.NotImplementedException ();
#else

            if (string.IsNullOrEmpty(path)) throw new UnityException("Cannot save NodeCanvas: No spath specified to save the NodeCanvas " + (nodeCanvas != null ? nodeCanvas.name : "") + " to!");
            if (nodeCanvas == null) throw new UnityException("Cannot save NodeCanvas: The specified NodeCanvas that should be saved to path " + path + " is null!");
            if (nodeCanvas.livesInScene)
                Debug.LogWarning("Attempting to save scene canvas " + nodeCanvas.name + " to an asset, scene object references will be broken!" + (!createWorkingCopy ? " No workingCopy is going to be created, so your scene save is broken, too!" : ""));
#if UNITY_EDITOR
            if (!createWorkingCopy && UnityEditor.AssetDatabase.Contains(nodeCanvas) && UnityEditor.AssetDatabase.GetAssetPath(nodeCanvas) != path) { Debug.LogError("Trying to create a duplicate save file for '" + nodeCanvas.name + "'! Forcing to create a working copy!"); createWorkingCopy = true; }
#endif

#if UNITY_EDITOR
            nodeCanvas.BeforeSavingCanvas();


            ProcessCanvas(ref nodeCanvas, createWorkingCopy);
            nodeCanvas.livesInScene = false;


            UnityEditor.AssetDatabase.CreateAsset(nodeCanvas, path);
            AddSubAssets(nodeCanvas.editorStates, nodeCanvas);


            foreach (Node node in nodeCanvas.nodes)
            {
                AddSubAsset(node, nodeCanvas);
                AddSubAssets(node.GetScriptableObjects(), node);
                foreach (NodeKnob knob in node.nodeKnobs)
                {
                    AddSubAsset(knob, node);
                    AddSubAssets(knob.GetScriptableObjects(), knob);
                }
            }

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#else
			// TODO: Node Editor: Need to implement ingame-saving (Resources, AsssetBundles, ... won't work)
#endif

            NodeEditorCallbacks.IssueOnSaveCanvas(nodeCanvas);

#endif
        }


        public static NodeCanvas LoadNodeCanvas(string path, bool createWorkingCopy)
        {
            if (!File.Exists(path)) throw new UnityException("Cannot Load NodeCanvas: File '" + path + "' deos not exist!");


            NodeCanvas nodeCanvas = ResourceManager.LoadResource<NodeCanvas>(path);
            if (nodeCanvas == null) throw new UnityException("Cannot Load NodeCanvas: The file at the specified path '" + path + "' is no valid save file as it does not contain a NodeCanvas!");

#if UNITY_EDITOR
            if (!Application.isPlaying && (nodeCanvas.editorStates == null || nodeCanvas.editorStates.Length == 0))
            {
                nodeCanvas.editorStates = ResourceManager.LoadResources<NodeEditorState>(path);
            }
#endif


            ProcessCanvas(ref nodeCanvas, createWorkingCopy);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            NodeEditorCallbacks.IssueOnLoadCanvas(nodeCanvas);
            return nodeCanvas;
        }

        #region Utility

#if UNITY_EDITOR


        public static void AddSubAssets(ScriptableObject[] subAssets, ScriptableObject mainAsset)
        {
            foreach (ScriptableObject subAsset in subAssets)
                AddSubAsset(subAsset, mainAsset);
        }


        public static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
        {
            if (subAsset != null && mainAsset != null)
            {
                UnityEditor.AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
                subAsset.hideFlags = HideFlags.HideInHierarchy;
            }
        }


        public static void AddSubAsset(ScriptableObject subAsset, string path)
        {
            if (subAsset != null && !string.IsNullOrEmpty(path))
            {
                UnityEditor.AssetDatabase.AddObjectToAsset(subAsset, path);
                subAsset.hideFlags = HideFlags.HideInHierarchy;
            }
        }

#endif


        private static void ProcessCanvas(ref NodeCanvas canvas, bool workingCopy)
        {

            if (workingCopy)
                canvas = CreateWorkingCopy(canvas, true);
            else
                canvas.Validate();
        }

        #endregion

        #endregion

        #region Compression



        public static void Uncompress(ref NodeCanvas nodeCanvas)
        {
            for (int nodeCnt = 0; nodeCnt < nodeCanvas.nodes.Count; nodeCnt++)
            {
                Node node = nodeCanvas.nodes[nodeCnt];
                node.Inputs = new List<NodeInput>();
                node.Outputs = new List<NodeOutput>();
                for (int knobCnt = 0; knobCnt < node.nodeKnobs.Count; knobCnt++)
                {
                    NodeKnob knob = node.nodeKnobs[knobCnt];
                    if (knob is NodeInput)
                        node.Inputs.Add(knob as NodeInput);
                    else if (knob is NodeOutput)
                        node.Outputs.Add(knob as NodeOutput);
                }
            }
        }

        #endregion

        #region Working Copy


        public static NodeCanvas CreateWorkingCopy(NodeCanvas nodeCanvas, bool editorStates)
        {
            nodeCanvas.Validate();
            nodeCanvas = Clone(nodeCanvas);


            List<ScriptableObject> allSOs = new List<ScriptableObject>();
            List<ScriptableObject> clonedSOs = new List<ScriptableObject>();
            for (int nodeCnt = 0; nodeCnt < nodeCanvas.nodes.Count; nodeCnt++)
            {
                Node node = nodeCanvas.nodes[nodeCnt];
                node.CheckNodeKnobMigration();


                Node clonedNode = AddClonedSO(allSOs, clonedSOs, node);
                AddClonedSOs(allSOs, clonedSOs, clonedNode.GetScriptableObjects());

                foreach (NodeKnob knob in clonedNode.nodeKnobs)
                {
                    AddClonedSO(allSOs, clonedSOs, knob);
                    AddClonedSOs(allSOs, clonedSOs, knob.GetScriptableObjects());
                }
            }


            for (int nodeCnt = 0; nodeCnt < nodeCanvas.nodes.Count; nodeCnt++)
            {
                Node node = nodeCanvas.nodes[nodeCnt];

                Node clonedNode = nodeCanvas.nodes[nodeCnt] = ReplaceSO(allSOs, clonedSOs, node);
                clonedNode.CopyScriptableObjects((ScriptableObject so) => ReplaceSO(allSOs, clonedSOs, so));


                clonedNode.Inputs = new List<NodeInput>();
                clonedNode.Outputs = new List<NodeOutput>();
                for (int knobCnt = 0; knobCnt < clonedNode.nodeKnobs.Count; knobCnt++)
                {
                    NodeKnob knob = clonedNode.nodeKnobs[knobCnt] = ReplaceSO(allSOs, clonedSOs, clonedNode.nodeKnobs[knobCnt]);
                    knob.body = clonedNode;

                    knob.CopyScriptableObjects((ScriptableObject so) => ReplaceSO(allSOs, clonedSOs, so));

                    if (knob is NodeInput)
                        clonedNode.Inputs.Add(knob as NodeInput);
                    else if (knob is NodeOutput)
                        clonedNode.Outputs.Add(knob as NodeOutput);
                }
            }

            if (editorStates)
            {
                nodeCanvas.editorStates = CreateWorkingCopy(nodeCanvas.editorStates, nodeCanvas);
                foreach (NodeEditorState state in nodeCanvas.editorStates)
                    state.selectedNode = ReplaceSO(allSOs, clonedSOs, state.selectedNode);
            }
            else
            {
                foreach (NodeEditorState state in nodeCanvas.editorStates)
                    state.selectedNode = null;
            }

            return nodeCanvas;
        }


        private static NodeEditorState[] CreateWorkingCopy(NodeEditorState[] editorStates, NodeCanvas associatedNodeCanvas)
        {
            if (editorStates == null)
                return new NodeEditorState[0];
            editorStates = (NodeEditorState[])editorStates.Clone();
            for (int stateCnt = 0; stateCnt < editorStates.Length; stateCnt++)
            {
                if (editorStates[stateCnt] == null)
                    continue;
                NodeEditorState state = editorStates[stateCnt] = Clone(editorStates[stateCnt]);
                if (state == null)
                {
                    Debug.LogError("Failed to create a working copy for an NodeEditorState during the loading process of " + associatedNodeCanvas.name + "!");
                    continue;
                }
                state.canvas = associatedNodeCanvas;
            }
            associatedNodeCanvas.editorStates = editorStates;
            return editorStates;
        }

        #region Utility


        private static T Clone<T>(T SO) where T : ScriptableObject
        {
            string soName = SO.name;
            SO = Object.Instantiate<T>(SO);
            SO.name = soName;
            return SO;
        }


        private static void AddClonedSOs(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, ScriptableObject[] initialSOs)
        {
            scriptableObjects.AddRange(initialSOs);
            clonedScriptableObjects.AddRange(initialSOs.Select((ScriptableObject so) => Clone(so)));
        }


        private static T AddClonedSO<T>(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, T initialSO) where T : ScriptableObject
        {
            if (initialSO == null)
                return null;
            scriptableObjects.Add(initialSO);
            T clonedSO = Clone(initialSO);
            clonedScriptableObjects.Add(clonedSO);
            return clonedSO;
        }


        private static T ReplaceSO<T>(List<ScriptableObject> scriptableObjects, List<ScriptableObject> clonedScriptableObjects, T initialSO) where T : ScriptableObject
        {
            if (initialSO == null)
                return null;
            int soInd = scriptableObjects.IndexOf(initialSO);
            if (soInd == -1)
                Debug.LogError("GetWorkingCopy: ScriptableObject " + initialSO.name + " was not copied before! It will be null!");
            return soInd == -1 ? null : (T)clonedScriptableObjects[soInd];
        }

        #endregion

        #endregion

        #region Utility


        public static NodeEditorState ExtractEditorState(NodeCanvas canvas, string stateName)
        {
            NodeEditorState state = null;
            if (canvas.editorStates.Length > 0)
            {
                state = canvas.editorStates.First((NodeEditorState s) => s.name == stateName);
                if (state == null)
                    state = canvas.editorStates[0];
            }
            if (state == null)
            {
                state = ScriptableObject.CreateInstance<NodeEditorState>();
                state.canvas = canvas;
                canvas.editorStates = new NodeEditorState[] { state };
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(canvas);
#endif
            }
            state.name = stateName;
            return state;
        }

        #endregion
    }
}