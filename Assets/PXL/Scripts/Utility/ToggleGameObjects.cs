using UnityEngine;
using System.Collections.Generic;

namespace PXL.Utility {

	public class ToggleGameObjects : MonoBehaviour {

		/// <summary>
		/// The key used to toggle the GameObjects
		/// </summary>
		public KeyCode ToggleKey;

		/// <summary>
		/// The GameObjects to toggle
		/// </summary>
		public List<GameObject> GameObjects = new List<GameObject>();

		private void Update() {
			if (Input.GetKeyDown(ToggleKey)) {
				GameObjects.ForEach(g => g.SetActive(!g.activeInHierarchy));
			}
		}

	}

}