using System.Collections.Generic;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	public class SendbackArea : ObjectArea {

		protected List<InteractiveObject> ValidObjects = new List<InteractiveObject>();

		protected override void Update() {
			base.Update();

			if (GameMode.GameOver || !AreaActive)
				return;

			Extensions.PurgeIfNecessary(ref ValidObjects);

			foreach (var o in ValidObjects) {
				TrySendBackObject(o);
			}
		}

		/// <summary>
		/// Adds the new object to <see cref="ValidObjects"/>
		/// </summary>
		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
			if (!ValidObjects.Contains(interactiveObject))
				ValidObjects.Add(interactiveObject);
		}

		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {
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

		/// <summary>
		/// Sends the given object back if the it has a grabbable component and it is stationary.
		/// </summary>
		private void TrySendBackObject(InteractiveObject interactiveObject) {
			var grabbable = interactiveObject.GetComponent<Grabbable>();

			if (grabbable == null || !grabbable.IsStationary())
				return;

			grabbable.transform.position = grabbable.PickupPosition;
		}

	}

}