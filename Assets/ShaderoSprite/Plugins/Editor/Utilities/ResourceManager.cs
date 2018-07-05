using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace _ShaderoShaderEditorFramework.Utilities 
{
	public static class ResourceManager 
	{

		private static string _ResourcePath = "";
		public static void SetDefaultResourcePath (string defaultResourcePath) 
		{
			_ResourcePath = defaultResourcePath;
		}

		#region Common Resource Loading

		public static string PreparePath (string path) 
		{

            path = path.Replace (Application.dataPath, "Assets");
            #if UNITY_EDITOR
            if (!path.StartsWith ("Assets/"))
			path = _ResourcePath + path;
            return path;
            #endif

        }

		public static T[] LoadResources<T> (string path) where T : UnityEngine.Object
		{
			path = PreparePath (path);
		#if UNITY_EDITOR 
			return UnityEditor.AssetDatabase.LoadAllAssetsAtPath (path).OfType<T> ().ToArray ();
		#endif
		}

		public static T LoadResource<T> (string path) where T : UnityEngine.Object
		{
			path = PreparePath (path);
   	    #if UNITY_EDITOR 
			return UnityEditor.AssetDatabase.LoadAssetAtPath<T> (path);
		#endif
		}

		#endregion

		#region Texture Management

		private static List<MemoryTexture> loadedTextures = new List<MemoryTexture> ();

			public static Texture2D LoadTexture (string texPath)
		{
			if (String.IsNullOrEmpty (texPath))
				return null;
			int existingInd = loadedTextures.FindIndex ((MemoryTexture memTex) => memTex.path == texPath);
			if (existingInd != -1) 
			{ 
				if (loadedTextures[existingInd].texture == null)
					loadedTextures.RemoveAt (existingInd);
				else
					return loadedTextures[existingInd].texture;
			}
		
			Texture2D tex = LoadResource<Texture2D> (texPath);
			AddTextureToMemory (texPath, tex);
			return tex;
		}

		public static Texture2D GetTintedTexture (string texPath, Color col) 
		{
			string texMod = "Tint:" + col.ToString ();
			Texture2D tintedTexture = GetTexture (texPath, texMod);
			if (tintedTexture == null)
			{ 
				tintedTexture = LoadTexture (texPath);
				AddTextureToMemory (texPath, tintedTexture); 
				tintedTexture = _ShaderoShaderEditorFramework.Utilities.RTEditorGUI.Tint (tintedTexture, col);
				AddTextureToMemory (texPath, tintedTexture, texMod); 
			}
           
            return tintedTexture;
		}
		
		public static void AddTextureToMemory (string texturePath, Texture2D texture, params string[] modifications)
		{
			if (texture == null) return;
			loadedTextures.Add (new MemoryTexture (texturePath, texture, modifications));
		}
		
		public static MemoryTexture FindInMemory (Texture2D tex)
		{
			int existingInd = loadedTextures.FindIndex ((MemoryTexture memTex) => memTex.texture == tex);
			return existingInd != -1? loadedTextures[existingInd] : null;
		}
		
		public static bool HasInMemory (string texturePath, params string[] modifications)
		{
			int existingInd = loadedTextures.FindIndex ((MemoryTexture memTex) => memTex.path == texturePath);
			return existingInd != -1 && EqualModifications (loadedTextures[existingInd].modifications, modifications);
		}
		
		public static MemoryTexture GetMemoryTexture (string texturePath, params string[] modifications)
		{
			List<MemoryTexture> textures = loadedTextures.FindAll ((MemoryTexture memTex) => memTex.path == texturePath);
			if (textures == null || textures.Count == 0)
				return null;
			foreach (MemoryTexture tex in textures)
				if (EqualModifications (tex.modifications, modifications))
					return tex;
			return null;
		}
		
		public static Texture2D GetTexture (string texturePath, params string[] modifications)
		{
			MemoryTexture memTex = GetMemoryTexture (texturePath, modifications);
			return memTex == null? null : memTex.texture;
		}
		
		private static bool EqualModifications (string[] modsA, string[] modsB) 
		{
			return modsA.Length == modsB.Length && Array.TrueForAll (modsA, mod => modsB.Count (oMod => mod == oMod) == modsA.Count (oMod => mod == oMod));
		}
		
		public class MemoryTexture 
		{
			public string path;
			public Texture2D texture;
			public string[] modifications;
			
			public MemoryTexture (string texPath, Texture2D tex, params string[] mods) 
			{
				path = texPath;
				texture = tex;
				modifications = mods;
			}
		}
		
		#endregion
	}

}