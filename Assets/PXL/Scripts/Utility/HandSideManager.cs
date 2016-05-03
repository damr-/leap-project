using UnityEngine;
using UnityEngine.UI;

namespace PXL.Utility {

	public class HandSideManager : MonoBehaviour {

		/// <summary>
		/// The button of the left hand menu to switch hand
		/// </summary>
		public Button LeftSideChangeButton;

		/// <summary>
		/// The button of the rightleft hand menu to switch hand
		/// </summary>
		public Button RightSideChangeButton;

		/// <summary>
		/// Loads the last saved hand menu side and updates the menues accordingly
		/// </summary>
		private void OnEnable() {
			var leftHandMenu = true;

			var val = PlayerPrefs.GetInt("LeftHandMenu", -1);
			if (val != -1)
				leftHandMenu = val != 0;

			AssertReferences();

			LeftSideChangeButton.onClick.AddListener(() => UpdateSide(false));
			RightSideChangeButton.onClick.AddListener(() => UpdateSide(true));

			if (leftHandMenu)
				RightSideChangeButton.onClick.Invoke();
			else
				LeftSideChangeButton.onClick.Invoke();
		}

		/// <summary>
		/// Assert that all needed references are valid
		/// </summary>
		private void AssertReferences() {
			LeftSideChangeButton.AssertNotNull("Missing button reference");
			RightSideChangeButton.AssertNotNull("Missing button reference!");
		}

		/// <summary>
		/// Save the new active hand menu side
		/// </summary>
		/// <param name="isLeft">Whether the active menu is the one of the left hand</param>
		public void UpdateSide(bool isLeft) {
			PlayerPrefs.SetInt("LeftHandMenu", isLeft ? 1 : 0);
		}

	}

}