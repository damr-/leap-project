using System;
using PXL.Utility;
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
		/// Adds the check for <see cref="delayedSpawn"/>
		/// </summary>
		protected override bool CanRespawnImmediately() {
			return !delayedSpawn && base.CanRespawnImmediately();
		}

		/// <summary>
		/// Adds a delay before spawning a new object
		/// </summary>
		public override void RemoveAllObjects() {
			base.RemoveAllObjects();
			delayedSpawn = true;

			Observable.Timer(TimeSpan.FromSeconds(SpawnDelay)).Subscribe(_ => {
				delayedSpawn = false;
				SpawnObject();
			});
		}

	}

}