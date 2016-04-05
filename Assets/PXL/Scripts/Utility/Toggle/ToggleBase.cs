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

		/// <summary>
		/// Whether the toggle is currently ON
		/// </summary>
		public ObservableProperty<bool> IsToggled = new ObservableProperty<bool>();

		/// <summary>
		/// Checks for keyboard input
		/// </summary>
		private void Update() {
			if (!Input.GetKeyDown(ToggleKey))
				return;

			TryToggle();
		}

		/// <summary>
		/// Checks whether admin is required and the admin mode is active and calles <see cref="HandleKeyDown"/>
		/// </summary>
		private void TryToggle() {
			if (OnlyAdmin && !AdminUiBase.IsAdmin)
				return;

			IsToggled.Value = !IsToggled.Value;
			HandleKeyDown();
		}

		protected abstract void HandleKeyDown();

		/// <summary>
		/// Simulates pressing the toggle key
		/// </summary>
		public void Toggle() {
			TryToggle();
		}

	}

}
