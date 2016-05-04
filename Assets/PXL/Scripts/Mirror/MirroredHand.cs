using System.Linq;
using Leap;
using Leap.Unity;
using PXL.Utility;
using UnityEditor;
using UnityEngine;

namespace PXL.Mirror {

	public class MirroredHand : ImprovedCapsuleHand {

		/// <summary>
		/// The object acting as a mirror in the X-axis
		/// </summary>
		public GameObject MirrorObject;

		/// <summary>
		/// The left hand in this scene
		/// </summary>
		private Hand leftHand;

		/// <summary>
		/// The LeapProvider of this scene
		/// </summary>
		private LeapProvider leapProvider;

		private void Start() {
			MirrorObject.AssertNotNull("Missing Mirror object!");
		}

		public override void SetLeapHand(Hand hand) {
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
				base.SetLeapHand(hand);
				return;
			}

			if (Hand == null)
				Hand = TestHandFactory.MakeTestHand(0, 0, false).TransformedCopy(UnityMatrixExtension.GetLeapMatrix(transform));

			if (leapProvider == null)
				leapProvider = FindObjectOfType<LeapProvider>();
			var frame = leapProvider.CurrentFrame;

			leftHand = frame.Hands.FirstOrDefault(h => h.IsLeft);
		}

		protected override void UpdateSpheres() {
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
				base.UpdateSpheres();
				return;
			}

			if (leftHand == null)
				return;

			var leftThumbBasePos = Vector3.zero;

			foreach (var item in Hand.Fingers.Select((finger, index) => new { finger, index })) {
				for (var i = 0; i < 4; i++) {
					var jointIndex = GetFingerJointIndex((int)item.finger.Type, i);
					var bone = leftHand.Fingers[item.index].Bone((Bone.BoneType)i);
					var originalPosition = bone.NextJoint.ToVector3();

					JointSpheres[jointIndex].position = GetMirroredPosition(originalPosition);

					if (jointIndex == ThumbBaseIndex)
						leftThumbBasePos = originalPosition;
				}
			}

			var mirroredPalmPos = GetMirroredPosition(leftHand.PalmPosition.ToVector3());

			PalmPositionSphere.position = mirroredPalmPos;
			WristPositionSphere.position = mirroredPalmPos;

			var leftThumbBaseToPalm = leftThumbBasePos - leftHand.PalmPosition.ToVector3();
			var newMockPose = leftHand.PalmPosition.ToVector3() + Vector3.Reflect(leftThumbBaseToPalm, leftHand.Basis.xBasis.ToVector3().normalized);
			MockThumbJointSphere.position = GetMirroredPosition(newMockPose);
		}

		protected override void UpdateArm() {
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
				base.UpdateArm();
				return;
			}

			if (leftHand == null)
				return;

			var arm = leftHand.Arm;
			var right = arm.Basis.xBasis.ToVector3().normalized * arm.Width * 0.7f * 0.5f;
			var wrist = arm.WristPosition.ToVector3();
			var elbow = arm.ElbowPosition.ToVector3();

			var armLength = Vector3.Distance(wrist, elbow);
			wrist -= arm.Direction.ToVector3() * armLength * 0.05f;

			ArmFrontRight.position = GetMirroredPosition(wrist + right);
			ArmFrontLeft.position = GetMirroredPosition(wrist - right);
			ArmBackRight.position = GetMirroredPosition(elbow + right);
			ArmBackLeft.position = GetMirroredPosition(elbow - right);
		}

		private Vector3 GetMirroredPosition(Vector3 origin) {
			var mirrorPos = MirrorObject.transform.position;
			var diffX = Mathf.Abs(mirrorPos.x - origin.x);
			return new Vector3(mirrorPos.x + diffX, origin.y, origin.z);
		}

	}

}