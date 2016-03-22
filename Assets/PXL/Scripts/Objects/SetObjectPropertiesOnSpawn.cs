using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectPropertiesOnSpawn : MonoBehaviour {
	
		/// <summary>
		/// The amount of force to apply.
		/// </summary>
		public Vector3 Force = Vector3.zero;

		/// <summary>
		/// Whether to apply a force
		/// </summary>
		public bool ApplyForce;
		
		/// <summary>
		/// If the force is always the same or should be chosen randomly between <see cref="MinForce"/> and <see cref="MaxForce"/>
		/// </summary>
		public bool Random;

		/// <summary>
		/// The minimum possible force when choosing a random one
		/// </summary>
		public Vector3 MinForce = Vector3.zero;

		/// <summary>
		/// The maximum possible force when choosing a random one
		/// </summary>
		public Vector3 MaxForce = Vector3.zero;

		/// <summary>
		/// The ForceMode to apply to the spawned object
		/// </summary>
		public ForceMode ForceMode = ForceMode.Force;
		
		/// <summary>
		/// Whether the mass should be set
		/// </summary>
		public bool SetMass;

		/// <summary>
		/// The set mass
		/// </summary>
		public float Mass;

		/// <summary>
		/// Whether the mass should be the same as the object's scale
		/// </summary>
		public bool SetScaleMass;

		/// <summary>
		/// Whether to change the <see cref="PhysicMaterial"/>
		/// </summary>
		public bool SetPhysicMaterial;

		/// <summary>
		/// The <see cref="PhysicMaterial"/> to set
		/// </summary>
		public PhysicMaterial PhysicMaterial;

		/// <summary>
		/// Whether to change the <see cref="Material"/>
		/// </summary>
		public bool SetMaterial;

		/// <summary>
		/// The <see cref="PhysicMaterial"/> to set
		/// </summary>
		public Material Material;

		/// <summary>
		/// The components which will be added to the object on spawn
		/// </summary>
		public List<string> Components = new List<string>();

		private string assemblyName;

		/// <summary>
		/// Whether the health properties will be changed
		/// </summary>
		public bool SetHealth;

		/// <summary>
		/// The initial health of the object
		/// </summary>
		public float InitialHealth = 1f;

		/// <summary>
		/// The maximum health of the object
		/// </summary>
		public float MaxHealth = 1f;

		/// <summary>
		/// The ObjectSpawner of this object
		/// </summary>
		private ObjectSpawner ObjectSpawner {
			get {
				return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>());
			}
		}
		private ObjectSpawner mObjectSpawner;

		public bool IsAddingComponents;

		public string newComponentName;

		private void Start() {
			assemblyName = Assembly.GetExecutingAssembly().FullName;
			ObjectSpawner.ObjectSpawned.Subscribe(HandleObjectSpawned);
		}
		
		/// <summary>
		/// Called when the referenced spawned spawned an object
		/// </summary>
		/// <param name="interactiveObject"></param>
		private void HandleObjectSpawned(InteractiveObject interactiveObject) {
			var rigidbodyComponent = interactiveObject.GetComponent<Rigidbody>();

			if (rigidbodyComponent != null) {
				if(ApplyForce)
					AddObjectForce(rigidbodyComponent);
				if(SetMass)
					rigidbodyComponent.mass = SetScaleMass ? interactiveObject.Scale : Mass;
			}

			var collider = interactiveObject.GetComponents<Collider>().First(c => !c.isTrigger);

			if (collider != null && PhysicMaterial != null && SetPhysicMaterial)
				collider.material = PhysicMaterial;

			var mesh = interactiveObject.GetComponent<MeshRenderer>();

			if (SetMaterial && mesh != null && Material != null) {
				mesh.material = Material;
			}

			Components.ForEach(c => CreateAndAddComponent(interactiveObject.gameObject, c));

			var health = interactiveObject.GetComponent<Health.Health>();
			if (health != null && SetHealth) {
				health.MaxHealth = MaxHealth;
				health.InitialHealth = InitialHealth;
				health.CurrentHealth.Value = InitialHealth;
			}
		}

		/// <summary>
		/// Adds force to the given <see cref="Rigidbody"/>
		/// </summary>
		private void AddObjectForce(Rigidbody rigidbodyComponent) {
			var force = Force;
			if (Random) {
				var x = UnityEngine.Random.Range(MinForce.x, MaxForce.x);
				var y = UnityEngine.Random.Range(MinForce.y, MaxForce.y);
				var z = UnityEngine.Random.Range(MinForce.z, MaxForce.z);
				force = new Vector3(x, y, z);
			}
			rigidbodyComponent.AddForce(force, ForceMode);
		}

		/// <summary>
		/// Creates a component from the given string and adds it to the given target
		/// </summary>
		private void CreateAndAddComponent(GameObject target, string component) {
			var componentType = Types.GetType(component, assemblyName);
			target.AddComponent(componentType);
		}

		/// <summary>
		/// Adds the content of <see cref="newComponentName"/> to <see cref="Components"/> if it's not already in the list and valid
		/// </summary>
		public void AddNewComponentToList() {
			var componentName = newComponentName.Trim();
            if (componentName.Length == 0 || Components.Contains(componentName))
				return;

			Components.Add(newComponentName);
		}
	}

}