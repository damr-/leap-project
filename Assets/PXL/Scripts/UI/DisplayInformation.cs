using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PXL.Interaction;
using PXL.Objects;
using PXL.Objects.Areas;
using PXL.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class DisplayInformation : MonoBehaviour {

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

		/// <summary>
		/// The observed ObjectManagers
		/// </summary>
		public ObjectManager[] ObjectManagers;

		/// <summary>
		/// The stack area of this level
		/// </summary>
		public StackArea StackArea;

		/// <summary>
		/// The canvas which will display the game over message
		/// </summary>
		public Canvas MessageCanvas;

		/// <summary>
		/// The LevelInput Component
		/// </summary>
		public LevelInput LevelInput;

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

		private void Start() {
			TimeText.AssertNotNull();

			for (var i = 0; i < 2; i++) {
				PicksTexts[i].AssertNotNull();
				DropsTexts[i].AssertNotNull();
				DistanceTexts[i].AssertNotNull();
			}

			foreach (var objectManager in ObjectManagers) {
				objectManager.AssertNotNull();
				objectManager.ObjectSpawned.Subscribe(ObjectSpawned);
			}

			StackArea.AssertNotNull();
			StackArea.GameWon.Subscribe(HandleGameWon);

			LevelInput.AssertNotNull();
			MessageCanvas.AssertNotNull();
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
		/// Called when an object is spawned. Sets up all needed subscriptions
		/// </summary>
		/// <param name="objectBehaviour"></param>
		private void ObjectSpawned(ObjectBehaviour objectBehaviour) {
			var grabbable = objectBehaviour.GetComponent<Grabbable>();

			if (grabbable == null)
				return;

			var c = new CompositeDisposable();

			grabbable.IsGrabbed.Where(grabbed => grabbed).Subscribe(_ => {
				IncrementText(grabbable, PicksTexts);
				TryStartTimer();
			}).AddTo(c);
			grabbable.Dropped.Subscribe(_ => IncrementText(grabbable, DropsTexts)).AddTo(c);
			grabbable.MovedWhileGrabbed.Subscribe(movementInfo => ObjectMoved(grabbable, movementInfo)).AddTo(c);

			grabbable.GetComponent<ObjectBehaviour>().ObjectDestroyed.Subscribe(_ => {
				if (grabbable.gameObject == null || !objectSubscriptions.ContainsKey(grabbable.gameObject))
					return;
				objectSubscriptions[grabbable.gameObject].Dispose();
				objectSubscriptions.Remove(grabbable.gameObject);
			});

			objectSubscriptions.Add(grabbable.gameObject, c);
		}

		/// <summary>
		/// Called when an object, grabbed by the correct hand, is moved
		/// </summary>
		private void ObjectMoved(Grabbable grabbable, MovementInfo movementInfo) {
			var index = GetHandIndexIfValid(grabbable);
			distances[index] += movementInfo.Delta;
			DistanceTexts[index].text = distances[index].ToString("0.000");
		}

		/// <summary>
		/// Returns the index for accessing the arrays for the correct hand, -1 if the hand is invalid
		/// </summary>
		private int GetHandIndexIfValid(Grabbable grabbable) {
			var hand = grabbable.CurrentHand;
			if (!hand.IsHandValid())
				return -1;
			return hand.GetLeapHand().IsLeft ? 0 : 1;
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
		private void IncrementText(Grabbable grabbable, IList<Text> possibleTexts) {
			var index = GetHandIndexIfValid(grabbable);
			if (index == -1)
				return;
			var text = possibleTexts[index];
			var value = GetLabelTextAsNumber(text);
			if (value != -1)
				text.text = (value + 1).ToString();
		}

		/// <summary>
		/// Called when the stacking game is over
		/// </summary>
		private void HandleGameWon(bool won) {
			startTime = -1;
			
			MessageCanvas.gameObject.SetActive(true);
			MessageCanvas.GetComponentInChildren<Text>().text = "Well done! Press " + LevelInput.RestartKey.ToString() + " to restart!";

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

	}
}