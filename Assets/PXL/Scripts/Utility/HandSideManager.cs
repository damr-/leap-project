using UnityEngine;
using UnityEngine.UI;

namespace PXL.Utility {

	public class HandSideManager : MonoBehaviour {

		/// <summary>
		/// The detector for the left hand orientation and finger positions
		/// </summary>
		public GameObject LeftHandDetector;

		/// <summary>
		/// The detector for the right hand orientation and finger positions
		/// </summary>
		public GameObject RightHandDetector;

		/// <summary>
		/// The menu canvas of the left hand
		/// </summary>
		public GameObject LeftHandMenuCanvas;

		/// <summary>
		/// The menu canvas of the right hand
		/// </summary>
		public GameObject RightHandMenuCanvas;

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

			LeftHandDetector.SetActive(leftHandMenu);
			RightHandDetector.SetActive(!leftHandMenu);
			LeftHandMenuCanvas.SetActive(leftHandMenu);
			RightHandMenuCanvas.SetActive(!leftHandMenu);

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
			LeftHandDetector.AssertNotNull("Missing left hand detector!");
			RightHandDetector.AssertNotNull("Missing right hand detector!");
			LeftHandMenuCanvas.AssertNotNull("Missing left hand menu canvas!");
			RightHandMenuCanvas.AssertNotNull("Missing left hand menu canvas!");
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