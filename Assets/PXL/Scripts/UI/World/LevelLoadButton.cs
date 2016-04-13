using System;
using Leap;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Image = UnityEngine.UI.Image;

namespace PXL.UI.World {

	[RequireComponent(typeof(Touchable))]
	public class LevelLoadButton : MonoBehaviour {

		/// <summary>
		/// The progress image sprite
		/// </summary>
		public Image Image;

		private readonly Color green = new Color(76 / 255f, 176 / 255f, 76 / 255f, 1f);

		private readonly Color red = new Color(176 / 255f, 76 / 255f, 76 / 255f, 1f);

		/// <summary>
		/// The Text component of this object's child
		/// </summary>
		public Text ButtonText {
			get { return mButtonText ?? (mButtonText = GetComponentInChildren<Text>()); }
		}
		private Text mButtonText;

		/// <summary>
		/// The Button component of this object
		/// </summary>
		public Button Button {
			get { return mButton ?? (mButton = GetComponentInChildren<Button>()); }
		}
		private Button mButton;

		/// <summary>
		/// The index of the level which will be loaded when this button is pressed
		/// </summary>
		public int LevelIndex { get; set; }

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}
		private Touchable mTouchable;

		/// <summary>
		/// How long the finger has to stay for the level to be loaded
		/// </summary>
		private const float RequiredFingerStayDuration = 1f;

		/// <summary>
		/// Whether there is a finger currently touching this level load button
		/// </summary>
		private Fingertip fingertip;

		/// <summary>
		/// When the finger entered
		/// </summary>
		private float startTime;

		/// <summary>
		/// Disposable for the timer when the progress image flashes red because the finger left too early
		/// </summary>
		private IDisposable flashRedDisposable = Disposable.Empty;

		private void Start() {
			Image.AssertNotNull("LevelButton is missing the progress image reference!");
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
		}

		private void Update() {
			if (fingertip == null || startTime < 0)
				return;

			Image.fillAmount = (Time.time - startTime).Remap(0, RequiredFingerStayDuration, 0, 1);

			if (Time.time - startTime <= RequiredFingerStayDuration)
				return;

			startTime = -1;
			fingertip = null;
			Button.onClick.Invoke();
		}

		private void HandleFingerEntered(FingerInfo fi) {
			if (fingertip != null)
				return;

			if (!Touchable.IsCertainFingerTouching(fi.HandModel, Finger.FingerType.TYPE_INDEX))
				return;

			flashRedDisposable.Dispose();
			Image.color = green;

			fingertip = fi.Fingertip;
			startTime = Time.time;
		}

		private void HandleFingerLeft(FingerInfo fingerInfo) {
			if (fingerInfo.Fingertip != fingertip)
				return;

			Image.color = red;
			flashRedDisposable.Dispose();
			flashRedDisposable = Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => {
				Image.fillAmount = 0;
			});
			startTime = -1;
			fingertip = null;
		}

		private void OnDisable() {
			flashRedDisposable.Dispose();
		}

	}

}