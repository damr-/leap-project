#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PXL.Utility {

	/// <summary>
	/// This class provides utility methods for creating custom editor scripts
	/// </summary>
	public static class EditorUtility {

		public static bool IsPlaying() {
			return EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying;
		}

		public static readonly GUIStyle HeaderStyle = new GUIStyle {
			fontSize = 12,
			fontStyle = FontStyle.Bold,
			alignment = TextAnchor.MiddleLeft
		};

		public static void FloatField(GUIContent labelContent, ref float floatVal, float min, float max,
			float labelWidth = 150, int indent = 0, bool endHorizontal = true) {
			BeginHorizontalField(labelContent, indent, labelWidth);
			floatVal = Mathf.Clamp(EditorGUILayout.FloatField(floatVal, GUILayout.MaxWidth(100)), min, max);
			if (endHorizontal)
				EditorGUILayout.EndHorizontal();
		}

		public static void IntField(GUIContent labelContent, ref int intVal, int min, int max, float labelWidth = 150,
			int indent = 0, bool endHorizontal = true) {
			BeginHorizontalField(labelContent, indent, labelWidth);
			intVal = Mathf.Clamp(EditorGUILayout.IntField(intVal, GUILayout.MaxWidth(100)), min, max);
			if (endHorizontal)
				EditorGUILayout.EndHorizontal();
		}

		public static void BeginHorizontalField(GUIContent labelContent, int indent, float labelWidth = 150) {
			EditorGUILayout.BeginHorizontal();
			if (indent > 0)
				EditorGUILayout.LabelField("", GUILayout.MinWidth(indent), GUILayout.MaxWidth(indent));
			EditorGUILayout.LabelField(labelContent, GUILayout.MaxWidth(labelWidth));
		}

		/// <summary>
		/// Creates a horizontal area with the given text and with the <see cref="HeaderStyle"/>
		/// </summary>
		/// <param name="text">The text content of the header</param>
		public static void Header(string text) {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUIStyle("Box"));
			EditorGUILayout.LabelField(text, HeaderStyle);
			GUILayout.EndHorizontal();
		}

	}

}
#endif