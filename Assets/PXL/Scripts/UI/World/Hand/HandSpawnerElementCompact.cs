using PXL.Objects.Spawner;
using PXL.UI.Admin;
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
		protected ObjectSpawner ObjectSpawner;

		public void SetSpawner(ObjectSpawner objectSpawner) {
			ObjectSpawner = objectSpawner;
			SpawnerNameText.text = ObjectSpawner.gameObject.name;

			ColorPreview.Setup(ObjectSpawner);
			ShapePreview.Setup(ObjectSpawner);

			ObjectSpawner.CurrentScaleFactor.Subscribe(UpdateScaleDisplay);
			UpdateScaleDisplay(ObjectSpawner.CurrentScaleFactor.Value);
		}

		public void UpdateScaleDisplay(float newScale) {
			ScaleText.text = newScale.ToString("0.00");
		}

	}

}