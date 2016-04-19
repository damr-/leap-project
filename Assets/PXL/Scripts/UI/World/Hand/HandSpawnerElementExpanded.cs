using System.Linq;
using PXL.Objects.Spawner;
using PXL.UI.Admin;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Hand {

	public class HandSpawnerElementExpanded : MonoBehaviour {

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