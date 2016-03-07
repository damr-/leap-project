using UnityEngine;

namespace PXL.Utility {

	public class QuitInput : MonoBehaviour {
	
		/// <summary>
		/// The key used for quitting the application
		/// </summary>
		public KeyCode quitKey = KeyCode.Escape;
		
		/// <summary>
		/// Within how many seconds the second tap has to follow
		/// </summary>
		public float maxDoubleTapDelay = 1f;

		private float lastTime;

		private bool pressedOnce = false;

		private void Update() {
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