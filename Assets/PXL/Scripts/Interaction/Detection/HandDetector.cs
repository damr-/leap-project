﻿using System.Collections.Generic;
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

		public IObservable<Unit> CorrectPose { get { return correctPoseSubject; } }
		private readonly ISubject<Unit> correctPoseSubject = new Subject<Unit>();

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
			foreach (var handModel in HandModels) {
				CheckHand(handModel);
			}
		}

		protected abstract void CheckHand(HandModel hand);

		protected Leap.Hand TryGetLeapHand(HandModel hand) {
			return !hand.gameObject.activeInHierarchy ? null : hand.GetLeapHand();
		}

		protected void TryInvokeCorrect() {
			if (!(Time.time - LastCorrectInvokeTime > 1 / CorrectInvokeFrequency))
				return;

			correctPoseSubject.OnNext(Unit.Default);
			LastCorrectInvokeTime = Time.time;
		}

		protected void TryInvokeIncorrect() {
			if (!(Time.time - LastIncorrectInvokeTime > 1 / IncorrectInvokeFrequency))
				return;

			InvokeIncorrect();
			LastIncorrectInvokeTime = Time.time;
		}

		protected void InvokeIncorrect() {
			incorrectPoseSubject.OnNext(Unit.Default);
		}

	}

}