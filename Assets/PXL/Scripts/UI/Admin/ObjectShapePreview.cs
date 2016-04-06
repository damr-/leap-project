using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PXL.UI.Admin {

	[RequireComponent(typeof(Image))]
	public class ObjectShapePreview : MonoBehaviour {

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
			var c = objectSpawner.GetComponent<SetObjectShapeOnSpawn>();
			if (c == null)
				throw new MissingReferenceException(objectSpawner.gameObject.name + " is missing a SetObjectShapeOnSpawn component!");
			c.CurrentObjectShape.Subscribe(s => UpdateShapePreview(s.Texture));
			UpdateShapePreview(c.CurrentObjectShape.Value.Texture);
		}

		private void UpdateShapePreview(Sprite Texture) {
			Image.sprite = Texture;
		}

	}

}