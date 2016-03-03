using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class ObjectColorPanel : AdminDropdownUI {

	[Serializable]
	public struct ObjectColor {
		public Color color;
		public string name;

		public ObjectColor(Color c, string n) {
			color = c;
			name = n;
		}
	}

	public ObjectColor[] availableColors = {
		new ObjectColor(Color.white, "Random"),
		new ObjectColor(Color.red, "Red"),
		new ObjectColor(Color.green, "Green"),
		new ObjectColor(Color.blue, "Blue"),
		new ObjectColor(Color.yellow, "Yellow"),
		new ObjectColor(Color.cyan, "Cyan"),
		new ObjectColor(Color.magenta , "Magenta")
	};

	/**
	* Sprite for when Random is selected
	*/
	public Sprite randomColorSprite;

	/**
	* Reference to the color preview image
	*/
	public Image image;

	public Color currentColor { get; set; }

	protected override void Start() {
		base.Start();

		if (image == null) {
			Debug.LogError("Target preview image is missing!");
			return;
		}

		objectManager.ObjectSpawned.Subscribe(SetObjectColor);

		dropdown.value = 1;
	}

	protected virtual void Update() {
		if (Input.GetKeyDown(KeyCode.Q))
			dropdown.value = 0;
		if (Input.GetKeyDown(KeyCode.W))
			dropdown.value = 1;
		if (Input.GetKeyDown(KeyCode.E))
			dropdown.value = 2;
		if (Input.GetKeyDown(KeyCode.R))
			dropdown.value = 3;
		if (Input.GetKeyDown(KeyCode.T))
			dropdown.value = 4;
		if (Input.GetKeyDown(KeyCode.Z))
			dropdown.value = 5;
		if (Input.GetKeyDown(KeyCode.U))
			dropdown.value = 6;
		if (Input.GetKeyDown(KeyCode.I))
			dropdown.value = 7;
		if (Input.GetKeyDown(KeyCode.O))
			dropdown.value = 8;
	}

	protected virtual void UpdatePreviewColor(Color newColor) {
		image.color = newColor;
		if (newColor == Color.white) {
			image.sprite = randomColorSprite;
		}
		else {
			image.sprite = null;
		}
	}

	/**
	* Sets a new color in the ObjectManager
	*/
	public void SelectionChanged(int menuIndex) {
		Color newColor = availableColors.ElementAt(menuIndex).color;

		currentColor = newColor;
		UpdatePreviewColor(newColor);
	}

	/**
	* Returns a random available color, without the first entry (random)
	*/
	public Color GetRandomColor() {
		return GetRandomEntry(availableColors).color;
	}

	/**
	* Applies the current color to the given ObjectBehaviour
	*/
	protected virtual void SetObjectColor(ObjectBehaviour objectBehaviour) {
		Color objectColor = currentColor;
		if (currentColor == Color.white)
			objectColor = GetRandomColor();
		objectBehaviour.GetComponent<Renderer>().material.color = objectColor;
	}

	/**
	* Adds all color entries
	*/
	protected override void AddDropdownEntries() {
		List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
		foreach (ObjectColor entry in availableColors) {
			optionsList.Add(new Dropdown.OptionData(entry.name));
		}
		dropdown.AddOptions(optionsList);
	}
}
