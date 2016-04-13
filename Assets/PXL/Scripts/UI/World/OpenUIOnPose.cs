using Leap.Unity;
using PXL.Interaction;
using UnityEngine;
using UniRx;

namespace PXL.UI.World {

	public class OpenUIOnPose : MonoBehaviour {

		public DetectPalmOrientation DetectPalmRotation;

		public Canvas TargetCanvas;

		private void Start() {
			DetectPalmRotation.CorrectOrientation.Subscribe(HandleCorrectOrientation);
			DetectPalmRotation.WrongOrientation.Subscribe(HandleWrongOrientation);
		}

		private void HandleCorrectOrientation(HandModel hand) {
			TargetCanvas.enabled = true;
		}

		private void HandleWrongOrientation(HandModel hand) {
			TargetCanvas.enabled = false;
		}

	}

}
