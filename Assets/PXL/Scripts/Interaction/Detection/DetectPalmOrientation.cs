using UnityEngine;
using Leap.Unity;

namespace PXL.Interaction.Detection {

	public class DetectPalmOrientation : HandDetector {

		/// <summary>
		/// The direction the normal of the hand's palm should be close to
		/// </summary>
		public Vector3 TargetDirection = Vector3.down;

		/// <summary>
		/// How many degrees away from the ideal <see cref="TargetDirection"/> the orientation is taken as correctly oriented
		/// </summary>
		public float AngleError = 30f;

		protected override void CheckHand(HandModel hand) {
			var leapHand = TryGetLeapHand(hand);
			if (leapHand == null)
				return;

			var palmNormal = leapHand.PalmNormal.ToVector3();
			var palmAngle = Vector3.Angle(TargetDirection, palmNormal);

			if (palmAngle < 30) {
				TryInvokeCorrect(hand);
			}
			else {
				TryInvokeIncorrect(hand);
			}
		}

	}

}