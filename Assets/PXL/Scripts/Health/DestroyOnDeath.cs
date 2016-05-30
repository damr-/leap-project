using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Health {

	/// <summary>
	/// This script despawns or destroys this object as soon as <see cref="Health"/> invokes the Death observable.
	/// </summary>
	[RequireComponent(typeof(Health))]
	public class DestroyOnDeath : MonoBehaviour {

		/// <summary>
		/// Whether this object should be despawned and not destroyed
		/// </summary>
		public bool Despawn = true;

		/// <summary>
		/// The Health component of this object
		/// </summary>
		private Health Health {
			get { return mHealth ?? (mHealth = this.TryGetComponent<Health>()); }
		}
		private Health mHealth;

		private void Start() {
			Health.Death.Subscribe(_ => HandleDeath());
		}

		/// <summary>
		/// Called when the <see cref="Health" /> component invokes the Death Observable
		/// </summary>
		private void HandleDeath() {
			if (Despawn)
				SimplePool.Despawn(gameObject);
			else
				Destroy(gameObject);
		}

	}

}