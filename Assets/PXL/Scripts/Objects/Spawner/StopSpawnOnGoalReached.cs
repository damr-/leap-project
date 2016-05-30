using UnityEngine;
using UniRx;
using PXL.Objects.Areas;
using PXL.Utility;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// This script disables the spawning of an <see cref="ObjectSpawner"/> as soon as the goal of a <see cref="DestroyArea"/> is reached.
	/// </summary>
	[RequireComponent(typeof(ObjectSpawner))]
	public class StopSpawnOnGoalReached : MonoBehaviour {

		/// <summary>
		/// The observed <see cref="DestroyArea"/>
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