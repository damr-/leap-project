using UnityEngine;

namespace PXL.UI.World.Hand {

	public class HandMenu : MonoBehaviour {

		/// <summary>
		/// If the origin should automatically be set to the position of the outermost parent
		/// </summary>
		public bool OriginIsOutermostParent = true;

		public Vector3 Origin = new Vector3(
			-0.05f,
			0.12f,
			-0.057f
		);

		private void Start() {
			if (!OriginIsOutermostParent)
				return;

			var o = transform;
			while (o.parent != null)
				o = o.parent;
			Origin = o.position;
		}

		private void LateUpdate() {
			transform.rotation = Quaternion.LookRotation(transform.position - Origin);
		}

	}

}