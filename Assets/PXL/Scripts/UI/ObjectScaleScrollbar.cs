using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectScaleScrollbar : AdminUIBase {

	public KeyCode increaseKey = KeyCode.B;
	public KeyCode decreaseKey = KeyCode.V;

	public float changeAmount = 0.1f;

	protected Scrollbar scrollbar;

	protected override void Start() {
		base.Start();
		scrollbar = GetComponent<Scrollbar>();
	}

	protected virtual void Update() {
		if (Input.GetKeyDown(increaseKey))
			MoveSlider(changeAmount);
		if (Input.GetKeyDown(decreaseKey))
			MoveSlider(-changeAmount);
	}

	protected virtual void MoveSlider(float amount) {
		objectManager.SetObjectScale(scrollbar.value - changeAmount);
	}
}
