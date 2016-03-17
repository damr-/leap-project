using UnityEngine;

namespace PXL.Utility {

	public class HideInGame : MonoBehaviour {

		private void Awake() {
			GetComponent<Renderer>().enabled = false;
		}

	}

}