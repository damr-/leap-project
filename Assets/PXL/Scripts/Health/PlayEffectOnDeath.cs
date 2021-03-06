﻿using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Health {

	/// <summary>
	/// This scripts spawns the given GameObject as soon as the Health's Death observable is invoked.
	/// The spawned GameObject should have a ParticleEmitter or similar attached which plays the effect automatically.
	/// </summary>
	[RequireComponent(typeof(Health))]
	public class PlayEffectOnDeath : MonoBehaviour {

		/// <summary>
		/// The GameObject to spawn when the object is destroyed
		/// </summary>
		public GameObject DestroyEffectObject;

		/// <summary>
		/// The <see cref="Health" /> Component of this object
		/// </summary>
		private Health Health {
			get { return mHealth ?? (mHealth = this.TryGetComponent<Health>()); }
		}
		private Health mHealth;

		private void Start() {
			DestroyEffectObject.AssertNotNull();
			Health.Death.Subscribe(_ => HandleDeath());
		}

		/// <summary>
		/// Called when the <see cref="Health" /> component invokes the Death Observable
		/// </summary>
		private void HandleDeath() {
			var particleSystemObject = SimplePool.Spawn(DestroyEffectObject, transform.position, Quaternion.identity);
			var particleComponent = particleSystemObject.GetComponent<ParticleSystem>();
			particleComponent.Play();
		}

	}

}