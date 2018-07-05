using UnityEngine;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using _ShaderoShaderEditorFramework.Utilities;

namespace _ShaderoShaderEditorFramework
{
	public enum ConnectionDrawMethod { Bezier, StraightLine }

	public static class ConnectionTypes
	{
		private static Type NullType { get { return typeof(ConnectionTypes); } }
		private static Dictionary<string, TypeData> types;
		public static Type GetType (string typeName)
		{
			return GetTypeData (typeName).Type ?? NullType;
		}
		public static TypeData GetTypeData (string typeName)
		{
			if (types == null || types.Count == 0)
				FetchTypes ();
			TypeData typeData;
			if (!types.TryGetValue (typeName, out typeData))
			{
				Type type = Type.GetType (typeName);
				if (type == null)
				{
					typeData = types.First ().Value;
					Debug.LogError ("No TypeData defined for: " + typeName + " and type could not be found either");
				}
				else 
				{
					typeData = types.Values.Count <= 0? null : types.Values.First ((TypeData data) => data.isValid () && data.Type == type);
					if (typeData == null)
						types.Add (typeName, typeData = new TypeData (type));
				}
			}
			return typeData;
		}

		public static TypeData GetTypeData (Type type)
		{
			if (types == null || types.Count == 0)
				FetchTypes ();
			TypeData typeData = types.Values.Count <= 0? null : types.Values.First ((TypeData data) => data.isValid () && data.Type == type);
			if (typeData == null)
				types.Add (type.Name, typeData = new TypeData (type));
			return typeData;
		}
		
		internal static void FetchTypes () 
		{
			types = new Dictionary<string, TypeData> { { "None", new TypeData (typeof(System.Object)) } };

			IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly"));
			foreach (Assembly assembly in scriptAssemblies) 
			{
				foreach (Type type in assembly.GetTypes ().Where (T => T.IsClass && !T.IsAbstract && T.GetInterfaces ().FixContains (typeof (IConnectionTypeDeclaration)))) 
				{
					IConnectionTypeDeclaration typeDecl = assembly.CreateInstance (type.FullName) as IConnectionTypeDeclaration;
					if (typeDecl == null)
						throw new UnityException ("Error with Type Declaration " + type.FullName);
					types.Add (typeDecl.Identifier, new TypeData (typeDecl));
				}
			}
		}
	}

	public class TypeData 
	{
		private IConnectionTypeDeclaration declaration;
		public string Identifier { get; private set; }
		public Type Type { get; private set; }
		public Color Color { get; private set; }
		public Texture2D InKnobTex { get; private set; }
		public Texture2D OutKnobTex { get; private set; }
		
		internal TypeData (IConnectionTypeDeclaration typeDecl) 
		{
			Identifier = typeDecl.Identifier;
			declaration = typeDecl;
			Type = declaration.Type;
			Color = declaration.Color;

			InKnobTex = ResourceManager.GetTintedTexture (declaration.InKnobTex, Color);
			OutKnobTex = ResourceManager.GetTintedTexture (declaration.OutKnobTex, Color);

			if (!isValid ())
				throw new DataMisalignedException ("Type Declaration " + typeDecl.Identifier + " contains invalid data!");
		}

		public TypeData (Type type) 
		{
			Identifier = type.Name;
			declaration = null;
			Type = type;
			Color = Color.white;

			int srcInt = type.GetHashCode ();
			byte[] bytes = BitConverter.GetBytes (srcInt);
			Color = new Color (Mathf.Pow (((float)bytes[0])/255, 0.5f), Mathf.Pow (((float)bytes[1])/255, 0.5f), Mathf.Pow (((float)bytes[2])/255, 0.5f));

			InKnobTex = ResourceManager.GetTintedTexture ("Textures/In_Knob.png", Color);
			OutKnobTex = ResourceManager.GetTintedTexture ("Textures/Out_Knob.png", Color);
		}

		public bool isValid () 
		{
			return Type != null && InKnobTex != null && OutKnobTex != null;
		}
	}

	public interface IConnectionTypeDeclaration
	{
		string Identifier { get; }
		Type Type { get; }
		Color Color { get; }
		string InKnobTex { get; }
		string OutKnobTex { get; }
	}

	public class FloatType : IConnectionTypeDeclaration 
	{
		public string Identifier { get { return "SuperFloat"; } }
		public Type Type { get { return typeof(SuperFloat); } }
		public Color Color { get { return new Color(0.5f,0.5f,1); } }
		public string InKnobTex { get { return "Textures/In_Knob_sf.png"; } }
		public string OutKnobTex { get { return "Textures/Out_Knob_sf.png"; } }
	}
    public class SuperFloat2Type : IConnectionTypeDeclaration
    {
        public string Identifier { get { return "SuperFloat2"; } }
        public Type Type { get { return typeof(SuperFloat2); } }
        public Color Color { get { return Color.yellow; } }
        public string InKnobTex { get { return "Textures/In_Knob_sf2.png"; } }
        public string OutKnobTex { get { return "Textures/Out_Knob_sf2.png"; } }
    }
    public class SuperFloat4Type : IConnectionTypeDeclaration
    {
        public string Identifier { get { return "SuperFloat4"; } }
        public Type Type { get { return typeof(SuperFloat4); } }
        public Color Color { get { return Color.white; } }
        public string InKnobTex { get { return "Textures/In_Knob.png"; } }
        public string OutKnobTex { get { return "Textures/Out_Knob.png"; } }
    }
    public class SuperSourceType : IConnectionTypeDeclaration
    {
        public string Identifier { get { return "SuperSuperSource"; } }
        public Type Type { get { return typeof(SuperSource); } }
        public Color Color { get { return Color.cyan; } }
        public string InKnobTex { get { return "Textures/In_Knob_source.png"; } }
        public string OutKnobTex { get { return "Textures/Out_Knob_source.png"; } }
    }

    

  


}

namespace System.Linq
{
    public static class EnumerableExt
    {
        public static bool FixContains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return System.Linq.Enumerable.Contains(source, value);
        }
        public static bool FixContains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            return System.Linq.Enumerable.Contains(source, value, comparer);
        }

    }
}