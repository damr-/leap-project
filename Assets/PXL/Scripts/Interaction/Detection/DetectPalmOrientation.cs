using UnityEngine;
using Leap.Unity;
using UnityEditor;

namespace PXL.Interaction.Detection {

	public class DetectPalmOrientation : HandDetector {

		public Vector3 TargetRotation = Vector3.down;

		public float AngleError = 30f;

		public bool PreviewTargetRotation = true;

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
				var palmRot = palm.rotation.eulerAngles;
				var targetRotation = Quaternion.Euler(TargetRotation);
				var qDiff = targetRotation * Quaternion.Inverse(palm.rotation);

				transform.position = palm.position;
				transform.localRotation = targetRotation;

				DrawTargetRotation(palm.position, transform.up, new Color(76 / 255f, 176 / 255f, 76 / 255f));
				DrawTargetRotation(palm.position, transform.right, new Color(176 / 255f, 76 / 255f, 76 / 255f));
				DrawTargetRotation(palm.position, transform.forward, new Color(76 / 255f, 76 / 255f, 176 / 255f));
			}
		}

		private void DrawTargetRotation(Vector3 origin, Vector3 direction, Color color) {
			var len = 0.25f;

			Gizmos.color = color;

			direction = direction.normalized * len;

			Gizmos.DrawRay(origin, direction);

			Gizmos.color = new Color(color.r / 2f, color.g / 2f, color.b / 2f);

			//for (var i = 0; i < 359; i++) {
			//	var x = 0.145f * Mathf.Cos(i);
			//	var z = 0.145f * AngleError * Mathf.Sin(i);
			//	Gizmos.DrawRay(origin, center + new Vector3(x, 0, z));
			//}

			Gizmos.DrawRay(origin, direction);
		}

	}

}

//				if (handModel == null)
//					continue;
//				var palm = handModel.palm;
//var qRot = Quaternion.Euler(TargetRotation);
//var palmRot = palm.rotation.eulerAngles;

//var diff = qRot * Quaternion.Inverse(palm.rotation);

//var up = (Quaternion.Euler(palm.up) * diff).eulerAngles;
//var right = (Quaternion.Euler(palm.right) * diff).eulerAngles;
//var forward = (Quaternion.Euler(palm.forward) * diff).eulerAngles;

//				DrawTargetRotation(palm.position, up, new Color(76 / 255f, 176 / 255f, 76 / 255f));
//				DrawTargetRotation(palm.position, right, new Color(176 / 255f, 76 / 255f, 76 / 255f));
//				DrawTargetRotation(palm.position, forward, new Color(76 / 255f, 76 / 255f, 176 / 255f));