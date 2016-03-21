using PXL.Utility;

namespace PXL.Gamemodes {

	public static class GameMode {

		/// <summary>
		/// Whether the game is won
		/// </summary>
		public static ObservableProperty<bool> GameWon = new ObservableProperty<bool>();

		/// <summary>
		/// If the game has multiple conditions, the game is won as soon as <see cref="CurrentPoints"/> is >= <see cref="WinPoints"/>.
		/// </summary>
		public static int WinPoints = -1;

		/// <summary>
		/// The current amount of points
		/// </summary>
		public static int CurrentPoints = 0;

		public static void SetGameOver(bool won) {
			GameWon.Value = won;
		}

		public static void SetWinPoints(int points) {
			WinPoints = points;
		}

		public static void AddPoints(int points) {
			CurrentPoints += points;

			if(CurrentPoints >= WinPoints)
				SetGameOver(true);
		}
	}

}