using UnityEngine;
using UnityEditor;

namespace PXL.Interaction.Editor {

	[CustomEditor(typeof(Moveable))]
	public class MoveableEditor : UnityEditor.Editor {

		/// <summary>
		/// The names of the axes
		/// </summary>
		private readonly string[] axes = { "X", "Y", "Z" };

		public override void OnInspectorGUI() {
			var moveable = (Moveable)target;

			moveable.OffsetPercent =
				Mathf.Clamp(
					EditorGUILayout.FloatField(
						new GUIContent("Offset Percent", "How many percent of the initial distance to the grabbed object which will be used while holding/moving it"),
						moveable.OffsetPercent), 0f, 1f);

			moveable.FreezePosition = UpdateFreezeOptions("Freeze Position", moveable.FreezePosition);
			moveable.FreezeRotation = UpdateFreezeOptions("Freeze Rotation", moveable.FreezeRotation);
		}

		private Vector3 UpdateFreezeOptions(string title, Vector3 freezeVector) {
			var freezeState = new bool[3];

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(title, GUILayout.MaxWidth(100));

			for (var i = 0; i < 3; i++) {
				freezeState[i] = EditorGUILayout.Toggle("", freezeVector[i] > 0, GUILayout.MaxWidth(10));
				EditorGUILayout.LabelField(axes[i], GUILayout.MaxWidth(15));
			}

			EditorGUILayout.EndHorizontal();

			return new Vector3(freezeState[0] ? 1 : 0,
								freezeState[1] ? 1 : 0,
								freezeState[2] ? 1 : 0);
		}

	}

}