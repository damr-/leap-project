using System;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Buttons {

	[RequireComponent(typeof(Collider))]
	public class LookatDurationButton : MonoBehaviour {

		/// <summary>
		/// The Image component of this object
		/// </summary>
		public Image Image {
			get { return mImage ?? (mImage = GetComponentInChildren<Image>()); }
		}
		private Image mImage;

		/// <summary>
		/// The Button component of this object
		/// </summary>
		public Button Button {
			get { return mButton ?? (mButton = GetComponentInChildren<Button>()); }
		}
		private Button mButton;

		/// <summary>
		/// The progress image sprite
		/// </summary>
		public Image ProgressImage;

		/// <summary>
		/// The target which we will cast a ray forwards from
		/// </summary>
		public Transform Target;

		/// <summary>
		/// The color for the progress image sprite when the the activation process is started
		/// </summary>
		private readonly Color touchedColor = new Color(143 / 255f, 240 / 255f, 134 / 255f, 128 / 255f);

		/// <summary>
		/// The color for the progress image sprite when the activation process is cancelled
		/// </summary>
		private readonly Color touchCancelledColor = new Color(255 / 255f, 143 / 255f, 134 / 255f, 128 / 255f);

		/// <summary>
		/// How long it takes to activate the button
		/// </summary>
		public float RequiredActivateDuration = 1f;

		/// <summary>
		/// When the activation process started
		/// </summary>
		private float startTime = -1;

		/// <summary>
		/// Disposable for the timer when the progress image flashes in <see cref="touchCancelledColor"/> color because the finger left too early
		/// </summary>
		protected IDisposable CancelledFlashDisposable = Disposable.Empty;

		/// <summary>
		/// DIsposable for the timer which raycasts
		/// </summary>
		private IDisposable updateDisposable = Disposable.Empty;

		/// <summary>
		/// How many times per second a ray will be cast
		/// </summary>
		private const float RaycastFrequency = 5f;

		/// <summary>
		/// Whether this button is currently being activated
		/// </summary>
		private bool isBeingActivated;

		/// <summary>
		/// Wether the ray should be drawn as debug ray
		/// </summary>
		public bool DrawDebugRay;
		
		/// <summary>
		/// The layermask of this gameobject, so that others are ignored
		/// </summary>
		private LayerMask layerMask;

		private void Start() {
			ProgressImage.AssertNotNull(gameObject.name + " is missing the progress image reference!");
			Target.AssertNotNull("Missing target for lookat button");

			layerMask = gameObject.layer;

			updateDisposable = Observable.Interval(TimeSpan.FromSeconds(1 / RaycastFrequency)).Subscribe(_ => {
				var ray = new Ray(Target.position, Target.forward);
				RaycastHit hit;

				if(DrawDebugRay)
					Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red, 1 / RaycastFrequency);

				if (Physics.Raycast(ray, out hit, 5f, layerMask)) {
					if (hit.collider.gameObject == gameObject)
						TryStartActivation();
					else
						TryStopActivation();
				}
				else
					TryStopActivation();
			});
		}

		private void Update() {
			if (!isBeingActivated || startTime < 0f)
				return;

			if (RequiredActivateDuration < 0.01f) {
				HandleButtonActivated();
				return;
			}

			ProgressImage.fillAmount = (Time.time - startTime).Remap(0, RequiredActivateDuration, 0, 1);

			if (Time.time - startTime <= RequiredActivateDuration)
				return;

			HandleButtonActivated();
		}

		private void TryStartActivation() {
			if (isBeingActivated)
				return;

			isBeingActivated = true;
			CancelledFlashDisposable.Dispose();
			ProgressImage.color = touchedColor;
			startTime = Time.time;
		}

		private void TryStopActivation() {
			if (!isBeingActivated)
				return;

			isBeingActivated = false;
			ProgressImage.color = touchCancelledColor;
			CancelledFlashDisposable.Dispose();
			CancelledFlashDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
				ProgressImage.fillAmount = 0;
			});
			startTime = -1;
		}

		private void HandleButtonActivated() {
			startTime = -1;
			ProgressImage.fillAmount = 0;
			Button.onClick.Invoke();
		}

		private void OnDisable() {
			updateDisposable.Dispose();
			CancelledFlashDisposable.Dispose();
			ProgressImage.fillAmount = 0;
			startTime = -1;
		}

	}

}