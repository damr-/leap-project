using UnityEngine;
using UniRx;
using PXL.Objects.Areas;
using PXL.Utility;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(ObjectSpawner))]
	public class StopSpawnOnGoalReached : MonoBehaviour {

		/// <summary>
		/// The <see cref="DestroyArea"/> to subscribe to
		/// </summary>
		public DestroyArea DestroyArea;

		/// <summary>
		/// The <see cref="ObjectSpawner"/> of this <see cref="GameObject"/>
		/// </summary>
		protected ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}
		private ObjectSpawner mObjectSpawner;

		private void Start() {
			DestroyArea.GoalReached.Subscribe(_ => HandleGoalReached());
		}

		/// <summary>
		/// Called when the goal of the <see cref="DestroyArea"/> is reached
		/// </summary>
		private void HandleGoalReached() {
			ObjectSpawner.IsSpawningEnabled = false;
		}

	}

}