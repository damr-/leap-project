using UnityEngine;

namespace PXL.Gamemodes {

	public class SetupGameMode : MonoBehaviour {
		public int NecessaryPoints = -1;

		private void Start() {
			GameMode.GameOver.Value = false;
			GameMode.SetWinPoints(NecessaryPoints);
		}

	}

}