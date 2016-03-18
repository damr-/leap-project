using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(PatternVisualizer))]
	public class PatternVisualizerEditor : UnityEditor.Editor {

		/// <summary>
		/// Style for header labels
		/// </summary>
		private readonly GUIStyle headerStyle = new GUIStyle() { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };

		private bool showPreviewObjects;

		public override void OnInspectorGUI() {
			var visualizer = (PatternVisualizer)target;

			visualizer.PreviewGameObject =
				(GameObject)
					EditorGUILayout.ObjectField(new GUIContent("Object Prefab", "The preview object to be used"),
						visualizer.PreviewGameObject, typeof(GameObject), false);

			visualizer.PreviewContainer =
				(Transform)
					EditorGUILayout.ObjectField(new GUIContent("Preview Objects Container", "Parent for spawned preview objects"),
						visualizer.PreviewContainer, typeof(Transform), false);

			showPreviewObjects = EditorGUILayout.Foldout(showPreviewObjects, "Preview Objects");
			if (showPreviewObjects) {
				EditorGUILayout.BeginVertical();
				foreach (var o in visualizer.PreviewObjects) {
					EditorGUILayout.ObjectField(o.gameObject.name, o, typeof (Transform), true);
				}
				EditorGUILayout.EndVertical();

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent("Refresh", "Delete all previews and recreate them"), GUILayout.MaxWidth(150))) {
					visualizer.Refresh();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

	/// <summary>
	/// Creates a horizontal area with the given text and with the <see cref="headerStyle"/>
	/// </summary>
	/// <param name="text"></param>
	private void Header(string text) {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUIStyle("Box"));
			EditorGUILayout.LabelField(text, headerStyle);
			GUILayout.EndHorizontal();
		}

	}

}