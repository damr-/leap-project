﻿using System;
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

		public bool DetectFingerPoses;

		public List<FingerPose> FingerPoses = new List<FingerPose>();

		public bool AllExtended;

		public bool AllContracted;

		public bool DetectGrabStrength;

		public bool LimitMinGrabStrength;

		public float MinGrabStrength;

		public bool LimitMaxGrabStrength;

		public float MaxGrabStrength;

		public FingerPose NewPose = new FingerPose();

		protected override void CheckHand(HandModel hand) {
			var leapHand = TryGetLeapHand(hand);
			if (leapHand == null)
				return;

			var correct = true;

			if (DetectFingerPoses)
				correct &= AreFingerPosesCorrect(leapHand);

			if (DetectGrabStrength)
				correct &= IsValidGrabStrength(leapHand);

			if(correct)
				TryInvokeCorrect(hand);
			else 
				TryInvokeIncorrect(hand);
		}

		private bool AreFingerPosesCorrect(Hand leapHand) {
			var fingers = leapHand.Fingers;

			if (AllExtended)
				return fingers.All(f => f.IsExtended);
			return AllContracted ? fingers.All(f => !f.IsExtended) : AreFingerPosesCorrect(fingers);
		}

		public void AddFingerPose() {
			if (NewPose.FingerType != Finger.FingerType.TYPE_UNKNOWN &&
				FingerPoses.Any(p => p.FingerType == NewPose.FingerType))
				return;
			FingerPoses.Add(NewPose);
			NewPose = new FingerPose();
		}

		private bool AreFingerPosesCorrect(List<Finger> fingers) {
			foreach (var fingerPose in FingerPoses) {
				var finger = fingers.First(f => f.Type == fingerPose.FingerType);
				if (finger.IsExtended == fingerPose.Extended)
					continue;

				return false;
			}
			return true;
		}

		private bool IsValidGrabStrength(Hand leapHand) {
			var success = true;

			if (LimitMinGrabStrength)
				success &= leapHand.GrabStrength >= MinGrabStrength;

			if (LimitMaxGrabStrength)
				success &= leapHand.GrabStrength <= MaxGrabStrength;
				
			return success;
		}

	}

}