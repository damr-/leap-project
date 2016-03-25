using UnityEngine;
using UniRx;
using System.Linq;
using System;
using PXL.Utility;
using PXL.Objects;

namespace PXL.UI.Admin {

	public class ObjectColorChanger : IndexedPropertyChanger {

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
		/// Sprite for the preview image if 'Random' is selected
		/// </summary>
		public Sprite RandomColorSprite;

		/// <summary>
		/// Current color used to tint objects
		/// </summary>
		public Color CurrentColor { get; set; }

		protected override void Start() {
			base.Start();
			ObjectSpawners.ForEach(o => o.ObjectSpawned.Subscribe(SetObjectColor));
		}

		protected override void ChangeProperty(int index) {
			if (!IsValidIndex(index))
				return;

			var newObjectColor = AvailableColors.ElementAt(index);
			var newColor = newObjectColor.Color;

			CurrentColor = newColor;
			UpdatePreviewColor(newColor);

			PropertyText.text = newObjectColor.Name;

			CurrentPropertyIndex = index;
		}

		protected override bool IsValidIndex(int index) {
			return index >= 0 && index < AvailableColors.Length;
		}

		/// <summary>
		/// Sets the color of the preview image to the current one or to the random color sprite
		/// </summary>
		/// <param name="newColor">The new color for the preview image</param>
		protected virtual void UpdatePreviewColor(Color newColor) {
			Preview.color = newColor;
			Preview.sprite = newColor == Color.white ? RandomColorSprite : null;
		}

		/// <summary>
		/// Applies the current color to the given InteractiveObject
		/// </summary>
		/// <param name="interactiveObject">The target object which will get the current color assigned</param>
		protected virtual void SetObjectColor(InteractiveObject interactiveObject) {
			var objectColor = CurrentColor == Color.white ? GetRandomColor() : CurrentColor;
			interactiveObject.GetComponent<Renderer>().material.color = objectColor;
		}

		/// <summary>
		/// Returns a random available color, without the first entry (white/random)
		/// </summary>
		public Color GetRandomColor() {
			return AvailableColors.GetRandomElement(1).Color;
		}
	}

}