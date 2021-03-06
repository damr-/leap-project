﻿using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Health {

	/// <summary>
	/// Class which handles the health of an object.
	/// It sets the initial health points and keeps track of the current amount of health points.
	/// The class provides functionality to heal, hurt or immediately kill an object (reduce it's health points to zero).
	/// It does not handle the case of death, but just invokes an observable to be picked up by other scripts.
	/// </summary>
	public class Health : MonoBehaviour {

		/// <summary>
		/// The initial amount of health points this object has
		/// </summary>
		public float InitialHealth = 1f;

		/// <summary>
		/// The maximum amount of health this object can have
		/// </summary>
		public float MaxHealth = 1f;

		/// <summary>
		/// The current amount of health points this object has
		/// </summary>
		public ObservableProperty<float> CurrentHealth = new ObservableProperty<float>();

		/// <summary>
		/// Invoked when this object dies
		/// </summary>
		public IObservable<Unit> Death { get { return deathSubject; } }
		private readonly ISubject<Unit> deathSubject = new Subject<Unit>();

		/// <summary>
		/// INvoked when this the amount of health points decrease
		/// </summary>
		public IObservable<Unit> Hurt { get { return hurtSubject; } } 
		private readonly ISubject<Unit> hurtSubject = new Subject<Unit>(); 

		/// <summary>
		/// Returns whether this object has more than 0 health points
		/// </summary>
		public bool IsAlive { get { return CurrentHealth.Value > 0f; } }

		private void OnEnable() {
			CurrentHealth.Value = InitialHealth;
		}

		/// <summary>
		/// Applies the given amount of damage to <see cref="CurrentHealth"/> and handles death
		/// </summary>
		public void ApplyDamage(float damage) {
			if (!IsAlive)
				return;

			var oldHealth = CurrentHealth.Value;

			CurrentHealth.Value -= damage;

			if (CurrentHealth.Value < oldHealth)
				hurtSubject.OnNext(Unit.Default);

			if (CurrentHealth > 0)
				return;

			deathSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Sets <see cref="CurrentHealth"/> to <see cref="MaxHealth"/>
		/// </summary>
		public void Heal() {
			CurrentHealth.Value = MaxHealth;
		}

		/// <summary>
		/// Applies <see cref="CurrentHealth"/> points of damage
		/// </summary>
		public void Kill() {
			ApplyDamage(CurrentHealth.Value);
		}

	}

}