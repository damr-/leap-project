using System.Collections.Generic;
using UnityEngine;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;

namespace PXL.Objects.Areas {

	public class PlacementDestroyArea : DestroyArea {

		protected List<ObjectBehaviour> ValidObjects = new List<ObjectBehaviour>();

		protected override void Update() {
			base.Update();

			if (GameMode.GameWon || !AreaActive)
				return;

			ValidObjects = ValidObjects.Purge();

			foreach (var o in ValidObjects) {
				if (!o.GetComponent<Grabbable>().IsStationary)
					continue;

				CurrentDestroyAmount.Value++;
				if (CurrentDestroyAmount.Value == WinDestroyAmount) {
					HandleGameWon();
					o.DestroyObject();
					break;
				}
				o.DestroyObject();
			}
		}

		/// <summary>
		/// Adds the new object to <see cref="ValidObjects"/>
		/// </summary>
		protected override void HandleValidObjectType(ObjectBehaviour objectBehaviour) {
			base.HandleValidObjectType(objectBehaviour);

			if (!ValidObjects.Contains(objectBehaviour))
				ValidObjects.Add(objectBehaviour);
		}

		/// <summary>
		/// Removes the object from <see cref="ValidObjects"/>
		/// </summary>
		protected override void OnTriggerExit(Collider other) {
			var objectBehaviour = other.GetComponent<ObjectBehaviour>();
			if (objectBehaviour != null && ValidObjects.Contains(objectBehaviour)) {
				ValidObjects.Remove(objectBehaviour);
			}

			base.OnTriggerExit(other);
		}

	}

}