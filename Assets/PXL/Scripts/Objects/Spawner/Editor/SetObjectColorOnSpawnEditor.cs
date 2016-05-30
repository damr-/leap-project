using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(SetObjectColorOnSpawn))]
	public class SetObjectColorOnSpawnEditor : UnityEditor.Editor {

		/// <summary>
		/// Whether the available colors are currently being shown
		/// </summary>
		private bool showAvailableColors;

		/// <summary>
		/// The references <see cref="SetObjectColorOnSpawn"/> component
		/// </summary>
		private SetObjectColorOnSpawn t;

		public override void OnInspectorGUI() {
			t = (SetObjectColorOnSpawn)target;

			if (t.DefaultColor.Name.Trim() == "" && t.AvailableColors.Count > 0)
				t.DefaultColor = t.AvailableColors[0];

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Default Color: ", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ColorField(new GUIContent("", ""), t.DefaultColor.Color, false, false, false, null, GUILayout.MaxWidth(50));
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.LabelField("", t.DefaultColor.Name, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();

			showAvailableColors = EditorGUILayout.Foldout(showAvailableColors, "Available Colors");

			if (showAvailableColors)
				DrawAvailableColors();

			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("New Color: ", EditorStyles.boldLabel, GUILayout.MaxWidth(75));
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Color: ", GUILayout.MaxWidth(50));
			t.NewColor.Color = EditorGUILayout.ColorField("", t.NewColor.Color, GUILayout.MaxWidth(75));
			EditorGUILayout.LabelField("Name: ", GUILayout.MaxWidth(50));
			t.NewColor.Name = EditorGUILayout.TextField("", t.NewColor.Name, GUILayout.MaxWidth(75));

			if (GUILayout.Button(new GUIContent("Add", "Adds the given color to the list"), GUILayout.MaxWidth(100)))
				t.AddColor();

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

		}

		private void DrawAvailableColors() {
			EditorGUILayout.BeginVertical();
			for (var i = 0; i < t.AvailableColors.Count; i++) {
				EditorGUILayout.BeginHorizontal();

				var c = t.AvailableColors[i];
				c.Color = EditorGUILayout.ColorField("", t.AvailableColors[i].Color, GUILayout.MaxWidth(50));
				c.Name = EditorGUILayout.TextField("", t.AvailableColors[i].Name, GUILayout.MaxWidth(150));
				t.AvailableColors[i] = c;

				EditorGUI.BeginDisabledGroup(t.DefaultColor.Name == t.AvailableColors[i].Name);
				if (GUILayout.Button(new GUIContent("✔", "Make this the default color"), GUILayout.MaxWidth(20),
					GUILayout.MaxHeight(20))) {
					t.DefaultColor = t.AvailableColors[i];
				}
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button(new GUIContent("X", "Removes this item from the list"), GUILayout.MaxWidth(20),
					GUILayout.MaxHeight(20))) {
					t.AvailableColors.RemoveAt(i);
				}
				EditorGUILayout.EndHorizontal();
			}

			if (t.AvailableColors.Count > 5 &&
			    GUILayout.Button(new GUIContent("Clear", "Remove all items"), GUILayout.MaxWidth(75),
				    GUILayout.MaxHeight(20))) {
				t.AvailableColors.Clear();
			}

			EditorGUILayout.EndVertical();
		}

	}

}