using UnityEditor;
using UnityEngine;

namespace PXL.Health.Editor {

	[CustomEditor(typeof(Health))]
	public class HealthEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			var health = (Health)target;


			health.InitialHealth = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Initial Health", "The initial health of this object"), health.InitialHealth),
											1f, float.MaxValue);

			health.MaxHealth = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Max Health", "The maximum health this object can have"), health.MaxHealth),
											health.InitialHealth, float.MaxValue);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("Heal", "Fully heal the object")) && Utility.EditorUtility.IsPlaying()) {
				health.Heal();
			}

			if (GUILayout.Button(new GUIContent("Hurt", "Apply 1 point of damage to this object's health")) && Utility.EditorUtility.IsPlaying()) {
				health.ApplyDamage(1f);
			}

			if (GUILayout.Button(new GUIContent("Kill", "Kill the object")) && Utility.EditorUtility.IsPlaying()) {
				health.Kill();
			}

			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

	}

}