using PXL.Objects.Spawner;
using UniRx;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public class SpawnerElementCompact : MenuElement {

		public Text SpawnerNameText;

		public ObjectShapePreview ShapePreview;

		public ObjectColorPreview ColorPreview;

		public Text ScaleText;

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