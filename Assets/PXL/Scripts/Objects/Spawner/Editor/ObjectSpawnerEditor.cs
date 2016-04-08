using UnityEngine;
using UnityEditor;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(ObjectSpawner))]
	public class ObjectSpawnerEditor : UnityEditor.Editor {

		/// <summary>
		/// Style for header labels
		/// </summary>
		protected readonly GUIStyle HeaderStyle = new GUIStyle() {
			fontSize = 12,
			fontStyle = FontStyle.Bold,
			alignment = TextAnchor.MiddleLeft
		};

		/// <summary>
		/// Creates a horizontal area with the given text and with the <see cref="HeaderStyle"/>
		/// </summary>
		/// <param name="text"></param>
		protected void Header(string text) {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUIStyle("Box"));
			EditorGUILayout.LabelField(text, HeaderStyle);
			GUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI() {
			var spawner = (ObjectSpawner) target;

			Header("General Spawner Options");
			spawner.SpawnKey = (KeyCode) EditorGUILayout.EnumPopup("Spawn object", spawner.SpawnKey);
			spawner.RemoveAllKey = (KeyCode) EditorGUILayout.EnumPopup("Remove all objects", spawner.RemoveAllKey);

			spawner.DefaultObjectPrefab =
				(GameObject) EditorGUILayout.ObjectField("Object Prefab", spawner.DefaultObjectPrefab, typeof(GameObject), false);
			if (spawner.DefaultObjectPrefab == null) {
				EditorGUILayout.HelpBox("Missing prefab!", MessageType.Warning);
			}

			spawner.SpawnedObjectsContainer =
				(Transform)
					EditorGUILayout.ObjectField(
						new GUIContent("Spawned Objects Container",
							"The parent for spawned objects. If not set, objects won't have a parent"), spawner.SpawnedObjectsContainer,
						typeof(Transform), true);

			spawner.IsSpawningEnabled =
				EditorGUILayout.Toggle(new GUIContent("Spawning Enabled", "Whether this spawner can spawn"),
					spawner.IsSpawningEnabled);

			spawner.StartAmount =
				Mathf.Clamp(
					EditorGUILayout.IntField(
						new GUIContent("Initial Object Amount", "How many objects are spawned automatically at the start"),
						spawner.StartAmount), 0, 100);

			EditorGUI.BeginDisabledGroup(spawner.StartAmount == 0);
			spawner.StartSpawnDelay =
				Mathf.Clamp(
					EditorGUILayout.IntField(
						new GUIContent("Initial Spawn Delay", "How long to wait before spawning any objects at the start"),
						spawner.StartSpawnDelay), 0, 1000);
			EditorGUI.EndDisabledGroup();

			spawner.StartSpawnFrequency =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Start Spawn Frequency",
							"The frequency which Initial Object Amount objects will be spawned with (after the Initial Spawn Delay)"),
						spawner.StartSpawnFrequency), 0.1f, 10f);

			spawner.RespawnOnDepleted =
				EditorGUILayout.Toggle(
					new GUIContent("Respawn On Depleted", "Whether to respawn the objects when all have been destroyed" +
					                                      " or when the amount of objects is less than Minimum Object Amount"),
					spawner.RespawnOnDepleted);

			EditorGUI.BeginDisabledGroup(!spawner.RespawnOnDepleted);
			spawner.RespawnDelay =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Respawn Delay",
							"How many seconds to wait before spawning a new object after all current ones are gone"), spawner.RespawnDelay),
					0, 100);

			spawner.MinObjectAmount =
				Mathf.Clamp(
					EditorGUILayout.IntField(
						new GUIContent("Minimum Object Amount",
							"The minimum number of objects that should exist at all times. " +
							"If there's too few, the necessary amount will be spawned (needs RespawnOnDepleted to be TRUE)"),
						spawner.MinObjectAmount), 1, int.MaxValue);
			EditorGUI.EndDisabledGroup();

			spawner.TotalSpawnLimit =
				Mathf.Clamp(
					EditorGUILayout.IntField(
						new GUIContent("Total Spawn Limit", "The total amount of objects this spawner can spawn in his life"),
						spawner.TotalSpawnLimit), -1, int.MaxValue);

			spawner.ConcurrentSpawnLimit =
				Mathf.Clamp(
					EditorGUILayout.IntField(
						new GUIContent("Concurrent Objects Limit",
							"The maximum allowed number of objects being around at the same time (spawned by this spawner)"),
						spawner.ConcurrentSpawnLimit), -1, spawner.TotalSpawnLimit == -1 ? int.MaxValue : spawner.TotalSpawnLimit);

			spawner.MinScaleFactor =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Min Scale Factor", "The minimum possible scale factor this spawner can apply"),
						spawner.MinScaleFactor), 0.1f, spawner.MaxScaleFactor);

			spawner.MaxScaleFactor =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Max Scale Factor", "The maximum possible scale factor this spawner can apply"),
						spawner.MaxScaleFactor), spawner.MinScaleFactor, 5.0f);

			spawner.DefaultScaleFactor =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Default Scale Factor", "The default scale factor this spawner applies"),
						spawner.DefaultScaleFactor), spawner.MinScaleFactor, spawner.MaxScaleFactor);

			spawner.SetObjectRotation = EditorGUILayout.BeginToggleGroup("Set Object Rotation", spawner.SetObjectRotation);
			spawner.ObjectRotation = EditorGUILayout.Vector3Field("Rotation", spawner.ObjectRotation, GUILayout.MinWidth(150));
			EditorGUILayout.EndToggleGroup();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Spawn", "Spawn an object")) && EditorApplication.isPlayingOrWillChangePlaymode &&
			    EditorApplication.isPlaying) {
				spawner.SpawnObject();
			}
			if (GUILayout.Button(new GUIContent("Clear", "Remove all current objects")) &&
			    EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying) {
				spawner.RemoveAllObjects();
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
		}

	}

}