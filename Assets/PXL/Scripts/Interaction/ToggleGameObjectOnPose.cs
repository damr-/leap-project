using UnityEngine;
using UniRx;
using System.Collections.Generic;
using PXL.Interaction.Detection;

namespace PXL.Interaction {

	public class ToggleGameObjectOnPose : MonoBehaviour {

		public DetectPalmOrientation DetectPalmOrientation;
		public DetectHandPose DetectHandPose;

		private bool correctOrientation;
		private bool correctPose;

		public List<GameObject> GameObjects = new List<GameObject>();

		private void Start() {
			DetectPalmOrientation.CorrectPose.Subscribe(_ => UpdateFlag(ref correctOrientation, true));
			DetectPalmOrientation.IncorrectPose.Subscribe(_ => UpdateFlag(ref correctOrientation, false));

			DetectHandPose.CorrectPose.Subscribe(_ => UpdateFlag(ref correctPose, true));
			DetectHandPose.IncorrectPose.Subscribe(_ => UpdateFlag(ref correctPose, false));
		}

		private void UpdateFlag(ref bool flag, bool value) {
			flag = value;
			if (value)
				EnableObjectsIfPossible();
			else 
				EnableObjects(false);
		}

		private void EnableObjectsIfPossible() {
			if (correctOrientation && correctPose)
				EnableObjects(true);
		}

		private void EnableObjects(bool newEnabledState) {
			GameObjects.ForEach(c => c.SetActive(newEnabledState));
		}

	}

}