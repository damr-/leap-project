using PXL.Utility;

namespace PXL.Gamemodes {

	public static class GameMode {

		/// <summary>
		/// Whether the game is won
		/// </summary>
		public static ObservableProperty<bool> GameWon { get { return IsGameWon; } }
		private static readonly ObservableProperty<bool> IsGameWon = new ObservableProperty<bool>();

		public static void SetGameOver(bool won) {
			GameWon.Value = won;
		}
	}

}