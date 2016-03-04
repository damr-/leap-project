using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ModeButton : MonoBehaviour {

	/**
	* The button which is used for switching mode
	*/
	public KeyCode switchModeKey = KeyCode.Y;

	private Button button;

	protected virtual void Start() {
		button = this.TryGetComponent<Button>();
	}

	protected virtual void Update() {
		if (Input.GetKeyDown(switchModeKey)) {
			button.onClick.Invoke();
			AdminUIBase.ToggleMode();
		}
	}

}