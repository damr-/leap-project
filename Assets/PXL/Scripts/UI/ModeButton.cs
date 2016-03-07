using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(Button))]
	public class ModeButton : MonoBehaviour {
	
		/// <summary>
		/// The button which is used for switching mode
		/// </summary>
		public KeyCode switchModeKey = KeyCode.Y;

		/// <summary>
		/// The button component of this GameObject
		/// </summary>
		private Button button;

		protected virtual void Start() {
			button = this.TryGetComponent<Button>();
		}

		protected virtual void Update() {
			if (Input.GetKeyDown(switchModeKey)) {
				button.onClick.Invoke();
				AdminUIBase.ToggleMode();
			}
		}

	}

}