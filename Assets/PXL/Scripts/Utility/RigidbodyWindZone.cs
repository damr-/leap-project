using PXL.Objects;
using UnityEngine;
using PXL.Objects.Areas;

namespace PXL.Utility {

	public class RigidbodyWindZone : ObjectArea {

		public Vector3 Force;

		public Vector3 MaxVelocity;

		protected override void Update() {
			base.Update();

			foreach (var keyValuePair in Objects) {
				var r = keyValuePair.Key.GetComponent<Rigidbody>();

				if (r == null)
					continue;

				var actualForce = Vector3.zero;

				for (var i = 0; i < 3; i++) {
					if (Mathf.Abs(r.velocity[i]) >= MaxVelocity[i])
						continue;

					if (Force[i] < 0 || Force[i] > 0)
						actualForce[i] = Force[i];
				}
				r.AddForce(actualForce, ForceMode.Force);
			}
		}

		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {

		}

		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {

		}

	}

}