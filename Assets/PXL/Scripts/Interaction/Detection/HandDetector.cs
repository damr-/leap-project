using System.Collections.Generic;
using Leap.Unity;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction.Detection {

	/// <summary>
	/// This class provides basic abstract functionality to detect a certain state of a hand.
	/// It provides observables for incorrect and correct poses, which will be invoked automatically.
	/// The given handmodels are being observed every frame and the corresponding observables are invoked with a certain frequency.
	/// </summary>
	public abstract class HandDetector : MonoBehaviour {

		/// <summary>
		/// The hands which palms should be observed
		/// </summary>
		public List<HandModel> HandModels = new List<HandModel>();

		/// <summary>
		/// Invoked every 1 / <see cref="CorrectInvokeFrequency"/> when the correct pose is detected.
		/// </summary>
		public IObservable<Unit> CorrectPose { get { return correctPoseSubject; } }
		private readonly ISubject<Unit> correctPoseSubject = new Subject<Unit>();

		/// <summary>
		/// Invoked every 1 / <see cref="IncorrectInvokeFrequency"/> when the incorrect pose is detected.
		/// </summary>
		public IObservable<Unit> IncorrectPose { get { return incorrectPoseSubject; } }
		private readonly ISubject<Unit> incorrectPoseSubject = new Subject<Unit>();

		/// <summary>
		/// The last time <see cref="CorrectPose"/> was invoked
		/// </summary>
		protected float LastCorrectInvokeTime;

		/// <summary>
		/// The last time <see cref="IncorrectPose"/> was invoked
		/// </summary>
		protected float LastIncorrectInvokeTime;

		/// <summary>
		/// How often per second the <see cref="CorrectPose"/> will be invoked, if so
		/// </summary>
		protected const float CorrectInvokeFrequency = 2f;

		/// <summary>
		/// How often per second the <see cref="IncorrectPose"/> will be invoked, if so
		/// </summary>
		protected const float IncorrectInvokeFrequency = 2f;

		protected virtual void Start() {
			if (HandModels.Count == 0)
				throw new MissingReferenceException("Missing hand models!");
			HandModels.ForEach(hm => hm.AssertNotNull("HandModel reference null!"));
		}

		protected virtual void Update() {
			var correct = true;
			foreach (var handModel in HandModels) {
				correct &= CheckHand(handModel);
			}

			if(correct)
				TryInvokeCorrect();
			else
				TryInvokeIncorrect();
		}

		protected abstract bool CheckHand(HandModel hand);

		/// <summary>
		/// Tries to retreive the leap hand of the given hand if the hand is not null.
		/// </summary>
		/// <param name="hand">The hand which leaphand we try to retreive</param>
		/// <returns>A reference to the leap hand or null otherwise</returns>
		protected Leap.Hand TryGetLeapHand(HandModel hand) {
			return hand == null || !hand.gameObject.activeInHierarchy ? null : hand.GetLeapHand();
		}

		/// <summary>
		/// Invokes the <see cref="correctPoseSubject"/>, if the interval of 1 / <see cref="CorrectInvokeFrequency"/> is met
		/// </summary>
		private void TryInvokeCorrect() {
			if (!(Time.time - LastCorrectInvokeTime > 1 / CorrectInvokeFrequency))
				return;
			
			correctPoseSubject.OnNext(Unit.Default);
			LastCorrectInvokeTime = Time.time;
		}

		/// <summary>
		/// Invokes the <see cref="incorrectPoseSubject"/>, if the interval of 1 / <see cref="IncorrectInvokeFrequency"/> is met
		/// </summary>
		private void TryInvokeIncorrect() {
			if (!(Time.time - LastIncorrectInvokeTime > 1 / IncorrectInvokeFrequency))
				return;

			InvokeIncorrect();
			LastIncorrectInvokeTime = Time.time;
		}

		/// <summary>
		/// Invoke <see cref="IncorrectPose"/> immediately
		/// </summary>
		protected void InvokeIncorrect() {
			incorrectPoseSubject.OnNext(Unit.Default);
		}

	}

}