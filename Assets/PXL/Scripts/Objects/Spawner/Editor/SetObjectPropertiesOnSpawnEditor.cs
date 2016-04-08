using UnityEngine;
using UnityEditor;

namespace PXL.Objects.Spawner.Editor {

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

		private bool showComponentList;

		public override void OnInspectorGUI() {
			var c = (SetObjectPropertiesOnSpawn)target;

			Header("Force");
			c.ApplyForce = EditorGUILayout.BeginToggleGroup("Apply Force", c.ApplyForce);
			c.ForceMode =
				(ForceMode)EditorGUILayout.EnumPopup(new GUIContent("Force Mode", "The ForceMode which will be used"), c.ForceMode);
			c.Random = EditorGUILayout.ToggleLeft("Randomise Force", c.Random);
			if (c.Random) {
				c.MinForce = EditorGUILayout.Vector3Field(new GUIContent("Min Force", "The minimum possible force applied"),
					c.MinForce);
				c.MaxForce = EditorGUILayout.Vector3Field(new GUIContent("Max Force", "The maximum possible force applied"),
					c.MaxForce);
			}
			else {
				c.Force = EditorGUILayout.Vector3Field(new GUIContent("Force", "The force to apply to spawned objects"), c.Force);
			}
			EditorGUILayout.EndToggleGroup();

			Header("Mass");
			c.SetMass = EditorGUILayout.BeginToggleGroup("Set Mass", c.SetMass);
			EditorGUI.BeginDisabledGroup(c.SetScaleMass);
			c.Mass = Mathf.Clamp(
				EditorGUILayout.FloatField(new GUIContent("Mass", "The mass each object will have set"), c.Mass), 0.01f, 100f);
			EditorGUI.EndDisabledGroup();
			c.SetScaleMass =
				EditorGUILayout.Toggle(new GUIContent("Set Scale Mass", "Set the mass according to the object's scale"),
					c.SetScaleMass);

			EditorGUILayout.EndToggleGroup();

			Header("Material");
			c.SetMaterial = EditorGUILayout.BeginToggleGroup("Set Material", c.SetMaterial);
			c.Material = (Material)EditorGUILayout.ObjectField(
				new GUIContent("Material", "The Material which will be applied to spawned objects"),
				c.Material,
				typeof(Material),
				false);
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();

			Header("Physic Material");
			c.SetPhysicMaterial = EditorGUILayout.BeginToggleGroup("Set Physic Material", c.SetPhysicMaterial);
			c.PhysicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(
				new GUIContent("Physic Material", "The Physic Material which will be applied to spawned objects"),
				c.PhysicMaterial,
				typeof(PhysicMaterial),
				false);
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();

			Header("Health");
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


			Header("Components");
			c.IsAddingComponents = EditorGUILayout.BeginToggleGroup("Add Components", c.IsAddingComponents);

			showComponentList = EditorGUILayout.Foldout(showComponentList, "Components");
			if (showComponentList) {
				EditorGUILayout.BeginVertical();
				for (var i = 0; i < c.Components.Count; i++) {
					EditorGUILayout.BeginHorizontal();
					c.Components[i] = EditorGUILayout.TextField("", c.Components[i], GUILayout.MaxWidth(150));
					if (GUILayout.Button(new GUIContent("X", "Removes this item from the list"), GUILayout.MaxWidth(20),
						GUILayout.MaxHeight(20))) {
						c.Components.RemoveAt(i);
					}
					EditorGUILayout.EndHorizontal();
				}

				if (c.Components.Count > 5 &&
					GUILayout.Button(new GUIContent("Clear", "Removes all items from the list"), GUILayout.MaxWidth(75),
						GUILayout.MaxHeight(20))) {
					c.Components.Clear();
				}

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginHorizontal();
			c.NewComponentName = EditorGUILayout.TextField("Component Namespace:", c.NewComponentName);
			if (GUILayout.Button(new GUIContent("Add", "Adds the given component namespace to the list"))) {
				c.AddNewComponentToList();
				c.NewComponentName = "";
				showComponentList = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
		}

		private void Header(string text) {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUIStyle("Box"));
			EditorGUILayout.LabelField(text, headerStyle);
			GUILayout.EndHorizontal();
		}

	}
}