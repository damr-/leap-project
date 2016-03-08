using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using PXL.Utility;
using PXL.Objects;

namespace PXL.UI {

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

		/// <summary>
		/// All the available colors
		/// </summary>
		public ObjectColor[] availableColors = {
			new ObjectColor(Color.white, "Random"),
			new ObjectColor(Color.red, "Red"),
			new ObjectColor(Color.green, "Green"),
			new ObjectColor(Color.blue, "Blue"),
			new ObjectColor(Color.yellow, "Yellow"),
			new ObjectColor(Color.cyan, "Cyan"),
			new ObjectColor(Color.magenta , "Magenta")
		};

		
		/// <summary>
		/// Sprite for the preview image if 'Random' is selected
		/// </summary>
		public Sprite randomColorSprite;

		/// <summary>
		/// Reference to the color preview image
		/// </summary>
		public Image image;

		/// <summary>
		/// Current color used to tint objects
		/// </summary>
		public Color currentColor { get; set; }
		
		/// <summary>
		/// All the keys used for switching between colors
		/// </summary>
		protected List<KeyCode> changeColorKeys = new List<KeyCode> {
			KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
		};

		protected override void Start() {
			base.Start();

			image.AssertNotNull("The target preview Image component is missing!");
			
			objectManager.ObjectSpawned.Subscribe(SetObjectColor);
		}
		
		protected virtual void Update() {
			if (!AdminUIBase.IsAdmin)
				return;
			foreach (var item in changeColorKeys.Select((value, index) => new { index, value })) {
				CheckKey(item.value, item.index);
			}
        }
		
		/// <summary>
		/// Checks whether the given key is pressed. If yes, sets the value of the dropdown menu
		/// </summary>
		/// <param name="key">The KeyCode to check</param>
		/// <param name="index">The index of the key in the list</param>
		private void CheckKey(KeyCode key, int index) {
			if (Input.GetKeyDown(key))
				dropdown.value = index;
		}
		
		/// <summary>
		/// Sets the color of the preview image to the current one or to the random color sprite
		/// </summary>
		/// <param name="newColor">The new color for the preview image</param>
		protected virtual void UpdatePreviewColor(Color newColor) {
			image.color = newColor;
			if (newColor == Color.white) {
				image.sprite = randomColorSprite;
			}
			else {
				image.sprite = null;
			}
		}
		
		/// <summary>
		/// Sets a new color in the ObjectManager
		/// </summary>
		/// <param name="menuIndex">The new index of the dropdown menu</param>
		public void SelectionChanged(int menuIndex) {
			Color newColor = availableColors.ElementAt(menuIndex).color;

			currentColor = newColor;
			UpdatePreviewColor(newColor);
		}
		
		/// <summary>
		/// Applies the current color to the given ObjectBehaviour
		/// </summary>
		/// <param name="objectBehaviour">The target object which will get the current color assigned</param>
		protected virtual void SetObjectColor(ObjectBehaviour objectBehaviour) {
			Color objectColor = (currentColor == Color.white) ? GetRandomColor() : currentColor;
			objectBehaviour.GetComponent<Renderer>().material.color = objectColor;
		}

		/// <summary>
		/// Returns a random available color, without the first entry (white/random)
		/// </summary>
		public Color GetRandomColor() {
			return availableColors.GetRandomElement(1).color;
		}
		
		/// <summary>
		/// Adds all color entries to the dropdown menu
		/// </summary>
		protected override void AddDropdownEntries() {
			List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
			foreach (ObjectColor entry in availableColors) {
				optionsList.Add(new Dropdown.OptionData(entry.name));
			}
			dropdown.AddOptions(optionsList);
		}
	}

}