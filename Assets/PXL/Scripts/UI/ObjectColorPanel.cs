using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using PXL.Utility;
using PXL.Objects;

namespace PXL.UI {

	public class ObjectColorPanel : AdminDropdownUi {

		[Serializable]
		public struct ObjectColor {
			public Color Color;
			public string Name;

			public ObjectColor(Color c, string n) {
				Color = c;
				Name = n;
			}
		}

		/// <summary>
		/// All the available colors
		/// </summary>
		public ObjectColor[] AvailableColors = {
			new ObjectColor(Color.white, "Random"),
			new ObjectColor(Color.red, "Red"),
			new ObjectColor(Color.green, "Green"),
			new ObjectColor(Color.blue, "Blue"),
			new ObjectColor(Color.yellow, "Yellow"),
			new ObjectColor(Color.cyan, "Cyan"),
			new ObjectColor(Color.magenta , "Magenta")
		};

		/// <summary>
		/// The text component of the color label
		/// </summary>
		public Text LabelText;
		
		/// <summary>
		/// Sprite for the preview image if 'Random' is selected
		/// </summary>
		public Sprite RandomColorSprite;

		/// <summary>
		/// Reference to the color preview image
		/// </summary>
		public Image ColorPreview;

		/// <summary>
		/// Current color used to tint objects
		/// </summary>
		public Color CurrentColor { get; set; }
		
		/// <summary>
		/// All available keys that could be used for switching colors
		/// </summary>
		protected List<KeyCode> AvailableColorKeys = new List<KeyCode> {
			KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
		};

		/// <summary>
		/// The keys that can be used to change color
		/// </summary>
		protected List<KeyCode> ChangeColorKeys = new List<KeyCode>();

		protected override void Start() {
			base.Start();

			ColorPreview.AssertNotNull("The target preview Image component is missing!");
			LabelText.AssertNotNull("The Text component of the color label is missing");

			if (AvailableColors.Length > AvailableColorKeys.Count)
				throw new IndexOutOfRangeException("There are too many colors and too few keys specified!");

			for (var i = 0; i < AvailableColors.Length; i++) {
				ChangeColorKeys.Add(AvailableColorKeys.ElementAt(i));
			}

			LabelText.text += ChangeColorKeys[0] + "-" + ChangeColorKeys.ElementAt(AvailableColors.Length - 1).ToString() + ")";

			ObjectManager.ForEach(o => o.ObjectSpawned.Subscribe(SetObjectColor));
		}
		
		protected virtual void Update() {
			if (!IsAdmin)
				return;
			foreach (var item in ChangeColorKeys.Select((value, index) => new { index, value })) {
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
				Dropdown.value = index;
		}
		
		/// <summary>
		/// Sets the color of the preview image to the current one or to the random color sprite
		/// </summary>
		/// <param name="newColor">The new color for the preview image</param>
		protected virtual void UpdatePreviewColor(Color newColor)
		{
			ColorPreview.color = newColor;
			ColorPreview.sprite = newColor == Color.white ? RandomColorSprite : null;
		}

		/// <summary>
		/// Sets a new color in the ObjectManager
		/// </summary>
		/// <param name="menuIndex">The new index of the dropdown menu</param>
		public void SelectionChanged(int menuIndex) {
			var newColor = AvailableColors.ElementAt(menuIndex).Color;

			CurrentColor = newColor;
			UpdatePreviewColor(newColor);
		}
		
		/// <summary>
		/// Applies the current color to the given ObjectBehaviour
		/// </summary>
		/// <param name="objectBehaviour">The target object which will get the current color assigned</param>
		protected virtual void SetObjectColor(ObjectBehaviour objectBehaviour) {
			var objectColor = CurrentColor == Color.white ? GetRandomColor() : CurrentColor;
			objectBehaviour.GetComponent<Renderer>().material.color = objectColor;
		}

		/// <summary>
		/// Returns a random available color, without the first entry (white/random)
		/// </summary>
		public Color GetRandomColor() {
			return AvailableColors.GetRandomElement(1).Color;
		}
		
		/// <summary>
		/// Adds all color entries to the dropdown menu
		/// </summary>
		protected override void AddDropdownEntries() {
			var optionsList = AvailableColors.Select(entry => new Dropdown.OptionData(entry.Name)).ToList();
			Dropdown.AddOptions(optionsList);
		}
	}

}