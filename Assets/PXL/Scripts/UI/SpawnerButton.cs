using PXL.Objects.Spawner;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class SpawnerButton : MonoBehaviour {

		public Text SpawnerNameText;

		public Button SpawnButton;

		public Button ClearButton;

		public Text SpawnButtonText;

		public Text ClearButtonText;

		public void SetSpawner(ObjectSpawner objectSpawner) {
			SpawnerNameText.text = objectSpawner.gameObject.name;
			SpawnButtonText.text += objectSpawner.SpawnKey + ")";
			ClearButtonText.text += objectSpawner.RemoveAllKey + ")";

			SpawnButton.onClick.AddListener(objectSpawner.SpawnObject);
			ClearButton.onClick.AddListener(objectSpawner.RemoveAllObjects);
		}

	}

}