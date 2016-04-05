using PXL.Objects.Spawner;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public class SpawnerElementCompact : MenuElement {
	
		public Text SpawnerNameText;

		public Image ShapePreview;

		public Image ColorPreview;

		public Text ScaleText;

		public void SetSpawner(ObjectSpawner objectSpawner) {
			SpawnerNameText.text = objectSpawner.gameObject.name;
			
			ScaleText.text = objectSpawner.ObjectScale.Value.ToString("0.0");
		}

	}

}