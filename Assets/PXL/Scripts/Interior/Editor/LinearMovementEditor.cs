using UnityEditor;
using UnityEngine;

namespace PXL.Interior.Editor {

	/// <summary>
	/// Custom editor script which exposes the necessary variables to the Inspector and 
	/// provides an easy to use interface.
	/// </summary>
	[CustomEditor(typeof(LinearMovement))]
	public class LinearMovementEditor : UnityEditor.Editor {

		private LinearMovement s;

		public override void OnInspectorGUI() {
			s = (LinearMovement)target;

			Utility.EditorUtility.FloatField(new GUIContent("Speed", ""), ref s.Speed, 0.001f, 10f, 50);

			EditorGUILayout.BeginHorizontal();
			s.LocalSpace = EditorGUILayout.Toggle("", s.LocalSpace, GUILayout.MaxWidth(15), GUILayout.MaxHeight(15));
			EditorGUILayout.LabelField("Use local space", GUILayout.MaxWidth(100));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("start position", GUILayout.MaxWidth(100));
			s.StartPosition = EditorGUILayout.Vector3Field("", s.StartPosition, GUILayout.MaxWidth(200));

			if (GUILayout.Button("set", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20)) && !Utility.EditorUtility.IsPlaying())
				s.StartPosition = s.Pos;

			if (GUILayout.Button("go there", GUILayout.MaxWidth(60), GUILayout.MaxHeight(20)) &&
			    !Utility.EditorUtility.IsPlaying())
				s.Pos = s.StartPosition;

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if (s.MovementAxes.Length != 3)
				s.MovementAxes = new bool[3];

			DrawAxis("X", ref s.MovementAxes[0], ref s.MinX, ref s.MaxX);
			DrawAxis("Y", ref s.MovementAxes[1], ref s.MinY, ref s.MaxY);
			DrawAxis("Z", ref s.MovementAxes[2], ref s.MinZ, ref s.MaxZ);
		}

		private void DrawAxis(string axisName, ref bool isMoving, ref float axisMin, ref float axisMax) {
			EditorGUILayout.BeginHorizontal();
			isMoving = EditorGUILayout.Toggle(isMoving, GUILayout.MaxWidth(15), GUILayout.MaxHeight(15));
			EditorGUILayout.LabelField("Move on " + axisName, GUILayout.MaxWidth(75));
			EditorGUILayout.EndHorizontal();

			EditorGUI.BeginDisabledGroup(!isMoving);
			DrawBoundary("Min " + axisName, ref axisMin, float.MinValue, axisMax, "min" + axisName.ToLower());
			DrawBoundary("Max " + axisName, ref axisMax, axisMin, float.MaxValue, "max" + axisName.ToLower());
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space();
		}

		private void DrawBoundary(string title, ref float var, float min, float max, string bounaryName) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(title, GUILayout.MaxWidth(50));
			var =
				Mathf.Clamp(
					EditorGUILayout.FloatField(new GUIContent("", ""), var, GUILayout.MaxWidth(60)),
					min, max);

			if (GUILayout.Button("set", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20)) && !Utility.EditorUtility.IsPlaying())
				s.UpdateBoundary(bounaryName);

			if (GUILayout.Button("clear", GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)) && !Utility.EditorUtility.IsPlaying())
				var = 0f;

			EditorGUILayout.EndHorizontal();
		}

	}

}