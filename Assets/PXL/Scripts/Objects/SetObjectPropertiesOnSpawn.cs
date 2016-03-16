using System.Linq;
using PXL.Interaction;
using UniRx;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectPropertiesOnSpawn : MonoBehaviour {

		#region Force
		/// <summary>
		/// The amount of force to apply.
		/// </summary>
		[Header("Object Force")]
		public Vector3 Force = Vector3.zero;

		/// <summary>
		/// The ForceMode to apply to the spawned object
		/// </summary>
		public ForceMode ForceMode = ForceMode.Impulse;
		#endregion
		
		/// <summary>
		/// The <see cref="PhysicMaterial"/> to set
		/// </summary>
		[Header("Physic Material")]
		public PhysicMaterial PhysicMaterial;

		/// <summary>
		/// The constraints for the object's <see cref="Moveable"/> component
		/// </summary>
		[Header("Constraints")]
		public RigidbodyConstraints Constraints;

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
			ObjectSpawner.ObjectSpawned.Subscribe(HandleObjectSpawned);
		}

		/// <summary>
		/// Called when the referenced spawned spawned an object
		/// </summary>
		/// <param name="objectBehaviour"></param>
		private void HandleObjectSpawned(ObjectBehaviour objectBehaviour) {
			var rigidbodyComponent = objectBehaviour.GetComponent<Rigidbody>();

			if (rigidbodyComponent == null)
				return;

			rigidbodyComponent.AddForce(Force, ForceMode);

			var collider = objectBehaviour.GetComponents<Collider>().First(c => !c.isTrigger);

			if (collider == null || PhysicMaterial == null)
				return;

			collider.material = PhysicMaterial;

			var moveable = objectBehaviour.GetComponent<Moveable>();

			if (moveable != null) {
				moveable.Constraints = Constraints;
			}

		}
    }

}