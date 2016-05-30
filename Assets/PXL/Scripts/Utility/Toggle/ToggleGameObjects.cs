using UnityEngine;
using System.Collections.Generic;

namespace PXL.Utility.Toggle {

	/// <summary>
	/// This script allows to toggel the active state of GameObjects by pressing a button
	/// </summary>
	public class ToggleGameObjects : ToggleBase {
	
		/// <summary>
		/// The GameObjects to toggle
		/// </summary>
		public List<GameObject> GameObjects = new List<GameObject>();

		protected override void HandleKeyDown() {
			GameObjects.ForEach(g => g.SetActive(!g.activeInHierarchy));
		}
	}

}