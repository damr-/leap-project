using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Objects;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class DisplayInformation : InteractionHandSubscriber {

		/// <summary>
		/// The Text components of the time labels for left and right hand
		/// </summary>
		public Text TimeText;

		/// <summary>
		/// The Text components of the picks labels for left and right hand
		/// </summary>
		public Text[] PicksTexts;

		/// <summary>
		/// The Text components of the drops labels for left and right hand
		/// </summary>
		public Text[] DropsTexts;

		/// <summary>
		/// The Text components of the distance labels for left and right hand
		/// </summary>
		public Text[] DistanceTexts;

		private IDisposable gameWinSubscription = Disposable.Empty;
		
		/// <summary>
		/// The distance each hand has carried an object
		/// </summary>
		private readonly float[] distances = new float[2];

		/// <summary>
		/// The time the first object got picked up
		/// </summary>
		private float startTime = -1f;

		/// <summary>
		/// The timespan for displaying the time
		/// </summary>
		private TimeSpan timeSpan;

		private readonly IDictionary<GameObject, CompositeDisposable> objectSubscriptions =
			new Dictionary<GameObject, CompositeDisposable>();

		protected override void Start() {
			base.Start();

			AssertReferences();
			gameWinSubscription = GameMode.GameWon.Subscribe(HandleGameWon);
		}

		protected override void HandleGrabbed(Grabbable grabbable) {
			IncrementTextValue(grabbable, PicksTexts);
			TryStartTimer();
		}

		protected override void HandleDropped(Grabbable grabbable) {
			IncrementTextValue(grabbable, DropsTexts);
		}

		protected override void HandleMoved(MovementInfo movementInfo) {
			var side = GetHandSideIfValid(movementInfo.Moveable.Grabbable);
			if (side == HandSide.None)
				return;

			var index = (int) side - 1;
			distances[index] += movementInfo.Delta.magnitude;
			DistanceTexts[index].text = distances[index].ToString("0.000");
		}

		private void Update() {
			if (startTime < 0f)
				return;

			timeSpan = TimeSpan.FromSeconds((Time.time - startTime));
			TimeText.text = string.Format("{0:D0}m:{1:D1}s:{2:D2}ms",
				timeSpan.Minutes,
				timeSpan.Seconds,
				timeSpan.Milliseconds);
		}

		/// <summary>
		/// Makes sure every needed reference is setup and not missing
		/// </summary>
		private void AssertReferences() {
			TimeText.AssertNotNull();

			for (var i = 0; i < 2; i++) {
				PicksTexts[i].AssertNotNull();
				DropsTexts[i].AssertNotNull();
				DistanceTexts[i].AssertNotNull();
			}
		}
		
		/// <summary>
		/// Returns the text of the given Text component as an Integer, if possible
		/// </summary>
		private int GetLabelTextAsNumber(Text text) {
			var result = -1;
			if (int.TryParse(text.text, out result))
				return result;
			return -1;
		}

		/// <summary>
		/// Increments the value of the Text component's text by one, if possible
		/// </summary>
		private void IncrementTextValue(Grabbable grabbable, IList<Text> possibleTexts) {
			var side = GetHandSideIfValid(grabbable);

			if (side == HandSide.None)
				return;

			var text = possibleTexts[(int)side-1];

			var value = GetLabelTextAsNumber(text);
			if (value != -1)
				text.text = (value + 1).ToString();
		}

		/// <summary>
		/// Called when the stacking game is over
		/// </summary>
		private void HandleGameWon(bool won) {
			if (!won)
				return;

			startTime = -1;

			while (objectSubscriptions.Count > 0) {
				var first = objectSubscriptions.ElementAt(0);
				first.Value.Dispose();
				objectSubscriptions.Remove(first);
			}
		}

		/// <summary>
		/// Starts the timer if it isn't running yet
		/// </summary>
		private void TryStartTimer() {
			if (startTime < 0f)
				startTime = Time.time;
		}


		private void OnDisable() {
			gameWinSubscription.Dispose();
		}

	}
}