using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(PatternSpawner))]
	public class PatternSpawnerEditor : ObjectSpawnerEditor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			var spawner = (PatternSpawner)target;

			Header("Pattern Spawner Options");
			var rowsTooltip = "The height of the available pattern [1," + PatternSpawner.MaxPatternRows + "]";
			var rowAmount = EditorGUILayout.IntSlider(new GUIContent("Pattern Height", rowsTooltip), spawner.PatternRows, 1, PatternSpawner.MaxPatternRows);
			spawner.SetRows(rowAmount);

			var columnsTooltip = "The width of the available pattern [1," + PatternSpawner.MaxPatternColumns + "]";
			var columnAmount = EditorGUILayout.IntSlider(new GUIContent("Pattern Width", columnsTooltip), spawner.PatternColumns, 1, PatternSpawner.MaxPatternColumns);
			spawner.SetColumns(columnAmount);

			#region Pattern
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Pattern", new GUIStyle() { fontSize = 12, fontStyle = FontStyle.Bold });

			#region PatternGrid
			for (var row = 0; row < spawner.PatternRows; row++) {
				EditorGUILayout.BeginHorizontal();
				for (var column = 0; column < spawner.PatternColumns; column++) {
					spawner.SpawnPattern[column][row] = EditorGUILayout.Toggle("", spawner.SpawnPattern[column][row], PatternToggleButtonOptions);
				}
				EditorGUILayout.EndHorizontal();
			}
			#endregion

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


			spawner.ColumnMargin = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Column Margin", "The space between columns"), spawner.ColumnMargin), 0, 5);

			spawner.RowMargin = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Row Margin", "The space between rows"), spawner.RowMargin), 0, 5);
			#endregion
			EditorGUILayout.Space();
		}

	}
}
