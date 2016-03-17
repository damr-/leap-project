using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(PatternSpawner))]
	public class PatternSpawnerEditor : UnityEditor.Editor {

		/// <summary>
		/// The layout options for a pattern toggle button
		/// </summary>
		private readonly GUILayoutOption[] patternToggleButtonOptions = {
			GUILayout.MinHeight(20), GUILayout.MinWidth(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)
		};

		/// <summary>
		/// Style for header labels
		/// </summary>
		private readonly GUIStyle headerStyle = new GUIStyle() { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };

		public override void OnInspectorGUI() {
			var spawner = (PatternSpawner)target;

			Header("General Spawner Options");
			spawner.SpawnKey = (KeyCode)EditorGUILayout.EnumPopup("Spawn object", spawner.SpawnKey);
			spawner.RemoveAllKey = (KeyCode)EditorGUILayout.EnumPopup("Remove all objects", spawner.RemoveAllKey);

			spawner.DefaultObjectPrefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", spawner.DefaultObjectPrefab, typeof(GameObject), false);

			spawner.IsSpawningEnabled = EditorGUILayout.Toggle(new GUIContent("Spawning Enabled", "Whether this spawner can spawn"), spawner.IsSpawningEnabled);

			spawner.StartAmount = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Initial Object Amount", "How many objects are spawned automatically at the start"), spawner.StartAmount), 0, 100);

			EditorGUI.BeginDisabledGroup(spawner.StartAmount == 0);
			spawner.StartSpawnDelay = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Initial Spawn Delay", "How long to wait before spawning any objects at the start"), spawner.StartSpawnDelay), 0, 1000);
			EditorGUI.EndDisabledGroup();

			spawner.RespawnDelay = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Respawn Delay", "How many seconds to wait before spawning a new object after all current ones are gone"), spawner.RespawnDelay), 0, 1000);

			spawner.TotalSpawnLimit = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Total Spawn Limit", "The total amount of objects this spawner can spawn in his life"), spawner.TotalSpawnLimit), -1, int.MaxValue);

			spawner.ConcurrentSpawnLimit = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Concurrent Objects Limit", "The maximum allowed number of objects being around at the same time (spawned by this spawner)"), spawner.ConcurrentSpawnLimit), -1, spawner.TotalSpawnLimit == -1 ? int.MaxValue : spawner.TotalSpawnLimit);

			spawner.MinScaleFactor = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Min Scale Factor", "The minimum possible scale factor this spawner can apply"), spawner.MinScaleFactor), 0.1f, spawner.MaxScaleFactor);

			spawner.MaxScaleFactor = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Max Scale Factor", "The maximum possible scale factor this spawner can apply"), spawner.MaxScaleFactor), spawner.MinScaleFactor, 5.0f);

			spawner.DefaultScaleFactor = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Default Scale Factor", "The default scale factor this spawner applies"), spawner.DefaultScaleFactor), spawner.MinScaleFactor, spawner.MaxScaleFactor);

			EditorGUILayout.Space();

			Header("Pattern Spawner Options");
			var rowsTooltip = "The height of the available pattern [1," + PatternSpawner.MaxPatternRows + "]";
			var rowAmount = EditorGUILayout.IntSlider(new GUIContent("Pattern Height", rowsTooltip), spawner.PatternRows, 1, PatternSpawner.MaxPatternRows);
			spawner.SetRows(rowAmount);

			var columnsTooltip = "The width of the available pattern [1," + PatternSpawner.MaxPatternColumns + "]";
			var columnAmount = EditorGUILayout.IntSlider(new GUIContent("Pattern Width", columnsTooltip), spawner.PatternColumns, 1, PatternSpawner.MaxPatternColumns);
			spawner.SetColumns(columnAmount);

			#region PatternButtons
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Clear", "Removes all checks"))) {
				spawner.ClearPattern();
			}
			if (GUILayout.Button(new GUIContent("Invert", "Invert the current pattern"))) {
				spawner.InvertPattern();
			}
			if (GUILayout.Button(new GUIContent("Random", "Create a random pattern"))) {
				spawner.CreateRandomPattern();
			}
			EditorGUILayout.EndHorizontal();
			#endregion

			for (var row = 0; row < spawner.PatternRows; row++) {
				EditorGUILayout.BeginHorizontal();
				for (var column = 0; column < spawner.PatternColumns; column++) {
					spawner.SpawnPattern[column][row] = EditorGUILayout.Toggle("", spawner.SpawnPattern[column][row], patternToggleButtonOptions);
				}
				EditorGUILayout.EndHorizontal();
			}

			spawner.ColumnMargin = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Column Margin", "The space between columns"), spawner.ColumnMargin), 0, 5);

			spawner.RowMargin = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Row Margin", "The space between rows"), spawner.RowMargin), 0, 5);
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
