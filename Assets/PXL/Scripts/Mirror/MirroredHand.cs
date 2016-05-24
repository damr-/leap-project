using System.Linq;
using Leap;
using Leap.Unity;
using PXL.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PXL.Mirror {

	public class MirroredHand : ImprovedCapsuleHand {

		/// <summary>
		/// The object acting as a mirror in the X-axis
		/// </summary>
		public GameObject MirrorObject;

		/// <summary>
		/// The original hand in this scene
		/// </summary>
		private Hand originalHand;

		/// <summary>
		/// The LeapProvider of this scene
		/// </summary>
		private LeapProvider leapProvider;

		/// <summary>
		/// The test hand used to setup this hand
		/// </summary>
		private Hand initialTestHand;

		/// <summary>
		/// The HandRepresentation for this hand
		/// </summary>
		private HandRepresentation handRepresentation;

		public void Setup() {
			MirrorObject.AssertNotNull("Missing Mirror object!");

			initialTestHand = MakeTestHand();
			handRepresentation = FindObjectOfType<HandPool>().MakeHandRepresentation(initialTestHand, ModelType.Graphics);

			BeginHand();
		}

		protected void Update() {
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying) {
				SetLeapHand(MakeTestHand());
				UpdateHand();
			}
			if (!Utility.EditorUtility.IsPlaying())
				return;
#endif
			if (handRepresentation != null)
				handRepresentation.UpdateRepresentation(initialTestHand);
		}

		public override void SetLeapHand(Hand hand) {
#if UNITY_EDITOR
			if (!Utility.EditorUtility.IsPlaying()) {
				base.SetLeapHand(hand);
				return;
			}
#endif
			if (Hand == null)
				Hand = MakeTestHand();

			if (leapProvider == null)
				leapProvider = FindObjectOfType<LeapProvider>();

			var originalHandLeft = Handedness != Chirality.Left;

			originalHand = leapProvider.CurrentFrame.Hands.FirstOrDefault(h => h.IsLeft == originalHandLeft);
			if (originalHand != null)
				BeginHand();
			else
				FinishHand();
		}

		protected override void UpdateSpheres() {
#if UNITY_EDITOR
			if (!Utility.EditorUtility.IsPlaying()) {
				base.UpdateSpheres();
				return;
			}
#endif
			if (handRepresentation == null)
				return;

			if (originalHand == null) {
				handRepresentation.Finish();
				handRepresentation = null;
				return;
			}

			var leftThumbBasePos = Vector3.zero;

			foreach (var item in Hand.Fingers.Select((finger, index) => new { finger, index })) {
				for (var i = 0; i < 4; i++) {
					var jointIndex = GetFingerJointIndex((int)item.finger.Type, i);
					var bone = originalHand.Fingers[item.index].Bone((Bone.BoneType)i);
					var originalPosition = bone.NextJoint.ToVector3();

					JointSpheres[jointIndex].position = GetMirroredPosition(originalPosition);

					if (jointIndex == ThumbBaseIndex)
						leftThumbBasePos = originalPosition;
				}
			}

			var mirroredPalmPos = GetMirroredPosition(originalHand.PalmPosition.ToVector3());

			PalmPositionSphere.position = mirroredPalmPos;
			WristPositionSphere.position = mirroredPalmPos;

			var leftThumbBaseToPalm = leftThumbBasePos - originalHand.PalmPosition.ToVector3();
			var newMockPose = originalHand.PalmPosition.ToVector3() + Vector3.Reflect(leftThumbBaseToPalm, originalHand.Basis.xBasis.ToVector3().normalized);
			MockThumbJointSphere.position = GetMirroredPosition(newMockPose);
		}

		protected override void UpdateArm() {
#if UNITY_EDITOR
			if (!Utility.EditorUtility.IsPlaying()) {
				base.UpdateArm();
				return;
			}
#endif
			if (originalHand == null)
				return;

			var arm = originalHand.Arm;
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
			if (Handedness == Chirality.Left)
				diffX *= -1;
			return new Vector3(mirrorPos.x + diffX, origin.y, origin.z);
		}

		private Hand MakeTestHand() {
			return TestHandFactory.MakeTestHand(0, 0, Handedness == Chirality.Left).TransformedCopy(transform.GetLeapMatrix());
		}

	}

}