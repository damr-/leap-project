using System.Linq;
using PXL.Objects.Spawner;
using PXL.UI.Admin;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Hand {

	public class HandSpawnerElementExpanded : MonoBehaviour {

		public Button SpawnButton;

		public Button ClearButton;

		public Text SpawnButtonText;

		public Text ClearButtonText;

		public void SetSpawner(ObjectSpawner objectSpawner) {
			SpawnButtonText.text += objectSpawner.SpawnKey + ")";
			ClearButtonText.text += objectSpawner.RemoveAllKey + ")";

			SpawnButton.onClick.AddListener(objectSpawner.SpawnObject);
			ClearButton.onClick.AddListener(objectSpawner.RemoveAllObjects);

			var propertyChangers = GetComponentsInChildren<PropertyChanger>().ToList();
			propertyChangers.ForEach(p => p.ObjectSpawner = objectSpawner);
		}

	}

}