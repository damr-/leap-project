using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour {
	/**
	* How long to wait between updates
	*/
	public float updateDelay = 0.5f;

	/**
	* The text component of the child
	*/
	private Text text;

	/**
	* The image component of this panel
	*/
	private Image image;

	/**
	* If the FPS are currently visible
	*/
	private bool CounterEnabled { get { return image.enabled && text.enabled; } }

	private float framesSum = 0;
	private int counter = 0;
	private float lastTime = 0;

	private void Start() {
		text = GetComponentInChildren<Text>();
		image = this.TryGetComponent<Image>();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.F)) {
			text.enabled = !text.enabled;
			image.enabled = !image.enabled;
		}

		if (!CounterEnabled)
			return;

		framesSum += 1f / Time.deltaTime;
		counter++;

		if (lastTime + updateDelay < Time.time) {
			text.text = (int)(framesSum / counter) + " fps";
			counter = 0;
			framesSum = 0;
			lastTime = Time.time;
		}
	}
}
