using UniRx;
using UnityEngine.UI;

public class ObjectScaleLabel : AdminUIBase {

	/**
	* How much the number will be scaled for more natural results
	*/
	public float scaleFactor = 100f;

	/**
	* The text component of the UI element
	*/
	private Text text;

	protected override void Start() {
		base.Start();

		text = GetComponent<Text>();
		objectManager.ObjectScale.Subscribe(SetText);
    }

	private void SetText(float cubeScale) {
		text.text = (cubeScale * scaleFactor).ToString("0.0");
	}
}
