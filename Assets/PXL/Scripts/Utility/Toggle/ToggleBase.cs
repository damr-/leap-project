using PXL.UI.Admin;
using UnityEngine;

namespace PXL.Utility.Toggle {

	public abstract class ToggleBase : MonoBehaviour {

		/// <summary>
		/// The key used to toggle the GameObjects
		/// </summary>
		public KeyCode ToggleKey;

		/// <summary>
		/// Whether only the admin can toggle this
		/// </summary>
		public bool OnlyAdmin = true;

		private void Update() {
			if (!Input.GetKeyDown(ToggleKey))
				return;

			if (!OnlyAdmin) {
				HandleKeyDown();
			}
			else if (AdminUiBase.IsAdmin) {
				HandleKeyDown();
			}
		}

		protected abstract void HandleKeyDown();

		public void Toggle() {
			HandleKeyDown();
		}

	}

}
