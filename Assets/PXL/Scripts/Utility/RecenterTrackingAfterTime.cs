using System;
using UniRx;
using UnityEngine;
using UnityEngine.VR;

namespace PXL.Utility {

	public class RecenterTrackingAfterTime : MonoBehaviour {
		
		/// <summary>
		/// After how much time the view is reset
		/// </summary>
		public float Delay = 2f;

		/// <summary>
		/// Disposable for the delay
		/// </summary>
		private IDisposable delayDisposable = Disposable.Empty;

		private void Start() {
			delayDisposable = Observable.Timer(TimeSpan.FromSeconds(Delay)).Subscribe(_ => {
				InputTracking.Recenter();
			});
		}

		private void OnDisable() {
			delayDisposable.Dispose();
		}

	}

}