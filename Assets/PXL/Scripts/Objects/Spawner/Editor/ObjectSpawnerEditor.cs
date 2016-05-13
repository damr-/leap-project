using UnityEngine;
using UnityEditor;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(ObjectSpawner))]
	public class ObjectSpawnerEditor : UnityEditor.Editor {

		/// <summary>
		/// Creates a horizontal area with the given text and with the <see cref="Utility.EditorUtility.HeaderStyle"/>
		/// </summary>
		/// <param name="text"></param>
		protected void Header(string text) {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUIStyle("Box"));
			EditorGUILayout.LabelField(text, Utility.EditorUtility.HeaderStyle);
			GUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI() {
			var spawner = (ObjectSpawner)target;

			Header("General Spawner Options");

			Utility.EditorUtility.BeginHorizontalField(new GUIContent("Spawn object", "They key assigned to spawning a new object"), 0);
			spawner.SpawnKey = (KeyCode)EditorGUILayout.EnumPopup("", spawner.SpawnKey, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.BeginHorizontalField(
				new GUIContent("Remove all objects", "They key assigned to removing all objects spawned by this spawner"), 0);
			spawner.RemoveAllKey = (KeyCode)EditorGUILayout.EnumPopup("", spawner.RemoveAllKey, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.BeginHorizontalField(
				new GUIContent("Admin Mode required", "Whether admin mode is required for the keyboard input"), 0);
			spawner.AdminModeRequired = EditorGUILayout.Toggle("", spawner.AdminModeRequired, GUILayout.MaxWidth(20));
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.BeginHorizontalField(
				new GUIContent("Add to Hand Menu", "Whether this spawner will be visible and editable in the admin hand menu"), 0);
			spawner.InHandMenu = EditorGUILayout.Toggle("", spawner.InHandMenu, GUILayout.MaxWidth(20));
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.BeginHorizontalField(new GUIContent("Object Prefab", "The prefab which will be spawned by default"), 0);
			spawner.DefaultObjectPrefab =
				(GameObject)
					EditorGUILayout.ObjectField("", spawner.DefaultObjectPrefab, typeof(GameObject), false, GUILayout.MaxWidth(150));
			if (spawner.DefaultObjectPrefab == null)
				EditorGUILayout.HelpBox("Missing prefab!", MessageType.Warning);
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.BeginHorizontalField(
				new GUIContent("Spawned Objects Container",
					"The parent for spawned objects. If not set, objects won't have a parent"), 0);
			spawner.SpawnedObjectsContainer =
				(Transform)
					EditorGUILayout.ObjectField("", spawner.SpawnedObjectsContainer,
						typeof(Transform), true, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.BeginHorizontalField(new GUIContent("Spawning Enabled", "Whether this spawner can spawn"), 0);
			spawner.IsSpawningEnabled = EditorGUILayout.Toggle("", spawner.IsSpawningEnabled, GUILayout.MaxWidth(20));
			EditorGUILayout.EndHorizontal();

			Utility.EditorUtility.IntField(new GUIContent("Initial Object Amount", "How many objects are spawned automatically at the start"),
				ref spawner.StartAmount, 0, 100);

			EditorGUI.BeginDisabledGroup(spawner.StartAmount == 0);
			Utility.EditorUtility.IntField(new GUIContent("Initial Spawn Delay", "How long to wait before spawning any objects at the start"),
				ref spawner.StartSpawnDelay, 0, 100);

			Utility.EditorUtility.FloatField(new GUIContent("Start Spawn Frequency",
				"The frequency which Initial Object Amount objects will be spawned with (after the Initial Spawn Delay)"),
				ref spawner.StartSpawnFrequency, 0.1f, 10f);
			EditorGUI.EndDisabledGroup();

			spawner.RespawnOnDepleted = EditorGUILayout.BeginToggleGroup(
				new GUIContent("Respawn On Depleted", "Whether to respawn the objects when all have been destroyed" +
													  " or when the amount of objects is less than Minimum Object Amount"),
				spawner.RespawnOnDepleted);

			Utility.EditorUtility.FloatField(
				new GUIContent("Respawn Delay",
					"How many seconds to wait before spawning a new object after all current ones are gone"), ref spawner.RespawnDelay,
				0, 100, 150, 25);

			Utility.EditorUtility.FloatField(
				new GUIContent("Respawn Frequency",
					"How many objects are spawned when respawning objects because all have been destroyed"),
				ref spawner.RespawnFrequency,
				0.1f, 10f, 150, 25);

			Utility.EditorUtility.IntField(
				new GUIContent("Minimum Object Amount",
					"The minimum number of objects that should exist at all times. " +
					"If there's too few, the necessary amount will be spawned (needs RespawnOnDepleted to be TRUE)"),
				ref spawner.MinObjectAmount, 1, int.MaxValue, 150, 25);

			EditorGUILayout.EndToggleGroup();


			Utility.EditorUtility.IntField(new GUIContent("Total Spawn Limit", "The total amount of objects this spawner can spawn in his life"),
				ref spawner.TotalSpawnLimit, -1, int.MaxValue);

			Utility.EditorUtility.IntField(new GUIContent("Concurrent Objects Limit",
				"The maximum allowed number of objects being around at the same time (spawned by this spawner)"),
				ref spawner.ConcurrentSpawnLimit, -1,
				spawner.TotalSpawnLimit == -1 ? int.MaxValue : spawner.TotalSpawnLimit);

			Utility.EditorUtility.FloatField(
				new GUIContent("Min Scale Factor", "The minimum possible scale factor this spawner can apply"), ref
					spawner.MinScaleFactor, 0.1f, spawner.MaxScaleFactor);

			Utility.EditorUtility.FloatField(new GUIContent("Max Scale Factor", "The maximum possible scale factor this spawner can apply"),
				ref spawner.MaxScaleFactor, spawner.MinScaleFactor, 5.0f);

			Utility.EditorUtility.FloatField(
				new GUIContent("Default Scale Factor", "The default scale factor this spawner applies"),
				ref spawner.DefaultScaleFactor, spawner.MinScaleFactor, spawner.MaxScaleFactor);

			spawner.SetObjectRotation = EditorGUILayout.BeginToggleGroup("Set Object Rotation", spawner.SetObjectRotation);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MinWidth(25), GUILayout.MaxWidth(25));
			EditorGUILayout.LabelField("Rotation", GUILayout.MaxWidth(60));
			spawner.ObjectRotation = EditorGUILayout.Vector3Field("", spawner.ObjectRotation, GUILayout.MaxWidth(175),
				GUILayout.MinWidth(175));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Spawn", "Spawn an object")) && Utility.EditorUtility.IsPlaying() && spawner.enabled)
				spawner.SpawnObject();
			if (GUILayout.Button(new GUIContent("Clear", "Remove all current objects")) && Utility.EditorUtility.IsPlaying() && spawner.enabled)
				spawner.RemoveAllObjects();

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
		}

	}

}