using System;
using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

	public class DisplayTime : MonoBehaviour {

		/// <summary>
		/// Whether the timer starts as soon as this component is activated
		/// </summary>
		public bool AutoStart;

		/// <summary>
		/// Disposable for the gamestate to detect when the game is won
		/// </summary>
		private IDisposable gameWinDisposable = Disposable.Empty;

		/// <summary>
		/// The time the first object got picked up
		/// </summary>
		private float startTime = -1f;

		/// <summary>
		/// The timespan for displaying the time
		/// </summary>
		private TimeSpan timeSpan;
		
		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		private void OnDisable() {
			gameWinDisposable.Dispose();
		}

		private void OnEnable() {
			if (AutoStart)
				TryStartTimer();
			gameWinDisposable = GameMode.GameOver.Subscribe(HandleGameOver);
		}

		private void Update() {
			if (startTime < 0f)
				return;

			timeSpan = TimeSpan.FromSeconds(Time.time - startTime);
			Text.text = string.Format("{0:D0}:{1:D1}:{2:D2}",
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
		/// Sets the timer back to zero. Doesn't stop the timer.
		/// </summary>
		public void ResetTimer() {
			startTime = Time.time;
			Text.text = "00:00:00";
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
		private void HandleGameOver(bool gameOver) {
			if (gameOver)
				StopTimer();
		}

	}

}