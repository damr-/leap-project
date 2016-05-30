using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Buttons {

	/// <summary>
	/// This script sets up the Reloadlevel button by finding the scene's <see cref="LevelInput"/> and
	/// telling the button to restart the level when it's pressed
	/// </summary>
	public class SetupReloadLevelButton : MonoBehaviour {

		/// <summary>
		/// The <see cref="LevelInput"/> of this scene
		/// </summary>
		private LevelInput levelInput;

		/// <summary>
		/// The Button component of this object
		/// </summary>
		public Button Button {
			get { return mButton ?? (mButton = GetComponentInChildren<Button>()); }
		}
		private Button mButton;

		private void Start() {
			levelInput = FindObjectOfType<LevelInput>();
			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(() => levelInput.StartReloadLevel());
		}

	}

}