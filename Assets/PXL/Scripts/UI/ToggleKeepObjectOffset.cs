using PXL.UI;
using UnityEngine;

namespace PXL.Interaction {

	public class ToggleKeepObjectOffset : DisplayBase {

		/// <summary>
		/// The key to toggle the offset mode
		/// </summary>
		public KeyCode toggleKey = KeyCode.B;

		protected override void Start() {
			base.Start();
			text.text = Grabbable.keepObjectOffset + "";
		}

		private void Update() {
			if(Input.GetKeyDown(toggleKey)) {
				Grabbable.keepObjectOffset = !Grabbable.keepObjectOffset;
				text.text = Grabbable.keepObjectOffset + "";
			}
		}

	}

}