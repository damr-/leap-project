using System.Collections.Generic;
using Leap.Unity;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction.Detection {

	public abstract class HandDetector : MonoBehaviour {

		/// <summary>
		/// The hands which palms should be observed
		/// </summary>
		public List<HandModel> HandModels = new List<HandModel>();

		public IObservable<HandModel> CorrectPose { get { return correctPoseSubject; } }
		private readonly ISubject<HandModel> correctPoseSubject = new Subject<HandModel>();

		public IObservable<HandModel> IncorrectPose { get { return incorrectPoseSubject; } }
		private readonly ISubject<HandModel> incorrectPoseSubject = new Subject<HandModel>();

		protected float LastCorrectInvokeTime;

		protected float LastIncorrectInvokeTime;

		protected const float CorrectInvokeFrequency = 2f;

		protected const float IncorrectInvokeFrequency = 2f;

		protected virtual void Start() {
			if (HandModels.Count == 0)
				throw new MissingReferenceException("Missing hand models!");
			HandModels.ForEach(hm => hm.AssertNotNull("HandModel reference null!"));
		}

		protected virtual void Update() {
			foreach (var handModel in HandModels) {
				CheckHand(handModel);
			}
		}

		protected abstract void CheckHand(HandModel hand);

		protected Leap.Hand TryGetLeapHand(HandModel hand) {
			return !hand.gameObject.activeInHierarchy ? null : hand.GetLeapHand();
		}

		protected void TryInvokeCorrect(HandModel currentHand) {
			if (!(Time.time - LastCorrectInvokeTime > 1 / CorrectInvokeFrequency))
				return;

			correctPoseSubject.OnNext(currentHand);
			LastCorrectInvokeTime = Time.time;
		}

		protected void TryInvokeIncorrect(HandModel currentHand) {
			if (!(Time.time - LastIncorrectInvokeTime > 1 / IncorrectInvokeFrequency))
				return;

			incorrectPoseSubject.OnNext(currentHand);
			LastIncorrectInvokeTime = Time.time;
		}

	}

}