using UnityEngine;
using UnityEditor;

namespace PXL.Objects.Editor {

	[CustomEditor(typeof(SetObjectPropertiesOnSpawn))]
	public class SetObjectPropertiesOnSpawnEditor : UnityEditor.Editor {

		/// <summary>
		/// Style for header labels
		/// </summary>
		private readonly GUIStyle headerStyle = new GUIStyle() {
			fontSize = 14,
			fontStyle = FontStyle.Bold,
			alignment = TextAnchor.MiddleLeft
		};

		public override void OnInspectorGUI() {
			var c = (SetObjectPropertiesOnSpawn)target;

			Header("Force");
			c.ApplyForce = EditorGUILayout.BeginToggleGroup("Apply Force", c.ApplyForce);
			c.ForceMode = (ForceMode)EditorGUILayout.EnumPopup(new GUIContent("Force Mode", "The ForceMode which will be used"), c.ForceMode);
			c.Random = EditorGUILayout.ToggleLeft("Randomise Force", c.Random);
			if (c.Random) {
				c.MinForce = EditorGUILayout.Vector3Field(new GUIContent("Min Force", "The minimum possible force applied"), c.MinForce);
				c.MaxForce = EditorGUILayout.Vector3Field(new GUIContent("Max Force", "The maximum possible force applied"), c.MaxForce);
			}
			else {
				c.Force = EditorGUILayout.Vector3Field(new GUIContent("Force", "The force to apply to spawned objects"), c.Force);
			}
			EditorGUILayout.EndToggleGroup();

			Header("Physic Material");
			c.SetPhysicMaterial = EditorGUILayout.BeginToggleGroup("Set Physic Material", c.SetPhysicMaterial);
			c.PhysicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(
				new GUIContent("Physic Material", "The Physic Matierl which will be applied to spawned objects"),
				c.PhysicMaterial,
				typeof(PhysicMaterial),
				false);
			EditorGUILayout.EndToggleGroup();
		}

		private void Header(string text) {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUIStyle("Box"));
			EditorGUILayout.LabelField(text, headerStyle);
			GUILayout.EndHorizontal();
		}

	}
}
