using UnityEngine;
using UnityEditor;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(AutomatedObjectSpawner))]
	public class AutomatedObjectSpawnerEditor : ObjectSpawnerEditor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			var spawner = (AutomatedObjectSpawner) target;

			Header("Automated Spawner Settings");

			EditorGUI.BeginDisabledGroup(spawner.ChooseRandomly);
			spawner.SpawnFrequency =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Spawn Frequency", "The frequency this spawner spawns objects with"),
						spawner.SpawnFrequency), 0.0001f, 1000);
			EditorGUI.EndDisabledGroup();
		
			spawner.ChooseRandomly = EditorGUILayout.BeginToggleGroup("Choose Frequency Randomly", spawner.ChooseRandomly);

			spawner.MinSpawnFrequency =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Max Spawn Frequency", "The maximum possible frequency for the spawner"),
						spawner.MinSpawnFrequency), 0.0001f, spawner.MaxSpawnFrequency);

			spawner.MaxSpawnFrequency =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Max Spawn Frequency", "The maximum possible frequency for the spawner"),
						spawner.MaxSpawnFrequency), spawner.MinSpawnFrequency, 1000);

			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
		}
	}

}