using UnityEngine;
using UnityEditor;

namespace PXL.Utility.Editor {

	[CustomEditor(typeof(MaintainTransformation))]
	public class MaintainTransformationEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			var m = (MaintainTransformation)target;

			CreateTransformSection("Position", ref m.MaintainPosition, ref m.DefinedPosition, ref m.MaintainedPosition);
			CreateTransformSection("Rotation", ref m.MaintainRotation, ref m.DefinedRotation, ref m.MaintainedRotation);
			CreateTransformSection("Scale", ref m.MaintainScale, ref m.DefinedScale, ref m.MaintainedScale);
		}

		private void CreateTransformSection(string transformType, ref bool maintain, ref bool define, ref Vector3 definedValue) {
			maintain = EditorGUILayout.BeginToggleGroup("Maintain " + transformType, maintain);
			EditorGUILayout.BeginHorizontal();
			define =
				EditorGUILayout.BeginToggleGroup(
					new GUIContent("Define " + transformType, "Ignore the object's starting " + transformType + " and use the given vector"),
					define);
			definedValue = EditorGUILayout.Vector3Field("", definedValue, GUILayout.MaxWidth(150));
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
		}

	}

}