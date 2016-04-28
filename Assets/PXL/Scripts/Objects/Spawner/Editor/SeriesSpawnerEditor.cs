using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(SeriesSpawner))]
	public class SeriesSpawnerEditor : ObjectSpawnerEditor {

		/// <summary>
		/// Whether the scales list is currently visible
		/// </summary>
		private bool showScales;

		/// <summary>
		/// The spawner this editor script references
		/// </summary>
		private SeriesSpawner spawner;

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			spawner = (SeriesSpawner)target;

			Header("Series Spawner Options");

			FloatField(new GUIContent("Series Spawn Frequency", "How many objects of the series are spawned per second"), ref spawner.SeriesSpawnFrequency, 0.1f, 10f);

			showScales = EditorGUILayout.Foldout(showScales, "Object Scales");
			if (showScales) {
				for (var i = 0; i < spawner.Scales.Count; i++)
					DisplayScaleEntry(i);

				if (spawner.Scales.Count == 0) {
					EditorGUILayout.LabelField("None",
						new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.BoldAndItalic },
						GUILayout.MaxWidth(50));
				}
			}

			DisplayAddScaleRegion();
		}

		private void DisplayAddScaleRegion() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add new scale: ", GUILayout.MaxWidth(100));
			spawner.NewScale = Mathf.Clamp(EditorGUILayout.FloatField("", spawner.NewScale, GUILayout.MaxWidth(75)), 0.1f, 10f);
			if (GUILayout.Button("Add", GUILayout.MaxWidth(100)))
				spawner.AddNewScale();
			EditorGUILayout.EndHorizontal();
		}

		private void DisplayScaleEntry(int i) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("#" + i, GUILayout.MaxWidth(25));
			spawner.Scales[i] = Mathf.Clamp(
				EditorGUILayout.FloatField("", spawner.Scales[i], GUILayout.MaxWidth(75)),
				0.1f, 10f);
			if (GUILayout.Button(new GUIContent("X", "Removes this item from the list"), GUILayout.MaxWidth(20),
				GUILayout.MaxHeight(20))) {
				spawner.Scales.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}

	}

}