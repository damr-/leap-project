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
			var spawner = (ObjectSpawner)target;

			Header("General Spawner Options");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Spawn object", GUILayout.MaxWidth(150));
			spawner.SpawnKey = (KeyCode)EditorGUILayout.EnumPopup("", spawner.SpawnKey, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Remove all objects", GUILayout.MaxWidth(150));
			spawner.RemoveAllKey = (KeyCode)EditorGUILayout.EnumPopup("", spawner.RemoveAllKey, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Object Prefab", GUILayout.MaxWidth(150));
			spawner.DefaultObjectPrefab =
				(GameObject)
					EditorGUILayout.ObjectField("", spawner.DefaultObjectPrefab, typeof(GameObject), false, GUILayout.MaxWidth(150));
			if (spawner.DefaultObjectPrefab == null)
				EditorGUILayout.HelpBox("Missing prefab!", MessageType.Warning);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(
				new GUIContent("Spawned Objects Container",
					"The parent for spawned objects. If not set, objects won't have a parent"), GUILayout.MaxWidth(150));
			spawner.SpawnedObjectsContainer =
				(Transform)
					EditorGUILayout.ObjectField("", spawner.SpawnedObjectsContainer,
						typeof(Transform), true, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Spawning Enabled", "Whether this spawner can spawn"),
				GUILayout.MaxWidth(150), GUILayout.MinWidth(150));
			spawner.IsSpawningEnabled = EditorGUILayout.Toggle("", spawner.IsSpawningEnabled, GUILayout.MaxWidth(20));
			EditorGUILayout.EndHorizontal();

			IntField(new GUIContent("Initial Object Amount", "How many objects are spawned automatically at the start"),
				ref spawner.StartAmount, 0, 100);

			EditorGUI.BeginDisabledGroup(spawner.StartAmount == 0);
			IntField(new GUIContent("Initial Spawn Delay", "How long to wait before spawning any objects at the start"),
				ref spawner.StartSpawnDelay, 0, 100);
			EditorGUI.EndDisabledGroup();

			FloatField(new GUIContent("Start Spawn Frequency",
				"The frequency which Initial Object Amount objects will be spawned with (after the Initial Spawn Delay)"),
				ref spawner.StartSpawnFrequency, 0.1f, 10f);



			spawner.RespawnOnDepleted = EditorGUILayout.BeginToggleGroup(
				new GUIContent("Respawn On Depleted", "Whether to respawn the objects when all have been destroyed" +
				                                      " or when the amount of objects is less than Minimum Object Amount"),
				spawner.RespawnOnDepleted);

			FloatField(
				new GUIContent("Respawn Delay",
					"How many seconds to wait before spawning a new object after all current ones are gone"), ref spawner.RespawnDelay,
				0, 100, 25);

			FloatField(
				new GUIContent("Respawn Frequency",
					"How many objects are spawned when respawning objects because all have been destroyed"),
				ref spawner.RespawnFrequency,
				0.1f, 10f, 25);

			IntField(
				new GUIContent("Minimum Object Amount",
					"The minimum number of objects that should exist at all times. " +
					"If there's too few, the necessary amount will be spawned (needs RespawnOnDepleted to be TRUE)"),
				ref spawner.MinObjectAmount, 1, int.MaxValue, 25);

			EditorGUILayout.EndToggleGroup();



			IntField(new GUIContent("Total Spawn Limit", "The total amount of objects this spawner can spawn in his life"),
				ref spawner.TotalSpawnLimit, -1, int.MaxValue);

			IntField(new GUIContent("Concurrent Objects Limit",
				"The maximum allowed number of objects being around at the same time (spawned by this spawner)"),
				ref spawner.ConcurrentSpawnLimit, -1,
				spawner.TotalSpawnLimit == -1 ? int.MaxValue : spawner.TotalSpawnLimit);

			FloatField(
				new GUIContent("Min Scale Factor", "The minimum possible scale factor this spawner can apply"), ref
					spawner.MinScaleFactor, 0.1f, spawner.MaxScaleFactor);

			FloatField(new GUIContent("Max Scale Factor", "The maximum possible scale factor this spawner can apply"),
				ref spawner.MaxScaleFactor, spawner.MinScaleFactor, 5.0f);

			FloatField(
				new GUIContent("Default Scale Factor", "The default scale factor this spawner applies"),
				ref spawner.DefaultScaleFactor, spawner.MinScaleFactor, spawner.MaxScaleFactor);

			spawner.SetObjectRotation = EditorGUILayout.BeginToggleGroup("Set Object Rotation", spawner.SetObjectRotation);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MinWidth(25), GUILayout.MaxWidth(25));
			EditorGUILayout.LabelField("Rotation", GUILayout.MaxWidth(75));
			spawner.ObjectRotation = EditorGUILayout.Vector3Field("", spawner.ObjectRotation, GUILayout.MaxWidth(175),
				GUILayout.MinWidth(175));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Spawn", "Spawn an object")) &&
			    EditorApplication.isPlayingOrWillChangePlaymode &&
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

		protected static void FloatField(GUIContent labelContent, ref float floatVal, float min, float max, int indent = 0) {
			BeginHorizontalField(labelContent, indent);
			floatVal = Mathf.Clamp(EditorGUILayout.FloatField(floatVal, GUILayout.MaxWidth(100)), min, max);
			EditorGUILayout.EndHorizontal();
		}

		protected static void IntField(GUIContent labelContent, ref int intVal, int min, int max, int indent = 0) {
			BeginHorizontalField(labelContent, indent);
			intVal = Mathf.Clamp(EditorGUILayout.IntField(intVal, GUILayout.MaxWidth(100)), min, max);
			EditorGUILayout.EndHorizontal();
		}

		private static void BeginHorizontalField(GUIContent labelContent, int indent) {
			EditorGUILayout.BeginHorizontal();
			if(indent > 0)
				EditorGUILayout.LabelField("", GUILayout.MinWidth(indent), GUILayout.MaxWidth(indent));
			EditorGUILayout.LabelField(labelContent, GUILayout.MaxWidth(150));
		}
	}

}