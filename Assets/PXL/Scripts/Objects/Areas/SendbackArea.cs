using System.Collections.Generic;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	public class SendbackArea : ObjectArea {

		protected List<InteractiveObject> ValidObjects = new List<InteractiveObject>();

		/// <summary>
		/// The Collider component of this object
		/// </summary>
		private Collider Collider {
			get { return mCollider ?? (mCollider = this.TryGetComponent<Collider>()); }
		}
		private Collider mCollider;

		protected override void Update() {
			base.Update();

			Extensions.PurgeIfNecessary(ref ValidObjects);

			foreach (var o in ValidObjects)
				TrySendBackObject(o);
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
			if (!Collider.bounds.Contains(interactiveObject.transform.position))
				return;

			var grabbable = interactiveObject.GetComponent<Grabbable>();

			if (grabbable == null || !grabbable.IsStationary())
				return;

			grabbable.transform.position = grabbable.PickupPosition;
			grabbable.transform.rotation = grabbable.PickupRotation;
		}

	}

}