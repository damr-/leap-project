using System;
using UnityEngine;
using UniRx;
using PXL.Utility;

namespace PXL.Objects.Areas {

	[RequireComponent(typeof(DestroyArea))]
	[RequireComponent(typeof(AudioSource))]
	public class DestroyAreaFeedback : MonoBehaviour {

		/// <summary>
		/// The <see cref="DestroyArea"/> of this object
		/// </summary>
		protected DestroyArea DestroyArea {
			get { return mDestroyArea ?? (mDestroyArea = this.TryGetComponent<DestroyArea>()); }
		}
		private DestroyArea mDestroyArea;

		/// <summary>
		/// The <see cref="AudioSource"/> of this object
		/// </summary>
		protected AudioSource AudioSource {
			get { return mAudioSource ?? (mAudioSource = this.TryGetComponent<AudioSource>()); }
		}
		private AudioSource mAudioSource;

		/// <summary>
		/// The <see cref="Light"/> component used for feedback
		/// </summary>
		public Light Light;

		/// <summary>
		/// The <see cref="Color"/> for the light when an invalid object enters
		/// </summary>
		public Color InvalidObjectColor = new Color(200 / 255f, 20 / 255f, 20 / 255f, 1f);

		/// <summary>
		/// The <see cref="Color"/> for the light when a valid object enters
		/// </summary>
		public Color ValidObjectColor = new Color(20 / 255f, 200 / 255f, 20 / 255f, 1f);

		/// <summary>
		/// The <see cref="AudioClip"/> to play when a valid object enters
		/// </summary>
		public AudioClip ValidObjectClip;

		/// <summary>
		/// The <see cref="AudioClip"/> to play when an invalid object enters
		/// </summary>
		public AudioClip InvalidObjectClip;

		/// <summary>
		/// The default color of the light
		/// </summary>
		private Color defaultColor;

		/// <summary>
		/// Subscription for resetting the light to <see cref="defaultColor"/>
		/// </summary>
		private IDisposable lightDisposable = Disposable.Empty;

		private void Start() {
			Light.AssertNotNull("Missing Light Reference on DestroyAreaFeedback!");
			defaultColor = Light.color;

			DestroyArea.ValidObject.Subscribe(HandleValidObject);
			DestroyArea.InvalidObject.Subscribe(HandleInvalidObject);
		}

		/// <summary>
		/// Called when a valid object enters the area
		/// </summary>
		private void HandleValidObject(InteractiveObject interactiveObject) {
			if(ValidObjectClip != null)
				PlayAudio(ValidObjectClip);
			SetLightColor(ValidObjectColor);
		}

		/// <summary>
		/// Called when an invalid object enters the area
		/// </summary>
		private void HandleInvalidObject(InteractiveObject interactiveObject) {
			if(InvalidObjectClip != null)
				PlayAudio(InvalidObjectClip);
			SetLightColor(InvalidObjectColor);
		}

		/// <summary>
		/// Plays the given <see cref="AudioClip"/> on this object's <see cref="AudioSource"/>
		/// </summary>
		/// <param name="audioClip"></param>
		private void PlayAudio(AudioClip audioClip) {
			AudioSource.clip = audioClip;
			AudioSource.Play();
		}

		/// <summary>
		/// Sets the light color to the specified one for <see cref="duration"/> seconds. Then it reverts to <see cref="defaultColor"/> again.
		/// </summary>
		private void SetLightColor(Color color, float duration = 1f) {
			Light.color = color;
			lightDisposable.Dispose();
			lightDisposable = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ => {
				Light.color = defaultColor;
			});
		}

	}

}