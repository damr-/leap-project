﻿using PXL.UI.Admin;
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

			TrySetState(!IsToggled.Value);
		}

		private void TrySetState(bool isEnabled) {
			if (OnlyAdmin && !AdminUiBase.IsAdmin)
				return;

			IsToggled.Value = isEnabled;
			HandleKeyDown();
		}

		protected abstract void HandleKeyDown();

		/// <summary>
		/// Simulates pressing the toggle key
		/// </summary>
		public void Toggle() {
			TrySetState(!IsToggled.Value);
		}

		public void Enable() {
			TrySetState(true);

		}

		public void Disable() {
			TrySetState(false);
		}

	}

}
