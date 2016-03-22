using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;


namespace PXL.Objects.Spawner {

	[RequireComponent(typeof (ObjectSpawner))]
	public class LoseGameOnTotalSpawnLimit : MonoBehaviour {

		private ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}

		private ObjectSpawner mObjectSpawner;

		private void Start() {
			ObjectSpawner.TotalDespawnCount.Subscribe(count => {
				if (count == ObjectSpawner.TotalSpawnLimit) {
					GameMode.SetGameWon(false);
				}
			});
		}

	}

}