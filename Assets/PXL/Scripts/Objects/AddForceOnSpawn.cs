using UniRx;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	[RequireComponent(typeof(ObjectSpawner))]
	public class AddForceOnSpawn : MonoBehaviour {

		/// <summary>
		/// The amount of force to apply
		/// </summary>
		public Vector3 Force = Vector3.forward;

		/// <summary>
		/// The ForceMode to apply to the spawned object
		/// </summary>
		public ForceMode ForceMode = ForceMode.Impulse;

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
		}
    }

}