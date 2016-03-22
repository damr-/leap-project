using PXL.Utility;
using UniRx;

namespace PXL.Gamemodes {

	public static class GameMode {

		public static ObservableProperty<bool> GameOver = new ObservableProperty<bool>();

		/// <summary>
		/// Invoked when the game is won
		/// </summary>
		public static IObservable<Unit> GameWon {
			get { return _gameWonSubject; }
		}

		private static readonly ISubject<Unit> _gameWonSubject = new Subject<Unit>();

		/// <summary>
		/// Invoked when the game is lost
		/// </summary>
		public static IObservable<Unit> GameLost {
			get { return _gameLostSubject; }
		}

		private static readonly ISubject<Unit> _gameLostSubject = new Subject<Unit>();

		/// <summary>
		/// If the game has multiple conditions, the game is won as soon as <see cref="CurrentPoints"/> is >= <see cref="WinPoints"/>.
		/// </summary>
		public static int WinPoints = -1;

		/// <summary>
		/// The current amount of points
		/// </summary>
		public static int CurrentPoints = 0;

		/// <summary>
		/// Sets <see cref="GameOver"/> to true and invokes observables
		/// </summary>
		public static void SetGameWon(bool won) {
			GameOver.Value = true;
			if (won)
				_gameWonSubject.OnNext(Unit.Default);
			else
				_gameLostSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Sets <see cref="WinPoints"/> to the given value
		/// </summary>
		public static void SetWinPoints(int points) {
			WinPoints = points;
		}

		/// <summary>
		/// Adds the given amount of points to <see cref="CurrentPoints"/> and 
		/// calls <see cref="SetGameOver"/> if <see cref="CurrentPoints"/> >= <see cref="WinPoints"/>
		/// </summary>
		public static void AddPoints(int points) {
			CurrentPoints += points;

			if (CurrentPoints >= WinPoints)
				GameOver.Value = true;
		}
	}

}