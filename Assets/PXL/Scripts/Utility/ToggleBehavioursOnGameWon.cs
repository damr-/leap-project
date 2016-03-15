using System;
using UniRx;
using PXL.Gamemodes;
using UnityEngine;

namespace PXL.Utility {

	public class ToggleBehavioursOnGameWon : MonoBehaviour {
		/// <summary>
		/// Behaviours that will be enabled when the game is won
		/// </summary>
		public Behaviour[] BehavioursToEnable;

		/// <summary>
		/// Behaviours that will be disabled when the game is won
		/// </summary>
		public Behaviour[] BehavioursToDisable;

		/// <summary>
		/// Subscription to the GameMode
		/// </summary>
		private IDisposable subscription = Disposable.Empty;

		private void Start() {
			subscription = GameMode.GameWon.Subscribe(HandleGameWon);
		}

		/// <summary>
		/// Called when the GameWon state changes
		/// </summary>
		private void HandleGameWon(bool won) {
			if (!won)
				return;

			foreach (var b in BehavioursToEnable) {
				b.enabled = true;
			}

			foreach (var b in BehavioursToDisable) {
				b.enabled = false;
			}
		}

		/// <summary>
		/// Called when this object is disabled
		/// </summary>
		private void OnDisable() {
			subscription.Dispose();
        }

	}

}