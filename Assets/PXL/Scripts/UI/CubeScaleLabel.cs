using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class CubeScaleLabel : MonoBehaviour {

	/**
	* Reference to the desired cubeManager
	*/
	public CubeManager cubeManager;

	/**
	* How much the number will be scaled for more natural results
	*/
	public float scaleFactor = 100f;

	/**
	* The text component of the UI element
	*/
	private Text text;

	private void Start() {
		text = GetComponent<Text>();

		if (cubeManager == null) {
			Debug.LogError("No CubeManager set!");
			return;
		}

		cubeManager.CubeScale.Subscribe(SetText);
    }

	private void SetText(float cubeScale) {
		text.text = (cubeScale * scaleFactor).ToString("0.0");
	}
}
