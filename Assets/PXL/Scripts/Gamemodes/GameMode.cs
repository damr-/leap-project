using UnityEngine;
using PXL.Utility;

namespace PXL.Gamemodes {

	public class GameMode : MonoBehaviour {

		/// <summary>
		/// Whether the game is won
		/// </summary>
		public ObservableProperty<bool> GameWon { get { return isGameWon; } }
		private readonly ObservableProperty<bool> isGameWon = new ObservableProperty<bool>();

		public void GameOver(bool win) {
			GameWon.Value = win;
		}
	}

}