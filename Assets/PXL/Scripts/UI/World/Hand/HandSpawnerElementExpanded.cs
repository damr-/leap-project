using System.Linq;
using PXL.Objects.Spawner;
using PXL.UI.Admin;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Hand {

	public class HandSpawnerElementExpanded : MonoBehaviour {

		/// <summary>
		/// The text for the name of the spawner
		/// </summary>
		public Text SpawnerNameText;

		/// <summary>
		/// The buttons used to spawn objects
		/// </summary>
		public Button SpawnButton;

		/// <summary>
		/// The button used to clear all spawned objects
		/// </summary>
		public Button ClearButton;

		/// <summary>
		/// The text of the spawn button
		/// </summary>
		public Text SpawnButtonText;

		/// <summary>
		/// The text of the clear button
		/// </summary>
		public Text ClearButtonText;

		/// <summary>
		/// The current compact element featured in the expanded window
		/// </summary>
		private HandSpawnerElementCompact currentElement;

		public void SetSpawner(ObjectSpawner objectSpawner) {
			SpawnerNameText.text = objectSpawner.gameObject.name;

			SpawnButtonText.text = "spawn (" + objectSpawner.SpawnKey + ")";
			ClearButtonText.text = "clear (" + objectSpawner.RemoveAllKey + ")";

			SpawnButton.onClick.RemoveAllListeners();
			ClearButton.onClick.RemoveAllListeners();

			SpawnButton.onClick.AddListener(objectSpawner.SpawnObject);
			ClearButton.onClick.AddListener(objectSpawner.RemoveAllObjects);

			var propertyChangers = GetComponentsInChildren<PropertyChanger>().ToList();
			propertyChangers.ForEach(p => p.SetObjectSpawner(objectSpawner));
		}

		public void ButtonPressed(HandSpawnerElementCompact compactElement) {
			if (currentElement != null)
				currentElement.SetSelected(false);

			if (currentElement == compactElement) {
				gameObject.SetActive(false);
				currentElement = null;
			}
			else {
				SetSpawner(compactElement.ObjectSpawner);
				currentElement = compactElement;
				currentElement.SetSelected(true);
				gameObject.SetActive(true);
			}
		}

	}

}