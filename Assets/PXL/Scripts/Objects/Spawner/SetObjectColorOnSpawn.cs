﻿using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectColorOnSpawn : MonoBehaviour {
	
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
		/// The default color of the spawner's spawned objects
		/// </summary>
		public ObjectColor DefaultColor;

		/// <summary>
		/// All the available colors
		/// </summary>
		public List<ObjectColor> AvailableColors = new List<ObjectColor>() {
			new ObjectColor(Color.white, "Random"),
			new ObjectColor(new Color(200 / 255f, 50 / 255f, 50 / 255f, 1f), "Red"),
			new ObjectColor(new Color(50 / 255f, 200 / 255f, 50 / 255f, 1f), "Green"),
			new ObjectColor(new Color(50 / 255f, 50 / 255f, 200 / 255f, 1f), "Blue"),
			new ObjectColor(new Color(200 / 255f, 200 / 255f, 50 / 255f, 1f), "Yellow"),
			new ObjectColor(new Color(50 / 255f, 200 / 255f, 200 / 255f, 1f), "Cyan"),
			new ObjectColor(new Color(200 / 255f, 50 / 255f, 200 / 255f, 1f) , "Magenta"),
			new ObjectColor(new Color(146 / 255f, 174 / 255f, 255 / 255f, 1f) , "Light Blue"),
			new ObjectColor(new Color(105 / 255f, 105 / 255f, 105 / 255f, 1f) , "Grey")
		};

		/// <summary>
		/// Current color used to dye objects
		/// </summary>
		public ObservableProperty<Color> CurrentColor = new ObservableProperty<Color>();

		/// <summary>
		/// The ObjectSpawner of this object
		/// </summary>
		private ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}
		private ObjectSpawner mObjectSpawner;

		/// <summary>
		/// The color which will be added to the list
		/// </summary>
		public ObjectColor NewColor;

		/// <summary>
		/// Adds <see cref="NewColor"/> to the list of <see cref="AvailableColors"/>, if the name is not already taken
		/// </summary>
		public void AddColor() {
			var componentName = NewColor.Name.Trim();
			if (componentName.Length == 0 || AvailableColors.Any(c => c.Name == NewColor.Name))
				return;
			AvailableColors.Add(NewColor);
		}

		/// <summary>
		/// Sets the default color as the current one and sets up the spawnsubsription
		/// </summary>
		private void Start() {
            CurrentColor.Value = DefaultColor.Color;
			ObjectSpawner.ObjectSpawned.Subscribe(SetObjectColor);
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

		/// <summary>
		/// Sets the color used to dye objects
		/// </summary>
		/// <param name="newColor"></param>
		public void SetColor(Color newColor) {
			CurrentColor.Value = newColor;
		}

	}

}