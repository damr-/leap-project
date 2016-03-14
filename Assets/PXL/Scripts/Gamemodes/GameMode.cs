using PXL.Utility;

namespace PXL.Gamemodes {

	public static class GameMode {

		/// <summary>
		/// Whether the game is won
		/// </summary>
		public static ObservableProperty<bool> GameWon = new ObservableProperty<bool>();

		public static void SetGameOver(bool won) {
			GameWon.Value = won;
		}
	}

}