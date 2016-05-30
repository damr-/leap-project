using UnityEngine;
using UnityEditor;

namespace PXL.Interaction.Editor {

	/// <summary>
	/// Custom editor script which exposes the necessary variables to the Inspector and 
	/// provides an easy to use interface.
	/// </summary>
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
						new GUIContent("Offset Percent",
							"How many percent of the initial distance to the grabbed object which will be used while holding/moving it"),
						moveable.OffsetPercent), 0f, 1f);

			EditorGUILayout.LabelField(
				new GUIContent("Freeze Movement",
					"Define on which axes this object should not be able to move or rotate."),
				new GUIStyle(EditorStyles.boldLabel));

			UpdateFreezeOptions("Freeze Position", ref moveable.FreezePosition);
			UpdateFreezeOptions("Freeze Rotation", ref moveable.FreezeRotation);

			EditorGUILayout.LabelField(
				new GUIContent("Overwrite State Values",
					"Set values which will be used for the freeze state instead of the last known values."),
				new GUIStyle(EditorStyles.boldLabel));

			UpdateOverwriteValues("Position", moveable.FreezePosition, ref moveable.OverwritePosition, ref moveable.OverwritePositionValues);
			UpdateOverwriteValues("Rotation", moveable.FreezeRotation, ref moveable.OverwriteRotation, ref moveable.OverwriteRotationValues);

		}

		/// <summary>
		/// Display the options to freeze certain axes
		/// </summary>
		/// <param name="title">The text of the label</param>
		/// <param name="freezeVector">The vector which describes the freeze state of the object internally</param>
		private void UpdateFreezeOptions(string title, ref Vector3 freezeVector) {
			var freezeState = new bool[3];

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(title, GUILayout.MaxWidth(100));

			for (var i = 0; i < 3; i++) {
				freezeState[i] = EditorGUILayout.Toggle("", freezeVector[i] > 0, GUILayout.MaxWidth(10));
				EditorGUILayout.LabelField(axes[i], GUILayout.MaxWidth(15));
			}

			EditorGUILayout.EndHorizontal();

			for (var i = 0; i < 3; i++)
				freezeVector[i] = freezeState[i] ? 1 : 0;
		}

		/// <summary>
		/// Adds toggles to overwrite the default values for the frozen state
		/// </summary>
		/// <param name="title">The content of the overwrite title label</param>
		/// <param name="freezeVector">The Vector describing which axes are frozen</param>
		/// <param name="overwriteBool">The bool array containing information about which axis will be overwritten</param>
		/// <param name="overwriteVector">The Vector containing the values for overwriting</param>
		private void UpdateOverwriteValues(string title, Vector3 freezeVector, ref bool[] overwriteBool, ref Vector3 overwriteVector) {
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(title, GUILayout.MaxWidth(100));

			for (var i = 0; i < 3; i++) {
				EditorGUI.BeginDisabledGroup(freezeVector[i] <= 0);

				if (freezeVector[i] <= 0)
					overwriteBool[i] = false;

				overwriteBool[i] = EditorGUILayout.Toggle("", overwriteBool[i], GUILayout.MaxWidth(10));

				EditorGUILayout.LabelField(axes[i], GUILayout.MaxWidth(15));

				EditorGUI.BeginDisabledGroup(!overwriteBool[i]);
				overwriteVector[i] = EditorGUILayout.FloatField(overwriteVector[i], GUILayout.MaxWidth(50));
				EditorGUI.EndDisabledGroup();
				GUILayout.FlexibleSpace();

				EditorGUI.EndDisabledGroup();
			}

			EditorGUILayout.EndHorizontal();
		}

	}

}