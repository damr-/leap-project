using System;
using PXL.Objects.Spawner;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Areas {

	public class SpawnOnTriggerExitArea : ObjectArea {

		/// <summary>
		/// The target objectspawner which will respawn an object
		/// </summary>
		public ObjectSpawner ObjectSpawner;

		/// <summary>
		/// How many objects can be spawned per second maximum
		/// </summary>
		public float RespawnDelay = 1f;

		/// <summary>
		/// The disposable for the delay time subscription
		/// </summary>
		private IDisposable disposable = Disposable.Empty;

		/// <summary>
		/// Whether this area is currently reacting to trigger exit events
		/// </summary>
		private bool isReacting = true;

		private void Start() {
			ObjectSpawner.AssertNotNull("Missing spawner reference!");
		}

		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit(other);
			if (!isReacting || !other.gameObject.CompareTag(TargetTag))
				return;

			isReacting = false;
			disposable = Observable.Timer(TimeSpan.FromSeconds(RespawnDelay)).Subscribe(_ => {
				ObjectSpawner.SpawnObject();
				isReacting = true;
			});
		}

		private void OnDisable() {
			disposable.Dispose();
		}

		protected override void HandleValidObjectType(InteractiveObject interactiveObject) { }

		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) { }

	}

}