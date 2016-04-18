using UnityEngine;

namespace PXL.UI.World.Hand {

	public class HandMenu : MonoBehaviour {

		private readonly Vector3 origin = new Vector3(
			0f,
			0.12f,
			-0.057f
		);

		private void LateUpdate() {
			transform.rotation = Quaternion.LookRotation(transform.position, Vector3.up);
		}

	}

}