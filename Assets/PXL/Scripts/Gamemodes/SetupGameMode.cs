using UnityEngine;

namespace PXL.Gamemodes {

	/// <summary>
	/// This class should be included in every scene and provides a possibility 
	/// for the developer to set the amount of points which are required to win.
	/// </summary>
	public class SetupGameMode : MonoBehaviour {

		/// <summary>
		/// The amount of points required to win
		/// </summary>
		public int NecessaryPoints = -1;

		private void Start() {
			GameState.GameOver.Value = false;
			GameState.SetWinPoints(NecessaryPoints);
		}

	}

}