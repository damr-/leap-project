using PXL.Utility.Time;
using UniRx;

namespace PXL.Objects.Spawner {

	public class ManualObjectSpawner : ObjectSpawner {

		/// <summary>
		/// The length of the spawn delay when removing all objects
		/// </summary>
		private const float SpawnDelay = 0.5f;

		/// <summary>
		/// Whether the spawning is currently delayed
		/// </summary>
		private bool delayedSpawn;

		/// <summary>
		/// When the delay started
		/// </summary>
		private float delayStartTime;

		/// <summary>
		/// Adds the check for <see cref="delayedSpawn"/>
		/// </summary>
		protected override bool CanRespawnImmediately() {
			return !delayedSpawn && base.CanRespawnImmediately();
		}

		protected override void Update() {
			base.Update();

			if (!delayedSpawn || !(UnityEngine.Time.time - delayStartTime > SpawnDelay))
				return;

			delayedSpawn = false;
			SpawnObject();
		}

		/// <summary>
		/// Adds a delay before spawning a new object
		/// </summary>
		public override void RemoveAllObjects() {
			base.RemoveAllObjects();
			delayedSpawn = true;
			delayStartTime = UnityEngine.Time.time;
		}

	}

}