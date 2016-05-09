using UnityEngine;
using Leap.Unity;

namespace PXL.Interaction.Detection {

	public class DetectPalmOrientation : HandDetector {

		/// <summary>
		/// The target rotation which should be detected
		/// </summary>
		public Vector3 TargetRotation = Vector3.zero;

		/// <summary>
		/// Axes with values > 0 will be ignored in the detection process
		/// </summary>
		public Vector3 IgnoreRotation = Vector3.zero;

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
		/// The color for the horizontal axis, pointing to the right of the palm
		/// </summary>
		private readonly Color red = new Color(1f, 42 / 255f, 42 / 255f);

		/// <summary>
		/// The color for the vertical axis, pointing towards the normal of the palm
		/// </summary>
		private readonly Color green = new Color(42 / 255f, 1f, 42 / 255f);

		/// <summary>
		/// The color for the horizontal axis, pointing forward from the palm
		/// </summary>
		private readonly Color blue = new Color(42 / 255f, 42 / 255f, 1f);

		protected override bool CheckHand(HandModel hand) {
			var handRotation = hand.palm.localRotation.eulerAngles;
			var targetRotation = TargetRotation;
			for (var i = 0; i < 3; i++) {
				if (!(IgnoreRotation[i] > 0f)) 
					continue;

				handRotation[i] = 0f;
				targetRotation[i] = 0f;
			}

			var angle = Quaternion.Angle(Quaternion.Euler(handRotation), Quaternion.Euler(targetRotation));

			if (PreviewTargetRotation) {
				Debug.DrawRay(hand.palm.position + new Vector3(0.001f, 0, 0), hand.palm.forward * 0.15f, Color.cyan, Time.deltaTime);
				Debug.DrawRay(hand.palm.position + new Vector3(0, 0, 0.001f), hand.palm.right * 0.15f, Color.magenta, Time.deltaTime);
				Debug.DrawRay(hand.palm.position + new Vector3(0.001f, 0, 0), hand.palm.up * 0.15f, Color.yellow, Time.deltaTime);
			}

			return angle < AngleError;
		}

		/// <summary>
		/// Draw the axes of the palm's rotation, which should be detected
		/// </summary>
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
		/// Draws a ray (with <see cref="color"/>) from origin in the normalized direction, multiplied by <see cref="LineLength"/>
		/// </summary>
		/// <param name="origin">The start position of the ray</param>
		/// <param name="direction">The direction of the ray which will be normalized and multiplied by <see cref="LineLength"/></param>
		/// <param name="color">The color of the ray</param>
		private static void DrawTargetRotation(Vector3 origin, Vector3 direction, Color color) {
			Gizmos.color = color;
			direction = direction.normalized * LineLength;
			Gizmos.DrawRay(origin, direction);
		}

	}

}