using System;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Buttons {

	public class TouchDurationButton : TouchableButton {

		/// <summary>
		/// The progress image sprite
		/// </summary>
		public Image ProgressImage;

		/// <summary>
		/// The color for the progress image sprite when the button is being touched
		/// </summary>
		public Color TouchedColor = new Color(76 / 255f, 176 / 255f, 76 / 255f, 1f);

		/// <summary>
		/// The color for the progress image sprite when touching the button is cancelled
		/// </summary>
		public Color TouchCancelledColor = new Color(176 / 255f, 76 / 255f, 76 / 255f, 1f);

		/// <summary>
		/// How long the finger has to stay for the button to be clicked
		/// </summary>
		public float RequiredFingerStayDuration = 1f;

		/// <summary>
		/// When the finger entered
		/// </summary>
		protected float StartTime;

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

			ProgressImage.fillAmount = (Time.time - StartTime).Remap(0, RequiredFingerStayDuration, 0, 1);

			if (Time.time - StartTime <= RequiredFingerStayDuration)
				return;

			HandleDurationOver();
		}

		protected override void HandleFingerPressed() {
			base.HandleFingerPressed();

			CancelledFlashDisposable.Dispose();
			ProgressImage.color = TouchedColor;
			StartTime = Time.time;
		}

		protected override void HandleFingerLeft(FingerInfo fingerInfo) {
			if (!IsTouchingFinger(fingerInfo))
				return;

			base.HandleFingerLeft(fingerInfo);

			ProgressImage.color = TouchCancelledColor;
			CancelledFlashDisposable.Dispose();
			CancelledFlashDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
				ProgressImage.fillAmount = 0;
			});
			StartTime = -1;
			Fingertip = null;
		}

		/// <summary>
		/// Called when the finger has touched the button for <see cref="RequiredFingerStayDuration"/>.
		/// Stops the progress animation and invokes the button onclick event.
		/// </summary>
		protected void HandleDurationOver() {
			StartTime = -1;
			ProgressImage.fillAmount = 0;
			Fingertip = null;
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