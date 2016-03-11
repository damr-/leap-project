using UnityEngine;
using System.Collections.Generic;

namespace PXL.Utility {

	public class ToggleBehaviours : MonoBehaviour {

		/// <summary>
		/// The key used to toggle the GameObjects
		/// </summary>
		public KeyCode ToggleKey;

		/// <summary>
		/// The GameObjects to toggle
		/// </summary>
		public List<Behaviour> Behaviours = new List<Behaviour>();

		private void Update() {
			if (Input.GetKeyDown(ToggleKey)) {
				Behaviours.ForEach(b => b.enabled = !b.enabled);
			}
		}

	}

}
