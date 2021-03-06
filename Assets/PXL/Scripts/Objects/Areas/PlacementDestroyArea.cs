﻿using System.Collections.Generic;
using UnityEngine;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;

namespace PXL.Objects.Areas {

	/// <summary>
	/// This area destroys valid objects, but only as soon as they are stationary.
	/// </summary>
	public class PlacementDestroyArea : DestroyArea {

		protected List<InteractiveObject> ValidObjects = new List<InteractiveObject>();

		protected override void Update() {
			base.Update();

			if (GameState.GameOver || !AreaActive)
				return;

			Extensions.PurgeIfNecessary(ref ValidObjects);

			foreach (var o in ValidObjects) {
				var g = o.GetComponent<Grabbable>();

				if (g != null) {
					if (!g.IsStationary())
						continue;
				}
				else {
					var r = o.GetComponent<Rigidbody>();
					if (r != null && !r.velocity.Equal(Vector3.zero))
						continue;
				}

				CurrentDestroyAmount.Value++;
				ObjectDestroyedSubject.OnNext(o);

				if (CurrentDestroyAmount.Value == WinDestroyAmount) {
					HandleGameWon();
					o.Kill();
					return;
				}
				o.Kill();
			}
		}

		/// <summary>
		/// Adds the new object to <see cref="ValidObjects"/>
		/// </summary>
		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
			base.HandleValidObjectType(interactiveObject);

			if (!ValidObjects.Contains(interactiveObject))
				ValidObjects.Add(interactiveObject);
		}

		/// <summary>
		/// Removes the object from <see cref="ValidObjects"/>
		/// </summary>
		protected override void OnTriggerExit(Collider other) {
			var interactiveObject = other.GetComponent<InteractiveObject>();
			if (interactiveObject != null && ValidObjects.Contains(interactiveObject)) {
				ValidObjects.Remove(interactiveObject);
			}

			base.OnTriggerExit(other);
		}

	}

}