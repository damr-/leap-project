using PXL.Utility;
using UniRx;

namespace PXL.Gamemodes {

	public static class GameMode {

		/// <summary>
		/// The current, observable state of the game
		/// </summary>
		public static ObservableProperty<bool> GameOver = new ObservableProperty<bool>();

		/// <summary>
		/// Invoked when the game is won
		/// </summary>
		public static IObservable<Unit> GameWon {
			get { return GameWonSubject; }
		}
		private static readonly ISubject<Unit> GameWonSubject = new Subject<Unit>();

		/// <summary>
		/// Invoked when the game is lost
		/// </summary>
		public static IObservable<Unit> GameLost {
			get { return GameLostSubject; }
		}
		private static readonly ISubject<Unit> GameLostSubject = new Subject<Unit>();

		/// <summary>
		/// If the game has multiple conditions, the game is won as soon as <see cref="CurrentPoints"/> is >= <see cref="WinPoints"/>.
		/// </summary>
		public static int WinPoints = -1;

		/// <summary>
		/// The current amount of points
		/// </summary>
		public static int CurrentPoints;

		/// <summary>
		/// Sets <see cref="GameOver"/> to true and invokes observables
		/// </summary>
		public static void SetGameWon(bool won) {
			GameOver.Value = true;
			if (won)
				GameWonSubject.OnNext(Unit.Default);
			else
				GameLostSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Sets <see cref="WinPoints"/> to the given value
		/// </summary>
		public static void SetWinPoints(int points) {
			CurrentPoints = 0;
			WinPoints = points;
		}

		/// <summary>
		/// Adds the given amount of points to <see cref="CurrentPoints"/> and 
		/// calls <see cref="SetGameWon"/> if <see cref="CurrentPoints"/> >= <see cref="WinPoints"/>
		/// </summary>
		public static void AddPoints(int points) {
			CurrentPoints += points;

			if (WinPoints >= 0 && CurrentPoints >= WinPoints) {
				SetGameWon(true);
			}
		}

	}

}