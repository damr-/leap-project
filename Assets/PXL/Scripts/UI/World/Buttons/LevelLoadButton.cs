using UnityEngine.UI;

namespace PXL.UI.World.Buttons {

	public class LevelLoadButton : TouchDurationButton {

		/// <summary>
		/// The index of the level which will be loaded when this button is pressed
		/// </summary>
		public int LevelIndex { get; set; }

		/// <summary>
		/// The Text component of this object's child
		/// </summary>
		public Text ButtonText {
			get { return mButtonText ?? (mButtonText = GetComponentInChildren<Text>()); }
		}
		private Text mButtonText;

	}

}