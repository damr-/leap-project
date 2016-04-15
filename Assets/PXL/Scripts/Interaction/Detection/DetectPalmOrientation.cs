using UnityEngine;
using Leap.Unity;

namespace PXL.Interaction.Detection {

	public class DetectPalmOrientation : HandDetector {

		public Vector3 TargetRotation = Vector3.down;

		public float AngleError = 30f;

		public bool PreviewTargetRotation = true;

		private const float LineLength = 0.25f;

		private readonly Color green = new Color(76 / 255f, 176 / 255f, 76 / 255f);

		private readonly Color red = new Color(176 / 255f, 76 / 255f, 76 / 255f);

		private readonly Color blue = new Color(76 / 255f, 76 / 255f, 176 / 255f);

		protected override void CheckHand(HandModel hand) {
			var angle = Quaternion.Angle(hand.palm.localRotation, Quaternion.Euler(TargetRotation));

			if (angle < AngleError) {
				TryInvokeCorrect(hand);
			}
			else {
				TryInvokeIncorrect(hand);
			}
		}

		void OnDrawGizmos() {
			if (!PreviewTargetRotation)
				return;

			foreach (var handModel in HandModels) {
				if (handModel == null)
					continue;

				var palm = handModel.palm;
				transform.position = palm.position;
				transform.localRotation = Quaternion.Euler(TargetRotation);

				DrawTargetRotation(palm.position, transform.up, green);
				DrawTargetRotation(palm.position, transform.right, red);
				DrawTargetRotation(palm.position, transform.forward, blue);
			}
		}

		private void DrawTargetRotation(Vector3 origin, Vector3 direction, Color color) {
			Gizmos.color = color;
			direction = direction.normalized * LineLength;
			Gizmos.DrawRay(origin, direction);
		}

	}

}