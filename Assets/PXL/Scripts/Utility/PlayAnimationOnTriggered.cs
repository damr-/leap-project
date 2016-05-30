using System;
using UniRx;
using UnityEngine;

namespace PXL.Utility {

	/// <summary>
	/// This script plays a referenced animation as soon as the trigger has been activated 
	/// and the object stayed inside for a certain <see cref="RequiredStayDuration"/>
	/// </summary>
	public class PlayAnimationOnTriggered : MonoBehaviour {

		/// <summary>
		/// The target animation which will be played
		/// </summary>
		public Animation Animation;

		/// <summary>
		/// How long the object has to stay for the animation to be played
		/// </summary>
		public float RequiredStayDuration = 2f;

		/// <summary>
		/// Disposable for the <see cref="RequiredStayDuration"/>
		/// </summary>
		private IDisposable stayDisposable = Disposable.Empty;

		private void Start() {
			Animation.AssertNotNull("Missing animator");
		}

		private void OnTriggerEnter() {
			stayDisposable = Observable.Timer(TimeSpan.FromSeconds(RequiredStayDuration)).Subscribe(_ => {
				Animation.Play();
			});
		}

		private void OnTriggerExit() {
			stayDisposable.Dispose();
		}

	}

}