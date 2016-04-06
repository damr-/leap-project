using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PXL.UI.Admin {

	[RequireComponent(typeof(Image))]
	public class ObjectColorPreview : MonoBehaviour {

		/// <summary>
		/// Sprite for the preview image if 'Random' is selected
		/// </summary>
		public Sprite RandomColorSprite;

		/// <summary>
		/// The Image component of this object
		/// </summary>
		private Image Image {
			get {
				return mImage ?? (mImage = this.TryGetComponent<Image>());
			}
		}
		private Image mImage;

		public void Setup(ObjectSpawner objectSpawner) {
			var c = objectSpawner.GetComponent<SetObjectColorOnSpawn>();
			if (c == null)
				throw new MissingReferenceException(objectSpawner.gameObject.name + " is missing a SetObjectColorOnSpawn component!");
			c.CurrentColor.Subscribe(UpdateColorPreview);
			UpdateColorPreview(c.CurrentColor);
		}

		private void UpdateColorPreview(Color newColor) {
			Image.color = newColor;
			Image.sprite = newColor == Color.white ? RandomColorSprite : null;
		}

	}

}