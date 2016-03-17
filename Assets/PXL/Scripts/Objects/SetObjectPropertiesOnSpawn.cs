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
		/// Whether to change the <see cref="PhysicMaterial"/>
		/// </summary>
		public bool SetPhysicMaterial;

		/// <summary>
		/// The <see cref="PhysicMaterial"/> to set
		/// </summary>
		public PhysicMaterial PhysicMaterial;

		/// <summary>
		/// The components which will be added to the object on spawn
		/// </summary>
		public List<string> Components = new List<string>();

		private string assemblyName;

		/// <summary>
		/// The ObjectSpawner of this object
		/// </summary>
		private ObjectSpawner ObjectSpawner {
			get {
				return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>());
			}
		}
		private ObjectSpawner mObjectSpawner;
		
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

			if (rigidbodyComponent != null && ApplyForce)
				AddObjectForce(rigidbodyComponent);

			var collider = interactiveObject.GetComponents<Collider>().First(c => !c.isTrigger);

			if (collider != null && PhysicMaterial != null && SetPhysicMaterial)
				collider.material = PhysicMaterial;

			AddComponents(interactiveObject.gameObject);
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
		/// Calls <see cref="CreateAndAddComponent"/> for every component in <see cref="Components"/>
		/// </summary>
		private void AddComponents(GameObject target) {
			Components.ForEach(c => CreateAndAddComponent(target, c));
		}

		/// <summary>
		/// Adds the given component to the given target
		/// </summary>
		private void CreateAndAddComponent(GameObject target, string component) {
			var componentType = Types.GetType(component, assemblyName);
			target.AddComponent(componentType);
		}

	}

}