using System;
using UniRx;
using PXL.Gamemodes;
using UnityEngine;

namespace PXL.Utility.Toggle {

	/// <summary>
	/// This script provides functionality to toggle behaviours as soon as the game is over.
	/// 
	/// It distinguishes between behaviours which are enabled/disabled or toggled either when the game is won or lost.
	/// </summary>
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
		private IDisposable winDisposable = Disposable.Empty;

		/// <summary>
		/// Subscription for game lose
		/// </summary>
		private IDisposable loseDisposable = Disposable.Empty;

		private void Start() {
			winDisposable = GameState.GameWon.Subscribe(_ => ToggleBehaviours(true));
			loseDisposable = GameState.GameLost.Subscribe(_ => ToggleBehaviours(false));
		}

		/// <summary>
		/// Called when the GameWon state changes
		/// </summary>
		private void ToggleBehaviours(bool won) {
			if (won) {
				foreach (var b in BehavioursToEnableOnWin)
					b.enabled = true;
				foreach (var b in BehavioursToDisableOnWin)
					b.enabled = false;
			}
			else {
				foreach (var b in BehavioursToEnableOnLose)
					b.enabled = true;

				foreach (var b in BehavioursToDisableOnLose)
					b.enabled = false;
			}
		}

		/// <summary>
		/// Called when this object is disabled
		/// </summary>
		private void OnDisable() {
			winDisposable.Dispose();
			loseDisposable.Dispose();
		}

	}

}