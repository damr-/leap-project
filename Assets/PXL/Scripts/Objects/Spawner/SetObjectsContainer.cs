using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectsContainer : MonoBehaviour {

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