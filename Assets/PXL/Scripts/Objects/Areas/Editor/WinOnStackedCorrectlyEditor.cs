using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Areas.Editor {

	[CustomEditor(typeof(WinOnStackedCorrectly))]
	public class WinOnStackedCorrectlyEditor : UnityEditor.Editor {


		public override void OnInspectorGUI() {
			var w = (WinOnStackedCorrectly)target;

			w.WinImmediately = EditorGUILayout.ToggleLeft("Win Immediately", w.WinImmediately);

			EditorGUI.BeginDisabledGroup(w.WinImmediately);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Win Points",
				"The amount of points that are rewarded when the objects in this area are stacked correctly."), GUILayout.MaxWidth(75));
			w.WinPoints = EditorGUILayout.IntField("", w.WinPoints, GUILayout.MaxWidth(50));
			EditorGUILayout.EndHorizontal();

			EditorGUI.EndDisabledGroup();
		}

	}

}