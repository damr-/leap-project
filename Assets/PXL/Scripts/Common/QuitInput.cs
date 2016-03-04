using UnityEngine;

public class QuitInput : MonoBehaviour {

	public KeyCode quitKey = KeyCode.Escape;

	public float maxDoubleTapDelay = 1f;

	private float lastTime;

	private bool pressedOnce = false;

	private void Update() {
		if(Input.GetKeyDown(quitKey)) {
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
