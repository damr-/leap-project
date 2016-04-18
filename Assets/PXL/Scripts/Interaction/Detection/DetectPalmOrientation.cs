using UnityEngine;
using Leap.Unity;

namespace PXL.Interaction.Detection {

	public class DetectPalmOrientation : HandDetector {

		/// <summary>
		/// The target rotation which should be detected
		/// </summary>
		public Vector3 TargetRotation = Vector3.down;

		/// <summary>
		/// How much difference in angle between the palm rotation and the target rotation may exist to be treated as valid.
		/// </summary>
		public float AngleError = 30f;

		/// <summary>
		/// Whether the orientation preview lines are visible
		/// </summary>
		public bool PreviewTargetRotation = true;
		
		/// <summary>
		/// The length of the orientation preview lines
		/// </summary>
		private const float LineLength = 0.15f;

		/// <summary>
		/// The color for the vertical axis, pointing towards the normal of the palm
		/// </summary>
		private readonly Color green = new Color(76 / 255f, 176 / 255f, 76 / 255f);

		/// <summary>
		/// The color for the horizontal axis, pointing to the right of the palm
		/// </summary>
		private readonly Color red = new Color(176 / 255f, 76 / 255f, 76 / 255f);

		/// <summary>
		/// The color for the horizontal axis, pointing forward from the palm
		/// </summary>
		private readonly Color blue = new Color(76 / 255f, 76 / 255f, 176 / 255f);

		protected override void CheckHand(HandModel hand) {
			var angle = Quaternion.Angle(hand.palm.localRotation, Quaternion.Euler(TargetRotation));

			if (angle < AngleError) {
				TryInvokeCorrect();
			}
			else {
				TryInvokeIncorrect();
			}
		}

		private void OnDrawGizmos() {
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

		/// <summary>
		/// Draws a with <see cref="color"/> dyed ray from origin in the normalized direction, multiplied by <see cref="LineLength"/>
		/// </summary>
		/// <param name="origin">The start position of the ray</param>
		/// <param name="direction">The direction of the ray which will be normalized and multiplied by <see cref="LineLength"/></param>
		/// <param name="color">The color of the ray</param>
		private void DrawTargetRotation(Vector3 origin, Vector3 direction, Color color) {
			Gizmos.color = color;
			direction = direction.normalized * LineLength;
			Gizmos.DrawRay(origin, direction);
		}

	}

}