using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// This script sets the <see cref="ObjectSpawner.SpawnedObjectsContainer"/> of this object's <see cref="ObjectSpawner"/>
	/// at start.
	/// 
	/// The name of the Container has to be exactly like in the Hierarchy and the container has to be at root level.
	/// </summary>
	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectsContainer : MonoBehaviour {

		/// <summary>
		/// The name of the container GameObject
		/// </summary>
		public string ContainerName;

		private ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}
		private ObjectSpawner mObjectSpawner;

		private void Start() {
			if (ContainerName.Trim() == "")
				throw new MissingReferenceException("Missing container name!");
			ObjectSpawner.SpawnedObjectsContainer = GameObject.Find(ContainerName).transform;
		}

	}

}