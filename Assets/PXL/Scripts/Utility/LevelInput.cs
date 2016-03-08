using UnityEngine;
using UnityEngine.SceneManagement;

namespace PXL.Utility {

	public class LevelInput : MonoBehaviour {
	
		/// <summary>
		/// The key used for quitting the application
		/// </summary>
		public KeyCode quitKey = KeyCode.Escape;

		/// <summary>
		/// The key used for restarting the level
		/// </summary>
		public KeyCode restartKey = KeyCode.Space;
		
		/// <summary>
		/// Within how many seconds the second tap has to follow
		/// </summary>
		public float maxDoubleTapDelay = 1f;

		/// <summary>
		/// Last time the escape key was pressed
		/// </summary>
		private float lastTime;

		private bool pressedOnce = false;

		private void Update() {
			if (Input.GetKeyDown(restartKey)) {
				Scene scene = SceneManager.GetActiveScene();
				SceneManager.LoadScene(scene.name);
			}

			if (Input.GetKeyDown(quitKey)) {
				if (!pressedOnce) {
					lastTime = Time.time;
					pressedOnce = true;
				}
				else {
					Application.Quit();
				}
			}

			if (pressedOnce && Time.time - lastTime > maxDoubleTapDelay)
				pressedOnce = false;
		}
	}

}