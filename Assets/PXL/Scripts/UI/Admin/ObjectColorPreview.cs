using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PXL.UI.Admin {

	/// <summary>
	/// This script makes it possible to display a preview of the color 
	/// that all future objects of an object spawner will be colored in
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class ObjectColorPreview : MonoBehaviour {

		/// <summary>
		/// The sprite used to display normal colors
		/// </summary>
		public Sprite ColorSprite;

		/// <summary>
		/// Sprite for the preview image if 'Random' is selected
		/// </summary>
		public Sprite RandomColorSprite;

		/// <summary>
		/// The Image component of this object
		/// </summary>
		private Image Image {
			get { return mImage ?? (mImage = this.TryGetComponent<Image>()); }
		}
		private Image mImage;

		public void Setup(ObjectSpawner objectSpawner) {
			ColorSprite.AssertNotNull("Missing ColorSprite!");
			RandomColorSprite.AssertNotNull("Missing random color sprite!");

			var c = objectSpawner.GetComponent<SetObjectColorOnSpawn>();
			if (c == null)
				throw new MissingReferenceException(objectSpawner.gameObject.name + " is missing a SetObjectColorOnSpawn component!");

			c.CurrentColor.Subscribe(UpdateColorPreview);
			UpdateColorPreview(c.CurrentColor);
		}

		private void UpdateColorPreview(SetObjectColorOnSpawn.ObjectColor newObjectColor) {
			Image.color = newObjectColor.Color;
			Image.sprite = newObjectColor.Color == Color.white ? RandomColorSprite : ColorSprite;
		}

	}

}