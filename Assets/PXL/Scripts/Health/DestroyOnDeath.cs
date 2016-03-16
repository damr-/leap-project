﻿using UniRx;
using PXL.Utility;
using UnityEngine;

namespace PXL.Health {

	[RequireComponent(typeof(Health))]
	public class DestroyOnDeath : MonoBehaviour {

		/// <summary>
		/// Whether this object should be despawned and not destroyed
		/// </summary>
		public bool Despawn = true;

		private Health Health {
			get { return mHealth ?? (mHealth = this.TryGetComponent<Health>()); }
		}

		private Health mHealth;

		private void Start() {
			Health.Death.Subscribe(_ => HandleDeath());
		}

		/// <summary>
		/// Called when the <see cref="Health"/> component invokes the Death Observable
		/// </summary>
		private void HandleDeath() {
			if (Despawn) {
				SimplePool.Despawn(gameObject);
			}
			else {
				Destroy(gameObject);
			}
		}
	}

}