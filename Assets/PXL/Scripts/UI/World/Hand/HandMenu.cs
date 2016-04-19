using UnityEngine;

namespace PXL.UI.World.Hand {

	public class HandMenu : MonoBehaviour {

		public Vector3 Origin = new Vector3(
			-0.05f,
			0.12f,
			-0.057f
		);

		private void LateUpdate() {
			transform.rotation = Quaternion.LookRotation(transform.position - Origin);
		}

	}

}