using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity;
using PXL.Utility;
using UniRx;

namespace PXL.Interaction {

	public class DetectPalmOrientation : MonoBehaviour {

		/// <summary>
		/// The hands which palms should be observed
		/// </summary>
		public List<HandModel> HandModels = new List<HandModel>();

		/// <summary>
		/// The direction the normal of the hand's palm should be close to
		/// </summary>
		public Vector3 TargetDirection = Vector3.down;

		/// <summary>
		/// How many degrees away from the ideal <see cref="TargetDirection"/> the orientation is taken as correctly oriented
		/// </summary>
		public float AngleError = 30f;

		/// <summary>
		/// Invoked every 1 / <see cref="InvokeFrequency"/>, when the palm has the right orientation
		/// </summary>
		public IObservable<HandModel> CorrectOrientation { get { return correctOrientationSubject; } }
		private readonly ISubject<HandModel> correctOrientationSubject = new Subject<HandModel>();

		/// <summary>
		/// Invoked once the orientation is wrong again
		/// </summary>
		public IObservable<HandModel> WrongOrientation { get { return wrongOrientationSubject; } }
		private readonly ISubject<HandModel> wrongOrientationSubject = new Subject<HandModel>();

		/// <summary>
		/// The last time the Observable was invoked
		/// </summary>
		private float lastInvokeTime;

		/// <summary>
		/// Whether the <see cref="WrongOrientation"/> Observable can be invoked
		/// </summary>
		private bool canInvokeWrongOrientation = true;

		/// <summary>
		/// How often per second <see cref="CorrectOrientation"/> should be invoked
		/// </summary>
		private const float InvokeFrequency = 2f;

		private void Start() {
			if (HandModels.Count == 0)
				throw new MissingReferenceException("Missing hand models!");
			HandModels.ForEach(hm => hm.AssertNotNull("HandModel reference null!"));
		}

		private void Update() {
			for (var i = 0; i < 2; i++) {
				CheckHand(HandModels[i]);
			}
		}

		private void CheckHand(HandModel hand) {
			if (!hand.gameObject.activeInHierarchy)
				return;

			var leapHand = hand.GetLeapHand();

			if (leapHand == null || leapHand.GrabStrength < 1)
				return;

			var fingers = leapHand.Fingers;

			var fist = fingers.All(finger => !finger.IsExtended);

			if (!fist)
				return;

			var handDir = leapHand.Direction.ToVector3();
			var palmNormal = leapHand.PalmNormal.ToVector3();

			Debug.DrawRay(hand.GetPalmPosition(), handDir * 100f, Color.red, 0.2f);
			Debug.DrawRay(hand.GetPalmPosition(), palmNormal * 100f, Color.green, 0.2f);

			var palmAngle = Vector3.Angle(TargetDirection, palmNormal);
			if (palmAngle < 30) {
				if (!(Time.time - lastInvokeTime > 1 / InvokeFrequency))
					return;
				correctOrientationSubject.OnNext(hand);
				lastInvokeTime = Time.time;
				canInvokeWrongOrientation = true;
			}
			else if (canInvokeWrongOrientation) {
				wrongOrientationSubject.OnNext(hand);
			}
		}

	}

}