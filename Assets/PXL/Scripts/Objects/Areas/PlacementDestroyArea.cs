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

			ValidObjects.Purge();

			foreach (var o in ValidObjects) {
				if (!IsObjectDropped(o))
					continue;

				if (++(CurrentDestroyAmount.Value) == WinDestroyAmount) {
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
			if (!ValidObjects.Contains(objectBehaviour))
				ValidObjects.Add(objectBehaviour);
		}

		protected override void HandleInvalidObjectType(ObjectBehaviour objectBehaviour) {
			Debug.Log("PlacementDestroyArea ignores invalid object types");
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

		/// <summary>
		/// Returns whether the given object is not grabbed and has close to no velocity
		/// </summary>
		protected virtual bool IsObjectDropped(ObjectBehaviour objectBehaviour) {
			var grabbable = objectBehaviour.GetComponent<Grabbable>();
			var rigidbody = objectBehaviour.GetComponent<Rigidbody>();
			return grabbable.IsGrabbed && rigidbody.velocity.Equal(Vector3.zero);
		}

	}

}