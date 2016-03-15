using UnityEngine;
using System.Collections.Generic;

namespace PXL.Utility.Toggle {

	public class ToggleBehaviours : ToggleBase {
	
		/// <summary>
		/// The GameObjects to toggle
		/// </summary>
		public List<Behaviour> Behaviours = new List<Behaviour>();
		
		protected override void HandleKeyDown() {
			Behaviours.ForEach(b => b.enabled = !b.enabled);
		}
	}

}
