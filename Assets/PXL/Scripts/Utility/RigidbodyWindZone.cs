using System.Linq;
using PXL.Objects;
using UnityEngine;
using PXL.Objects.Areas;

namespace PXL.Utility {

	/// <summary>
	/// This script keeps up the velocity of all rigidbodies within it's bounds.
	/// </summary>
	public class RigidbodyWindZone : ObjectArea {

		/// <summary>
		/// The force which gets applied to objects inside this area
		/// </summary>
		public Vector3 Force;

		/// <summary>
		/// The maximum speed an object can have
		/// </summary>
		[Range(0.1f, 10f)]
		public float MaxSpeed;

		protected override void Update() {
			base.Update();

			foreach (var r in Objects.Select(keyValuePair => keyValuePair.Key.GetComponent<Rigidbody>()).Where(r => r != null)) {
				if (r.velocity.magnitude > MaxSpeed) {
					var correctVelocity = r.velocity.normalized * MaxSpeed;
					r.velocity = Vector3.Lerp(r.velocity, correctVelocity, Time.deltaTime * 1.5f);
				}
				else {
					var modifiedForce = Force * (1 - r.velocity.magnitude / MaxSpeed);
					var force = Vector3.Lerp(Force, modifiedForce, Vector3.Dot(r.velocity, Force));
					r.AddForce(force, ForceMode.Force);
				}
			}
		}

		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
		}

		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {
		}

	}

}