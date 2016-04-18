using UnityEngine;

namespace PXL.Utility {

	public class DeactivateOnStart : MonoBehaviour {

		private void Start() {
			gameObject.SetActive(false);
		}

	}

}