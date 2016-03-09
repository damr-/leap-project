using UnityEngine;
using UnityEngine.SceneManagement;

namespace PXL.Utility {

	public class LevelInput : MonoBehaviour {
	
		/// <summary>
		/// The key used for quitting the application
		/// </summary>
		public KeyCode QuitKey = KeyCode.Escape;

		/// <summary>
		/// The key used for restarting the level
		/// </summary>
		public KeyCode RestartKey = KeyCode.Space;
		
		/// <summary>
		/// Within how many seconds the second tap has to follow
		/// </summary>
		public float MaxDoubleTapDelay = 1f;

		/// <summary>
		/// Last time the escape key was pressed
		/// </summary>
		private float lastTime;

		private bool pressedOnce;

		private void Update() {
			if (Input.GetKeyDown(RestartKey)) {
				var scene = SceneManager.GetActiveScene();
				SceneManager.LoadScene(scene.name);
			}

			if (Input.GetKeyDown(QuitKey)) {
				if (!pressedOnce) {
					lastTime = Time.time;
					pressedOnce = true;
				}
				else {
					Application.Quit();
				}
			}

			if (pressedOnce && Time.time - lastTime > MaxDoubleTapDelay)
				pressedOnce = false;
		}
	}

}