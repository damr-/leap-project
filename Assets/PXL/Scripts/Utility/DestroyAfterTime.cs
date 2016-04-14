using System;
using UniRx;
using UnityEngine;

namespace PXL.Utility {

	public class DestroyAfterTime : MonoBehaviour {
	
		/// <summary>
		/// After how many seconds this object will be despawned
		/// </summary>
		public float LifeTime = 1f;

		/// <summary>
		/// Whether this object should be despawned or destroyed
		/// </summary>
		public bool Despawn = true;

		/// <summary>
		/// Disposable for the life time subscription
		/// </summary>
		private IDisposable lifeTimeDisposable = Disposable.Empty;
		
		private void OnEnable() {
			lifeTimeDisposable = Observable.Timer(TimeSpan.FromSeconds(LifeTime)).Subscribe(_ => {
				if(Despawn)
					SimplePool.Despawn(gameObject);
				else
					Destroy(gameObject);
			});
		}

		private void OnDisable() {
			lifeTimeDisposable.Dispose();
		}

	}

}