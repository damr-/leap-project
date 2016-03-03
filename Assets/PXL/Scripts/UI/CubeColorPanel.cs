using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class CubeColorPanel : MonoBehaviour {

	public static Dictionary<Color, string> availableColors = new Dictionary<Color, string>(){
		{ Color.white, "Random" },
		{ Color.red, "Red" },
		{ Color.green, "Green" },
		{ Color.blue, "Blue" },
		{ Color.yellow, "Yellow" },
		{ Color.cyan, "Cyan" },
		{ Color.magenta , "Magenta" }
		};

	/**
	* Reference to the desired cubeManager
	*/
	public CubeManager cubeManager;

	/**
	* Sprite for when Random is selected
	*/
	public Sprite randomColorSprite;

	/**
	* Reference to the color preview image
	*/
	public Image image;

	/**
	* The dropdown component of the child UI element
	*/
	private Dropdown dropdown;

	private void Start() {
		dropdown = GetComponentInChildren<Dropdown>();

		if (cubeManager == null) {
			Debug.LogError("No CubeManager set!");
			return;
		}

		if (image == null) {
			Debug.LogError("Target preview image is missing!");
			return;
		}

		dropdown.ClearOptions();

		AddDropdownEntries();

		cubeManager.CubeColor.Subscribe(UpdateImageColor);

		dropdown.value = 1;
	}

	private void AddDropdownEntries() {
		List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
		foreach (KeyValuePair<Color, string> entry in availableColors) {
			optionsList.Add(new Dropdown.OptionData(entry.Value));
		}
		dropdown.AddOptions(optionsList);
	}

	private void UpdateImageColor(Color cubeColor) {
		image.color = cubeColor;
		if (cubeColor == Color.white) {
			image.sprite = randomColorSprite;
		}
		else {
			image.sprite = null;			
		}
	}

}
