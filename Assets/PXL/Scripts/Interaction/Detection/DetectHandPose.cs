using System;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity;

namespace PXL.Interaction.Detection {

	[Serializable]
	public class FingerPose {
		public Finger.FingerType FingerType;
		public bool Extended;
	}

	public class DetectHandPose : HandDetector {

		/// <summary>
		/// Whether a pose of the fingers should be detected
		/// </summary>
		public bool DetectFingerPoses;

		/// <summary>
		/// The poses of individual fingers which should be detected
		/// </summary>
		public List<FingerPose> FingerPoses = new List<FingerPose>();

		/// <summary>
		/// Whether the detection of the pose requires all fingers to be extended
		/// </summary>
		public bool AllExtended;

		/// <summary>
		/// Whether the detection of the pose requires all fingers to be contracted
		/// </summary>
		public bool AllContracted;

		/// <summary>
		/// Whether the grab strength should be observed and be a requirement for the detection
		/// </summary>
		public bool DetectGrabStrength;

		/// <summary>
		/// Whether there is a minimum grab strength limit which has to be surpassed for the pose to be valid
		/// </summary>
		public bool LimitMinGrabStrength;

		/// <summary>
		/// The minimum grab strength limit which has to be surpassed for the pose to be valid
		/// </summary>
		public float MinGrabStrength;

		/// <summary>
		/// Whether there is a maximum grab strength limit which mustn't be surpassed for the pose to be valid
		/// </summary>
		public bool LimitMaxGrabStrength;

		/// <summary>
		/// The maximum grab strength limit which mustn't be surpassed for the pose to be valid
		/// </summary>
		public float MaxGrabStrength;

		/// <summary>
		/// The new pose which will be added through the Editor
		/// </summary>
		public FingerPose NewPose = new FingerPose();

		/// <summary>
		/// The new hand for observation which will be added through the Editor
		/// </summary>
		public HandModel NewHand;

		/// <summary>
		/// Whether the incorrect pose subject can be invoked
		/// </summary>
		private bool canInvokeIncorrectImmediately;

		protected override void CheckHand(HandModel hand) {
			var leapHand = TryGetLeapHand(hand);
			if (leapHand == null) {
				if (!canInvokeIncorrectImmediately) 
					return;
				canInvokeIncorrectImmediately = false;
				InvokeIncorrect();
				return;
			}
			if (!canInvokeIncorrectImmediately)
				canInvokeIncorrectImmediately = true;

			var correctPose = true;

			if (DetectFingerPoses)
				correctPose &= AreFingerPosesCorrect(leapHand);

			if (DetectGrabStrength)
				correctPose &= IsValidGrabStrength(leapHand);

			if (correctPose)
				TryInvokeCorrect();
			else
				TryInvokeIncorrect();
		}

		/// <summary>
		/// Returns whether the poses of all fingers of the given hand are correct
		/// </summary>
		private bool AreFingerPosesCorrect(Hand leapHand) {
			var fingers = leapHand.Fingers;

			if (AllExtended)
				return fingers.All(f => f.IsExtended);
			return AllContracted ? fingers.All(f => !f.IsExtended) : AreFingerPosesCorrect(fingers);
		}

		/// <summary>
		/// Returns whether the given, individual fingers are all in the correct pose (extended or not extended)
		/// </summary>
		private bool AreFingerPosesCorrect(List<Finger> fingers) {
			foreach (var fingerPose in FingerPoses) {
				var finger = fingers.First(f => f.Type == fingerPose.FingerType);
				if (finger.IsExtended != fingerPose.Extended)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Returns whether the grab strength of the given hand is valid with the set limits
		/// </summary>
		/// <param name="leapHand">The Hand to check</param>
		private bool IsValidGrabStrength(Hand leapHand) {
			var validGrabSTrength = true;

			if (LimitMinGrabStrength)
				validGrabSTrength &= leapHand.GrabStrength >= MinGrabStrength;

			if (LimitMaxGrabStrength)
				validGrabSTrength &= leapHand.GrabStrength <= MaxGrabStrength;

			return validGrabSTrength;
		}

		/// <summary>
		/// Adds the <see cref="NewPose"/> to the fingerposes which should be detected, if the fingertype isn't observed yet
		/// </summary>
		public void AddFingerPose() {
			if (NewPose.FingerType != Finger.FingerType.TYPE_UNKNOWN &&
			    FingerPoses.Any(p => p.FingerType == NewPose.FingerType))
				return;
			FingerPoses.Add(NewPose);
			NewPose = new FingerPose();
		}

		/// <summary>
		/// Adds the <see cref="NewHand"/> to the observed hands, if it's not contained yet
		/// </summary>
		public void AddHand() {
			if (HandModels.Contains(NewHand))
				return;
			HandModels.Add(NewHand);
			NewHand = null;
		}

	}

}