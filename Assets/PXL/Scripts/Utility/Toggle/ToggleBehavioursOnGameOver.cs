using System;
using UniRx;
using PXL.Gamemodes;
using UnityEngine;

namespace PXL.Utility.Toggle {

	public class ToggleBehavioursOnGameOver : MonoBehaviour {

		/// <summary>
		/// Behaviours that will be enabled when the game is won
		/// </summary>
		public Behaviour[] BehavioursToEnableOnWin;

		/// <summary>
		/// Behaviours that will be enabled when the game is lost
		/// </summary>
		public Behaviour[] BehavioursToEnableOnLose;

		/// <summary>
		/// Behaviours that will be disabled when the game is won
		/// </summary>
		public Behaviour[] BehavioursToDisableOnWin;

		/// <summary>
		/// Behaviours that will be disabled when the game is lost
		/// </summary>
		public Behaviour[] BehavioursToDisableOnLose;

		/// <summary>
		/// Subscription for game win
		/// </summary>
		private IDisposable winSubscription = Disposable.Empty;

		/// <summary>
		/// Subscription for game lose
		/// </summary>
		private IDisposable loseSubscription = Disposable.Empty;

		private void Start() {
			winSubscription = GameMode.GameWon.Subscribe(_ => ToggleBehaviours(true));
			loseSubscription = GameMode.GameLost.Subscribe(_ => ToggleBehaviours(false));
		}

		/// <summary>
		/// Called when the GameWon state changes
		/// </summary>
		private void ToggleBehaviours(bool won) {

			foreach (var b in BehavioursToEnableOnWin) {
				b.enabled = won;
			}

			foreach (var b in BehavioursToDisableOnWin) {
				b.enabled = !won;
			}

			foreach (var b in BehavioursToEnableOnLose) {
				b.enabled = !won;
			}

			foreach (var b in BehavioursToDisableOnLose) {
				b.enabled = won;
			}

		}

		/// <summary>
		/// Called when this object is disabled
		/// </summary>
		private void OnDisable() {
			winSubscription.Dispose();
			loseSubscription.Dispose();
		}

	}

}