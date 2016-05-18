using UnityEngine;
using UnityEditor;

namespace PXL.Objects.Spawner.Editor {

	[CustomEditor(typeof(SetObjectPropertiesOnSpawn))]
	public class SetObjectPropertiesOnSpawnEditor : UnityEditor.Editor {
	
		public override void OnInspectorGUI() {
			var c = (SetObjectPropertiesOnSpawn)target;
			
			Utility.EditorUtility.Header("Health");
			c.SetHealth = EditorGUILayout.BeginToggleGroup("Set Health Properties", c.SetHealth);

			c.InitialHealth =
				Mathf.Clamp(
					EditorGUILayout.FloatField(new GUIContent("Initial Health", "The initial health of this object"), c.InitialHealth),
					1f, float.MaxValue);

			c.MaxHealth =
				Mathf.Clamp(
					EditorGUILayout.FloatField(new GUIContent("Max Health", "The maximumg health this object can have"), c.MaxHealth),
					c.InitialHealth, float.MaxValue);

			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
		}

	}
}