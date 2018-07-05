using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework 
{
	public static class NodeEditorInputSystem
	{
		#region Setup and Fetching

		private static List<KeyValuePair<EventHandlerAttribute, Delegate>> eventHandlers;
		private static List<KeyValuePair<HotkeyAttribute, Delegate>> hotkeyHandlers;
		private static List<KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>> contextEntries;
		private static List<KeyValuePair<ContextFillerAttribute, Delegate>> contextFillers;

		public static void SetupInput () 
		{
			eventHandlers = new List<KeyValuePair<EventHandlerAttribute, Delegate>> ();
			hotkeyHandlers = new List<KeyValuePair<HotkeyAttribute, Delegate>> ();
			contextEntries = new List<KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>> ();
			contextFillers = new List<KeyValuePair<ContextFillerAttribute, Delegate>> ();

			IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly"));
			foreach (Assembly assembly in scriptAssemblies) 
			{
				foreach (Type type in assembly.GetTypes ()) 
				{
					foreach (MethodInfo method in type.GetMethods (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)) 
					{
						#region Event-Attributes recognition and storing

						Delegate actionDelegate = null;
						foreach (object attr in method.GetCustomAttributes (true))
						{
							Type attrType = attr.GetType ();
							if (attrType == typeof(EventHandlerAttribute))
							{ 	if (EventHandlerAttribute.AssureValidity (method, attr as EventHandlerAttribute)) 
								{   if (actionDelegate == null) actionDelegate = Delegate.CreateDelegate (typeof(Action<NodeEditorInputInfo>), method);
									eventHandlers.Add (new KeyValuePair<EventHandlerAttribute, Delegate> (attr as EventHandlerAttribute, actionDelegate));
								}
							}
							else if (attrType == typeof(HotkeyAttribute))
							{ 	if (HotkeyAttribute.AssureValidity (method, attr as HotkeyAttribute)) 
								{   if (actionDelegate == null) actionDelegate = Delegate.CreateDelegate (typeof(Action<NodeEditorInputInfo>), method);
									hotkeyHandlers.Add (new KeyValuePair<HotkeyAttribute, Delegate> (attr as HotkeyAttribute, actionDelegate));
								}
							}
							else if (attrType == typeof(ContextEntryAttribute))
							{ 	if (ContextEntryAttribute.AssureValidity (method, attr as ContextEntryAttribute)) 
								{  if (actionDelegate == null) actionDelegate = Delegate.CreateDelegate (typeof(Action<NodeEditorInputInfo>), method);
									PopupMenu.MenuFunctionData menuFunction = (object callbackObj) => 
									{
										if (!(callbackObj is NodeEditorInputInfo))
											throw new UnityException ("Callback Object passed by context is not of type NodeEditorMenuCallback!");
										actionDelegate.DynamicInvoke (callbackObj as NodeEditorInputInfo);
									};
									contextEntries.Add (new KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData> (attr as ContextEntryAttribute, menuFunction));
								}
							}
							else if (attrType == typeof(ContextFillerAttribute))
							{ 	if (ContextFillerAttribute.AssureValidity (method, attr as ContextFillerAttribute)) 
								{   Delegate methodDel = Delegate.CreateDelegate (typeof(Action<NodeEditorInputInfo, GenericMenu>), method);
									contextFillers.Add (new KeyValuePair<ContextFillerAttribute, Delegate> (attr as ContextFillerAttribute, methodDel));
								}
							}
						}

						#endregion
					}
				}
			}

			eventHandlers.Sort ((handlerA, handlerB) => handlerA.Key.priority.CompareTo (handlerB.Key.priority));
			hotkeyHandlers.Sort ((handlerA, handlerB) => handlerA.Key.priority.CompareTo (handlerB.Key.priority));
		}

		#endregion

		#region Invoking Dynamic Input Handlers

		private static void CallEventHandlers (NodeEditorInputInfo inputInfo, bool late) 
		{
			object[] parameter = new object[] { inputInfo };
			foreach (KeyValuePair<EventHandlerAttribute, Delegate> eventHandler in eventHandlers)
			{
				if ((eventHandler.Key.handledEvent == null || eventHandler.Key.handledEvent == inputInfo.inputEvent.type) &&
					(late? eventHandler.Key.priority >= 100 : eventHandler.Key.priority < 100))
				{ 	eventHandler.Value.DynamicInvoke (parameter);
					if (inputInfo.inputEvent.type == EventType.Used)
						return;
				}
			}
		}

		private static void CallHotkeys (NodeEditorInputInfo inputInfo, KeyCode keyCode, EventModifiers mods) 
		{
			object[] parameter = new object[] { inputInfo };
			foreach (KeyValuePair<HotkeyAttribute, Delegate> hotKey in hotkeyHandlers)
			{
				if (hotKey.Key.handledHotKey == keyCode && 
					(hotKey.Key.modifiers == null || hotKey.Key.modifiers == mods) && 
					(hotKey.Key.limitingEventType == null || hotKey.Key.limitingEventType == inputInfo.inputEvent.type))
				{
					hotKey.Value.DynamicInvoke (parameter);
					if (inputInfo.inputEvent.type == EventType.Used)
						return;
				}
			}
		}

			private static void FillContextMenu (NodeEditorInputInfo inputInfo, GenericMenu contextMenu, ContextType contextType) 
		{
          	foreach (KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData> contextEntry in contextEntries)
			{   if (inputInfo.editorState.focusedNode)
                {
                     if (inputInfo.editorState.focusedNode.name == "Build Shader" && contextEntry.Key.contextPath == "Duplicate Node" && NodeEditor.NoBuildShaderContext) continue;
                     if (inputInfo.editorState.focusedNode.name == "Lighting Support" && contextEntry.Key.contextPath == "Duplicate Node" && NodeEditor.NoBuildShaderContext) continue;
                }
                if (contextEntry.Key.contextType == contextType)
					contextMenu.AddItem (new GUIContent (contextEntry.Key.contextPath), false, contextEntry.Value, inputInfo);
			}

			object[] fillerParams = new object[] { inputInfo, contextMenu };
			foreach (KeyValuePair<ContextFillerAttribute, Delegate> contextFiller in contextFillers)
			{ 	if (contextFiller.Key.contextType == contextType)
					contextFiller.Value.DynamicInvoke (fillerParams);
			}
		}

		#endregion

		#region Event Handling
    	public static void HandleInputEvents (NodeEditorState state)
		{
			if (shouldIgnoreInput (state))
				return;

			NodeEditorInputInfo inputInfo = new NodeEditorInputInfo (state);
			CallEventHandlers (inputInfo, false);
			CallHotkeys (inputInfo, Event.current.keyCode, Event.current.modifiers);


		}
		public static void HandleLateInputEvents (NodeEditorState state) 
		{
			if (shouldIgnoreInput (state))
				return;
				NodeEditorInputInfo inputInfo = new NodeEditorInputInfo (state);
			CallEventHandlers (inputInfo, true);
		}

			internal static bool shouldIgnoreInput (NodeEditorState state) 
		{
			if (OverlayGUI.HasPopupControl ())
				return true;
			if (!state.canvasRect.Contains (Event.current.mousePosition))
				return true;
			for (int ignoreCnt = 0; ignoreCnt < state.ignoreInput.Count; ignoreCnt++) 
			{
				if (state.ignoreInput [ignoreCnt].Contains (Event.current.mousePosition)) 
					return true;
			}
			return false;
		}

		#endregion

		#region Essential Controls
	
		private static NodeEditorState unfocusControlsForState;

		[EventHandlerAttribute (-4)] 
		private static void HandleFocussing (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			state.focusedNode = NodeEditor.NodeAtPosition (NodeEditor.ScreenToCanvasSpace (inputInfo.inputPos), out state.focusedNodeKnob);
			
			if (unfocusControlsForState == state && Event.current.type == EventType.Repaint) 
			{
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
				unfocusControlsForState = null;
			}
		}

		[EventHandlerAttribute (EventType.MouseDown, -2)]
		private static void HandleSelecting (NodeEditorInputInfo inputInfo) 
		{
			NodeEditorState state = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0 && state.focusedNode != state.selectedNode)
			{
				unfocusControlsForState = state;
				state.selectedNode = state.focusedNode;
				NodeEditor.RepaintClients ();
			#if UNITY_EDITOR
				if (state.selectedNode != null)
					UnityEditor.Selection.activeObject = state.selectedNode;
			#endif
			}
		}
		[EventHandlerAttribute (EventType.MouseDown, 0)] 
		private static void HandleContextClicks (NodeEditorInputInfo inputInfo) 
		{
			if (Event.current.button == 1) 
			{ 
                GenericMenu contextMenu = new GenericMenu();
                if (inputInfo.editorState.focusedNode != null) // Node Context Click
                {
                     FillContextMenu(inputInfo, contextMenu, ContextType.Node);
                }
                else 
                    FillContextMenu(inputInfo, contextMenu, ContextType.Canvas);
                contextMenu.Show(inputInfo.inputPos);
                Event.current.Use();
            }
		}

		#endregion
	}

	public class NodeEditorInputInfo
	{
		public string message;
		public NodeEditorState editorState;
		public Event inputEvent;
		public Vector2 inputPos;

		public NodeEditorInputInfo (NodeEditorState EditorState) 
		{
			message = null;
			editorState = EditorState;
			inputEvent = Event.current;
			inputPos = inputEvent.mousePosition;
		}

		public NodeEditorInputInfo (string Message, NodeEditorState EditorState) 
		{
			message = Message;
			editorState = EditorState;
			inputEvent = Event.current;
			inputPos = inputEvent.mousePosition;
		}

		public void SetAsCurrentEnvironment () 
		{
			NodeEditor.curEditorState = editorState;
			NodeEditor.curNodeCanvas = editorState.canvas;
		}
	}

	#region Event Attributes

	[AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
	public class EventHandlerAttribute : Attribute 
	{
		public EventType? handledEvent { get; private set; }
		public int priority { get; private set; }

			public EventHandlerAttribute(EventType eventType, int priorityValue)
		{
			handledEvent = eventType;
			priority = priorityValue;
		}

		public EventHandlerAttribute(int priorityValue)
		{
			handledEvent = null;
			priority = priorityValue;
		}

			public EventHandlerAttribute (EventType eventType) 
		{
			handledEvent = eventType;
			priority = 50;
		}

		public EventHandlerAttribute () 
		{
			handledEvent = null;
		}

		internal static bool AssureValidity (MethodInfo method, EventHandlerAttribute attr) 
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{ 
				ParameterInfo[] methodParams = method.GetParameters ();
				if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(NodeEditorInputInfo))
					return true;
				else
					Debug.LogWarning ("Method " + method.Name + " has incorrect signature for EventHandlerAttribute!");
			}
			return false;
		}
	}

	[AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
	public class HotkeyAttribute : Attribute 
	{
		public KeyCode handledHotKey { get; private set; }
		public EventModifiers? modifiers { get; private set; }
		public EventType? limitingEventType { get; private set; }
		public int priority { get; private set; }

			public HotkeyAttribute (KeyCode handledKey) 
		{
			handledHotKey = handledKey;
			modifiers = null;
			limitingEventType = null;
			priority = 50;
		}	

			public HotkeyAttribute (KeyCode handledKey, EventModifiers eventModifiers) 
		{
			handledHotKey = handledKey;
			modifiers = eventModifiers;
			limitingEventType = null;
			priority = 50;
		}

			public HotkeyAttribute (KeyCode handledKey, EventType LimitEventType) 
		{
			handledHotKey = handledKey;
			modifiers = null;
			limitingEventType = LimitEventType;
			priority = 50;
		}

			public HotkeyAttribute(KeyCode handledKey, EventType LimitEventType, int priorityValue)
		{
			handledHotKey = handledKey;
			modifiers = null;
			limitingEventType = LimitEventType;
			priority = priorityValue;
		}


		public HotkeyAttribute (KeyCode handledKey, EventModifiers eventModifiers, EventType LimitEventType) 
		{
			handledHotKey = handledKey;
			modifiers = eventModifiers;
			limitingEventType = LimitEventType;
			priority = 50;
		}

		public HotkeyAttribute (KeyCode handledKey, EventModifiers eventModifiers, EventType LimitEventType, int priorityValue) 
		{
			handledHotKey = handledKey;
			modifiers = eventModifiers;
			limitingEventType = LimitEventType;
			priority = priorityValue;
		}

		internal static bool AssureValidity (MethodInfo method, HotkeyAttribute attr) 
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] methodParams = method.GetParameters ();
				if (methodParams.Length == 1 && methodParams[0].ParameterType.IsAssignableFrom (typeof(NodeEditorInputInfo)))
					return true;
				else
					Debug.LogWarning ("Method " + method.Name + " has incorrect signature for HotkeyAttribute!");
			}
			return false;
		}
	}

	public enum ContextType { Node, Canvas, Toolbar }

	[AttributeUsage (AttributeTargets.Method)]
	public class ContextEntryAttribute : Attribute 
	{
		public ContextType contextType { get; private set; }
		public string contextPath { get; private set; }

		public ContextEntryAttribute (ContextType type, string path) 
		{
			contextType = type;
			contextPath = path;
		}

		internal static bool AssureValidity (MethodInfo method, ContextEntryAttribute attr) 
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{ 
				ParameterInfo[] methodParams = method.GetParameters ();
				if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(NodeEditorInputInfo))
					return true;
				else
					Debug.LogWarning ("Method " + method.Name + " has incorrect signature for ContextAttribute!");
			}
			return false;
		}
	}

	[AttributeUsage (AttributeTargets.Method)]
	public class ContextFillerAttribute : Attribute 
	{
		public ContextType contextType { get; private set; }

		public ContextFillerAttribute (ContextType type)
		{
			contextType = type;
		}

		internal static bool AssureValidity (MethodInfo method, ContextFillerAttribute attr) 
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{ 
				ParameterInfo[] methodParams = method.GetParameters ();
				if (methodParams.Length == 2 && methodParams[0].ParameterType == typeof(NodeEditorInputInfo) && methodParams[1].ParameterType == typeof(GenericMenu))
					return true;
				else
					Debug.LogWarning ("Method " + method.Name + " has incorrect signature for ContextAttribute!");
			}
			return false;
		}
	}

	#endregion
}