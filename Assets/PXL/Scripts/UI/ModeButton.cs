using PXL.UI.Admin;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(Button))]
	public class ModeButton : MonoBehaviour {

		/// <summary>
		/// The button which is used for switching mode
		/// </summary>
		public KeyCode SwitchModeKey = KeyCode.Y;

		/// <summary>
		/// The button component of this GameObject
		/// </summary>
		private Button button;

		protected virtual void Start() {
			button = this.TryGetComponent<Button>();
		}

		protected virtual void Update() {
			if (!Input.GetKeyDown(SwitchModeKey)) 
				return;

			button.onClick.Invoke();
		}

		/// <summary>
		/// Called by the button when it's clicked
		/// </summary>
		public void ToggleMode() {
			AdminUiBase.ToggleMode();
		}

	}

}