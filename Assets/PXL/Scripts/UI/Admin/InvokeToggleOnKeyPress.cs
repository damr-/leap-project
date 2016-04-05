using PXL.Utility;
using UnityEngine;
using PXL.Utility.Toggle;
using UnityEngine.UI;

namespace PXL.UI.Admin {

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