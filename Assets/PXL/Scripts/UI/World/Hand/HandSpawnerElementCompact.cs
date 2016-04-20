using PXL.Objects.Spawner;
using PXL.UI.Admin;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Hand {

	public class HandSpawnerElementCompact : MonoBehaviour {

		/// <summary>
		/// The text for the name of the spawner
		/// </summary>
		public Text SpawnerNameText;

		/// <summary>
		/// The preview for the object shape
		/// </summary>
		public ObjectShapePreview ShapePreview;

		/// <summary>
		/// The preview for the object color
		/// </summary>
		public ObjectColorPreview ColorPreview;

		/// <summary>
		/// The text for displaying the currently applied scale
		/// </summary>
		public Text ScaleText;

		/// <summary>
		/// The referenced ObjectSpawner
		/// </summary>
		public ObjectSpawner ObjectSpawner { get; private set; }

		/// <summary>
		/// The Image component of this object
		/// </summary>
		private Image Image {
			get { return mImage ?? (mImage = this.TryGetComponent<Image>()); }
		}
		private Image mImage;

		/// <summary>
		/// The default color for the button sprite
		/// </summary>
		private readonly Color defaultColor = new Color(191 / 255f, 191 / 255f, 191 / 255f);

		/// <summary>
		/// The color for the button sprite when it's selected
		/// </summary>
		private readonly Color selectedColor = new Color(75 / 255f, 75 / 255f, 191 / 255f);

		public void SetupCompactElement(ObjectSpawner objectSpawner, HandSpawnerElementExpanded handSpawnerElementExpanded) {
			ObjectSpawner = objectSpawner;
			SpawnerNameText.text = ObjectSpawner.gameObject.name;

			ColorPreview.Setup(ObjectSpawner);
			ShapePreview.Setup(ObjectSpawner);

			ObjectSpawner.CurrentScaleFactor.Subscribe(UpdateScaleDisplay);
			UpdateScaleDisplay(ObjectSpawner.CurrentScaleFactor.Value);

			this.TryGetComponent<Button>().onClick.AddListener(() => handSpawnerElementExpanded.ButtonPressed(this));
		}

		/// <summary>
		/// Sets the text of the <see cref="ScaleText"/> to the given, new scale
		/// </summary>
		/// <param name="newScale">The new scale which will be displayed</param>
		public void UpdateScaleDisplay(float newScale) {
			ScaleText.text = newScale.ToString("0.00");
		}

		/// <summary>
		/// Updates the selected state of this compact element by changing the color accordingly
		/// </summary>
		public void SetSelected(bool newSelectedState) {
			Image.color = newSelectedState == false ? defaultColor : selectedColor;
		}

	}

}