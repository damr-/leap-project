using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(PatternSpawnerVisualizer))]
	public class PatternSpawnerVisualizerEditor : UnityEditor.Editor {

		/// <summary>
		/// Whether the preview object list is currently visible
		/// </summary>
		private bool showPreviewObjects;

		public override void OnInspectorGUI() {
			var visualizer = (PatternSpawnerVisualizer)target;

			visualizer.PreviewGameObject =
				(GameObject)
					EditorGUILayout.ObjectField(new GUIContent("Object Prefab", "The preview object to be used"),
						visualizer.PreviewGameObject, typeof(GameObject), false);

			visualizer.RandomPreviewGameObject =
				(GameObject)
					EditorGUILayout.ObjectField(
						new GUIContent("Random Prefab",
							"The preview object used for showing a spot in the preview which will have an object assigned randomly"),
						visualizer.RandomPreviewGameObject, typeof(GameObject), false);

			visualizer.PossiblyRandomPreviewGameObject =
				(GameObject)
					EditorGUILayout.ObjectField(
						new GUIContent("Possibly Random Prefab",
							"The preview used for showing a spot in the preview which might be added due to the random column and row count"),
						visualizer.PossiblyRandomPreviewGameObject, typeof(GameObject), false);

			showPreviewObjects = EditorGUILayout.Foldout(showPreviewObjects, "Preview Objects");
			if (showPreviewObjects) {
				EditorGUILayout.BeginVertical();
				foreach (var o in visualizer.PreviewObjects)
					EditorGUILayout.ObjectField(o.gameObject.name, o, typeof(Transform), true);
				foreach (var o in visualizer.RandomPreviewObjects)
					EditorGUILayout.ObjectField(o.gameObject.name, o, typeof(Transform), true);
				foreach (var o in visualizer.PossiblyRandomPreviewObjects)
					EditorGUILayout.ObjectField(o.gameObject.name, o, typeof(Transform), true);
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Refresh", "Recreate the preview objects")))
				visualizer.Refresh();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

	}

}