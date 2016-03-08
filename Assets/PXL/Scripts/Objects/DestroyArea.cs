using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public class DestroyArea : TargetArea {
	
		/// <summary>
		/// Destroy the overlapping object if it has an ObjectBehaviour Component
		/// </summary>
		/// <param name="other">The Collider of the overlapping object</param>
		protected override void HandleValidOther(Collider other) {
			ObjectBehaviour objectBehaviour = other.TryGetComponent<ObjectBehaviour>();
			objectBehaviour.AssertNotNull("GameObject '" + other.gameObject.name + "' has tag '" + targetTag + "' but no component ObjectBehaviour!");
			objectBehaviour.DestroyObject();
		}
	}

}
