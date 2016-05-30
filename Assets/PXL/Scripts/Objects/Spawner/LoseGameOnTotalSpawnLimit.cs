using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// This script makes the user lose the game as soon as a spawner has reached it's <see cref="ObjectSpawner.TotalSpawnLimit"/>.
	/// </summary>
	[RequireComponent(typeof (ObjectSpawner))]
	public class LoseGameOnTotalSpawnLimit : MonoBehaviour {

		/// <summary>
		/// The <see cref="ObjectSpawner"/> component of this object
		/// </summary>
		private ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}
		private ObjectSpawner mObjectSpawner;

		private void Start() {
			ObjectSpawner.TotalDespawnCount.Subscribe(count => {
				if (count == ObjectSpawner.TotalSpawnLimit)
					GameState.SetGameWon(false);
			});
		}

	}

}