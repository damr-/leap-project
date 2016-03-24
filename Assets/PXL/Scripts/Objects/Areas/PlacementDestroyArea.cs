using System.Collections.Generic;
using UnityEngine;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;

namespace PXL.Objects.Areas {

	public class PlacementDestroyArea : DestroyArea {

		protected List<InteractiveObject> ValidObjects = new List<InteractiveObject>();

		protected override void Update() {
			if (GameMode.GameOver || !AreaActive)
				return;

			Extensions.PurgeIfNecessary(ref ValidObjects);

			foreach (var o in ValidObjects) {
				if (!o.GetComponent<Grabbable>().IsStationary())
					continue;

				CurrentDestroyAmount.Value++;
				if (CurrentDestroyAmount.Value == WinDestroyAmount) {
					HandleGameWon();
					o.Kill();
					break;
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