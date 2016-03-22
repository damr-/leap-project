using System;
using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(Text))]
	public class DisplayTime : MonoBehaviour {

		/// <summary>
		/// Whether the timer starts as soon as this component is activated
		/// </summary>
		public bool AutoStart;

		/// <summary>
		/// The Text components of the time label
		/// </summary>
		private Text TimeText {
			get { return mTimeText ?? (mTimeText = this.TryGetComponent<Text>()); }
		}

		private Text mTimeText;

		/// <summary>
		/// The time the first object got picked up
		/// </summary>
		private float startTime = -1f;

		/// <summary>
		/// The timespan for displaying the time
		/// </summary>
		private TimeSpan timeSpan;

		private IDisposable gameWinSubscription = Disposable.Empty;

		private void OnDisable() {
			gameWinSubscription.Dispose();
		}

		private void OnEnable() {
			if (AutoStart)
				TryStartTimer();
			gameWinSubscription = GameMode.GameOver.Subscribe(_ => HandleGameOver());
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
		/// Starts the timer if it isn't running yet
		/// </summary>
		public void TryStartTimer() {
			if (startTime < 0f)
				startTime = Time.time;
		}

		/// <summary>
		/// Stops the timer
		/// </summary>
		public void StopTimer() {
			startTime = -1;
		}


		/// <summary>
		/// Called when the game is over
		/// </summary>
		private void HandleGameOver() {
			StopTimer();
		}

	}

}