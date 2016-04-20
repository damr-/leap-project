using UnityEditor;
using UnityEngine;

namespace PXL.UI.World.Hand.Editor {

	[CustomEditor(typeof(TrackRotation))]
	public class TrackRotationEditor : UnityEditor.Editor {

		private TrackRotation t;

		public override void OnInspectorGUI() {
			t = (TrackRotation) target;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Tracking Technique", GUILayout.MaxWidth(125));
			t.Technique = (TrackRotation.TrackingTechnique) EditorGUILayout.EnumPopup("", t.Technique, GUILayout.MaxWidth(175));
			EditorGUILayout.EndHorizontal();


			switch (t.Technique) {
				case TrackRotation.TrackingTechnique.TargetTransform:
					DrawTargetTransformTechnique();
					break;
				case TrackRotation.TrackingTechnique.CustomPosition:
					DrawCustomPositionTechnique();
					break;
			}
		}

		private void DrawTargetTransformTechnique() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Origin transform", GUILayout.MaxWidth(100));
			t.TargetTransform = (Transform)EditorGUILayout.ObjectField("", t.TargetTransform, typeof(Transform), true, GUILayout.MaxWidth(150));
			EditorGUILayout.EndHorizontal();
		}

		private void DrawCustomPositionTechnique() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Origin", "The position this object will keep rotating towards"), GUILayout.MaxWidth(50));
			t.CustomPosition = EditorGUILayout.Vector3Field("", t.CustomPosition, GUILayout.MaxWidth(200));
			EditorGUILayout.EndHorizontal();
		}

	}

}