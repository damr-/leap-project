using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(PatternSpawner))]
	public class PatternSpawnerEditor : ObjectSpawnerEditor {

		/// <summary>
		/// The layout options for a pattern toggle button
		/// </summary>
		protected readonly GUILayoutOption[] PatternToggleButtonOptions = {
			GUILayout.MinHeight(20), GUILayout.MinWidth(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)
		};

		private const string RandomHelpText = "Normal '?' represent the guaranteed size of the pattern which will be randomly filled. " +
											  "Greyed out '?' are fields which might be added due to the random amount of columns and rows.";

		private const string NormalHelpText = "Checked boxes represent spots where the spawner will place an object. Unchecked boxes represent empty spots where no object will be placed.";


		private bool displayPatternHelp;

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			var spawner = (PatternSpawner)target;

			Utility.EditorUtility.Header("Pattern Spawner Options");

			spawner.PatternFinishPoints =
				Mathf.Clamp(
				EditorGUILayout.IntField(
					new GUIContent("Win Points",
						"How many points are added the the GameMode's CurrentPoints when this pattern has been finished"),
					spawner.PatternFinishPoints), 0, int.MaxValue);

			EditorGUILayout.Space();

			spawner.RandomizePattern = EditorGUILayout.ToggleLeft("Randomise Pattern", spawner.RandomizePattern);
			EditorGUILayout.Space();

			if (spawner.RandomizePattern) {
				spawner.RandomizeRowCount = EditorGUILayout.ToggleLeft("Randomise Row Count", spawner.RandomizeRowCount);
				if (spawner.RandomizeRowCount) {
					spawner.MinRandomRowCount =
						EditorGUILayout.IntSlider(new GUIContent("Min Row Count", "The minimum possible row count"),
							spawner.MinRandomRowCount, 1, PatternSpawner.MaxPatternRows);
					spawner.MaxRandomRowCount =
						EditorGUILayout.IntSlider(new GUIContent("Max Row Count", "The maximum possible row count"),
							spawner.MaxRandomRowCount, spawner.MinRandomRowCount, PatternSpawner.MaxPatternRows);
					EditorGUILayout.Space();
				}
			}
			else {
				spawner.RandomizeRowCount = false;
				spawner.RandomizeColumnCount = false;
			}

			if (!spawner.RandomizeRowCount) {
				EditorGUI.BeginDisabledGroup(spawner.RandomizeRowCount);
				var rowAmount = EditorGUILayout.IntSlider(
					new GUIContent("Pattern Row Count", "The amount of rows of the available pattern"),
					spawner.PatternRows, 1, PatternSpawner.MaxPatternRows);
				spawner.SetRows(rowAmount);
				EditorGUI.EndDisabledGroup();
				if (spawner.RandomizePattern)
					EditorGUILayout.Space();
			}

			if (spawner.RandomizePattern) {
				spawner.RandomizeColumnCount = EditorGUILayout.ToggleLeft("Randomise Column Count", spawner.RandomizeColumnCount);
				if (spawner.RandomizeColumnCount) {
					spawner.MinRandomColumnCount =
						EditorGUILayout.IntSlider(new GUIContent("Min Column Count", "The minimum possible column count"),
							spawner.MinRandomColumnCount, 1, PatternSpawner.MaxPatternColumns);
					spawner.MaxRandomColumnCount =
						EditorGUILayout.IntSlider(new GUIContent("Max Column Count", "The maximum possible column count"),
							spawner.MaxRandomColumnCount, spawner.MinRandomColumnCount, PatternSpawner.MaxPatternColumns);
					EditorGUILayout.Space();
				}
			}

			if (!spawner.RandomizeColumnCount) {
				EditorGUI.BeginDisabledGroup(spawner.RandomizeColumnCount);
				var columnAmount = EditorGUILayout.IntSlider(
					new GUIContent("Pattern Column Count", "The amount of columns of the available pattern"),
					spawner.PatternColumns, 1, PatternSpawner.MaxPatternColumns);
				spawner.SetColumns(columnAmount);
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.Space();
			}

			spawner.ColumnMargin = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Column Margin", "The space between columns"), spawner.ColumnMargin), 0, 5);
			spawner.RowMargin = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Row Margin", "The space between rows"), spawner.RowMargin), 0, 5);

			#region Pattern
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Pattern", new GUIStyle() { fontSize = 12, fontStyle = FontStyle.Bold });
			if (GUILayout.Button(new GUIContent((displayPatternHelp ? "Hide" : "Show") + " Help", "Show some text explaining the pattern"))) {
				displayPatternHelp = !displayPatternHelp;
			}
			EditorGUILayout.EndHorizontal();

			if (displayPatternHelp) {
				EditorGUILayout.HelpBox(
					"Below you see a preview for the generated pattern.\n" + (spawner.RandomizePattern ? RandomHelpText : NormalHelpText),
					MessageType.Info,
					true);
			}

			#region PatternGrid
			var rowCount = spawner.RandomizeRowCount ? spawner.MaxRandomRowCount : spawner.PatternRows;
			var columnCount = spawner.RandomizeColumnCount ? spawner.MaxRandomColumnCount : spawner.PatternColumns;

			for (var row = 0; row < rowCount; row++) {
				EditorGUILayout.BeginHorizontal();

				for (var column = 0; column < columnCount; column++) {

					if (spawner.RandomizePattern) {
						var possiblyRandomField = (spawner.RandomizeColumnCount && column >= spawner.MinRandomColumnCount) || (spawner.RandomizeRowCount && row >= spawner.MinRandomRowCount);
						EditorGUI.BeginDisabledGroup(possiblyRandomField);
						EditorGUILayout.BeginHorizontal("Box");
						EditorGUILayout.LabelField("?", new GUIStyle { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(20));
						EditorGUILayout.EndHorizontal();
						EditorGUI.EndDisabledGroup();

					}
					else {
						EditorGUILayout.BeginHorizontal("Box");
						spawner.SpawnPattern[column][row] = EditorGUILayout.Toggle("", spawner.SpawnPattern[column][row], PatternToggleButtonOptions);
						EditorGUILayout.EndHorizontal();
					}

				}

				EditorGUILayout.EndHorizontal();
			}
			#endregion

			if (!spawner.RandomizePattern) {
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
			}

			#endregion
			EditorGUILayout.Space();
		}

	}

}