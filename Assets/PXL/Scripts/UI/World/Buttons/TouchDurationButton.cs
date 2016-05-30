using System;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Buttons {

	/// <summary>
	/// This script provides functionality for a button which has to be pressed for a certain duration before it will be activated.
	/// </summary>
	public class TouchDurationButton : TouchableButton {

		/// <summary>
		/// The progress image sprite
		/// </summary>
		public Image ProgressImage;

		/// <summary>
		/// The color for the progress image sprite when the button is being touched
		/// </summary>
		protected Color TouchedColor = new Color(140 / 255f, 140 / 255f, 140 / 255f, 128 / 255f);

		/// <summary>
		/// The color for the progress image sprite when touching the button is cancelled
		/// </summary>
		protected Color TouchCancelledColor = new Color(255 / 255f, 143 / 255f, 134 / 255f, 128 / 255f);

		/// <summary>
		/// How long the finger has to stay for the button to be clicked
		/// </summary>
		public float RequiredFingerStayDuration = 1f;

		/// <summary>
		/// When the finger entered
		/// </summary>
		protected float StartTime = -1;

		/// <summary>
		/// Disposable for the timer when the progress image flashes in <see cref="TouchCancelledColor"/> color because the finger left too early
		/// </summary>
		protected IDisposable CancelledFlashDisposable = Disposable.Empty;
		
		protected override void Start() {
			base.Start();
			ProgressImage.AssertNotNull(gameObject.name + " is missing the progress image reference!");
		}

		protected override void Update() {
			base.Update();

			if (!IsTouched())
				return;

			if (RequiredFingerStayDuration < 0.01f) {
				HandleDurationOver();
				return;
			}

			ProgressImage.fillAmount = (Time.time - StartTime).Remap(0, RequiredFingerStayDuration, 0, 1);

			if (Time.time - StartTime <= RequiredFingerStayDuration)
				return;

			HandleDurationOver();
		}

		protected override void HandlePressed() {
			base.HandlePressed();

			CancelledFlashDisposable.Dispose();
			ProgressImage.color = TouchedColor;
			StartTime = Time.time;
		}

		protected override void HandleFingerLeft(FingerInfo fingerInfo) {
			if (!IsTouchingFinger(fingerInfo))
				return;

			HandlePressCancelled();
			base.HandleFingerLeft(fingerInfo);
		}

		protected override void HandlePressCancelled() {
			base.HandlePressCancelled();

			ProgressImage.color = TouchCancelledColor;
			CancelledFlashDisposable.Dispose();
			CancelledFlashDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
				ProgressImage.fillAmount = 0;
			});
			StartTime = -1;
		}

		/// <summary>
		/// Called when the finger has touched the button for <see cref="RequiredFingerStayDuration"/>.
		/// Stops the progress animation and invokes the button onclick event.
		/// </summary>
		protected void HandleDurationOver() {
			StartTime = -1;
			ProgressImage.fillAmount = 0;
			Button.onClick.Invoke();
		}

		/// <summary>
		/// Returns whether the button is currently touched by a finger and the duration is counting
		/// </summary>
		protected bool IsTouched() {
			return Fingertip != null && StartTime >= 0;
		}

		private void OnDisable() {
			CancelledFlashDisposable.Dispose();
			ProgressImage.fillAmount = 0;
			Fingertip = null;
			StartTime = -1;
		}

	}

}