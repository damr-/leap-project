using UnityEngine;

namespace PXL.Utility {

	public abstract class ToggleBase : MonoBehaviour {
	
		/// <summary>
		/// The key used to toggle the GameObjects
		/// </summary>
		public KeyCode ToggleKey;

		private void Update() {
			if (Input.GetKeyDown(ToggleKey)) {
				HandleKeyDown();
			}
		}

		protected abstract void HandleKeyDown();

	}

}
