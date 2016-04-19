using System.Collections.Generic;
using System.Linq;
using PXL.Objects.Spawner;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public class SpawnerElementExpanded : MonoBehaviour {

		public Text SpawnerNameText;

		public Button SpawnButton;

		public Button ClearButton;

		public Text SpawnButtonText;

		public Text ClearButtonText;

		private int currentIndex;

		private PropertyChanger currentPropertyChanger;

		private List<PropertyChanger> propertyChangers = new List<PropertyChanger>();

		public void SetSpawner(ObjectSpawner objectSpawner) {
			SpawnerNameText.text = objectSpawner.gameObject.name;
			SpawnButtonText.text += objectSpawner.SpawnKey + ")";
			ClearButtonText.text += objectSpawner.RemoveAllKey + ")";

			SpawnButton.onClick.AddListener(objectSpawner.SpawnObject);
			ClearButton.onClick.AddListener(objectSpawner.RemoveAllObjects);

			propertyChangers = GetComponentsInChildren<PropertyChanger>().ToList();
			propertyChangers.ForEach(p => p.SetObjectSpawner(objectSpawner));
		}

		public bool TrySelectNextEntry() {
			return SelectEntry(currentIndex + 1);
		}

		public bool TrySelectPreviousEntry() {
			return SelectEntry(currentIndex - 1);
		}

		public bool SelectEntry(int index) {
			if (index < 0 || index >= propertyChangers.Count)
				return false;

			if (currentPropertyChanger != null)
				currentPropertyChanger.IsSelected = false;

			currentPropertyChanger = propertyChangers.ElementAt(index);
			currentPropertyChanger.IsSelected = true;
			currentIndex = index;
			return true;
		}

	}

}