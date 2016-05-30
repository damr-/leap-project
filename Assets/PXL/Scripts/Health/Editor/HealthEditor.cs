using UnityEditor;
using UnityEngine;

namespace PXL.Health.Editor {

	/// <summary>
	/// Custom editor class which exposes the necessary variables to the Unity Editor and also provides buttons
	/// to quickly invoke important functions of the script
	/// </summary>
	[CustomEditor(typeof(Health))]
	public class HealthEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			var health = (Health)target;

			health.InitialHealth =
				Mathf.Clamp(
					EditorGUILayout.FloatField(new GUIContent("Initial Health", "The initial health of this object"),
						health.InitialHealth),
					1f, float.MaxValue);

			health.MaxHealth =
				Mathf.Clamp(
					EditorGUILayout.FloatField(new GUIContent("Max Health", "The maximum health this object can have"),
						health.MaxHealth),
					health.InitialHealth, float.MaxValue);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("Heal", "Fully heal the object")) && Utility.EditorUtility.IsPlaying())
				health.Heal();

			if (GUILayout.Button(new GUIContent("Hurt", "Apply 1 point of damage")) && Utility.EditorUtility.IsPlaying())
				health.ApplyDamage(1f);

			if (GUILayout.Button(new GUIContent("Kill", "Kill the object")) && Utility.EditorUtility.IsPlaying())
				health.Kill();

			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

	}

}