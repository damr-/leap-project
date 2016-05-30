using PXL.Utility;
using UnityEngine;
using PXL.Utility.Toggle;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	/// <summary>
	/// This script provides the functionality to toggle the referenced <see cref="Toggle"/> component as soon as a certain key has been pressed
	/// </summary>
	[RequireComponent(typeof(Toggle))]
	public class InvokeToggleOnKeyPress : ToggleBase {

		private Toggle ToggleComponent {
			get { return mToggle ?? (mToggle = this.TryGetComponent<Toggle>()); }
		}
		private Toggle mToggle;

		protected override void HandleKeyDown() {
			ToggleComponent.isOn = !ToggleComponent.isOn;
		}

	}

}