using System;
using System.Linq;
using UniRx;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectColorOnSpawn : MonoBehaviour {

		/// <summary>
		/// The ObjectSpawner of this object
		/// </summary>
		private ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}
		private ObjectSpawner mObjectSpawner;

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

		private void Start() {
			CurrentColor.Value = AvailableColors.ElementAt(1 % AvailableColors.Length).Color;
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