using UnityEngine;
using UnityEngine.UI;

namespace PXL.Utility {

	public class HandSideManager : MonoBehaviour {

		public GameObject LeftHandDetector;

		public GameObject RightHandDetector;

		public GameObject LeftHandManuCanvas;

		public GameObject RightHandMenuCanvas;

		public Button LeftSideChangeButton;

		public Button RightSideChangeButton;

		public static bool LeftHandMenu = true;

		private void OnEnable() {
			var val = PlayerPrefs.GetInt("LeftHandMenu", -1);
			if (val != -1)
				LeftHandMenu = val != 0;

			AssertReferences();

			LeftHandDetector.SetActive(LeftHandMenu);
			RightHandDetector.SetActive(!LeftHandMenu);
			LeftHandManuCanvas.SetActive(LeftHandMenu);
			RightHandMenuCanvas.SetActive(!LeftHandMenu);

			LeftSideChangeButton.onClick.AddListener(() => UpdateSide(false));
			RightSideChangeButton.onClick.AddListener(() => UpdateSide(true));
		}

		private void AssertReferences() {
			LeftHandDetector.AssertNotNull("Missing left hand detector!");
			RightHandDetector.AssertNotNull("Missing right hand detector!");
			LeftHandManuCanvas.AssertNotNull("Missing left hand menu canvas!");
			RightHandMenuCanvas.AssertNotNull("Missing left hand menu canvas!");
			LeftSideChangeButton.AssertNotNull("Missing button reference");
			RightSideChangeButton.AssertNotNull("Missing button reference!");
		}

		public void UpdateSide(bool isLeft) {
			LeftHandMenu = isLeft;
		}

		private void OnDisable() {
			PlayerPrefs.SetInt("LeftHandMenu", LeftHandMenu ? 1 : 0);
		}

	}

}