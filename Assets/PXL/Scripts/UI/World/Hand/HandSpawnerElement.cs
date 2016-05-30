using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.World.Hand {

	/// <summary>
	/// This script adds functionality to a hand menu element.
	/// 
	/// It stores the reference to the <see cref="HandSpawnerElementCompact"/> and provides a Setup function to set up the compact element
	/// </summary>
	public class HandSpawnerElement : MonoBehaviour {
	
		/// <summary>
		/// The <see cref="HandSpawnerElementCompact"/> of this object
		/// </summary>
		public HandSpawnerElementCompact HandSpawnerElementCompact {
			get { return mHandSpawnerElementCompact ?? (mHandSpawnerElementCompact = GetComponentInChildren<HandSpawnerElementCompact>(true)); }
		}
		private HandSpawnerElementCompact mHandSpawnerElementCompact;
		
		/// <summary>
		/// Sets the ObjectSpawner reference of this SpawnerElement
		/// </summary>
		public void Setup(ObjectSpawner objectSpawner, HandSpawnerElementExpanded handSpawnerElementExpanded) {
			HandSpawnerElementCompact.SetupCompactElement(objectSpawner, handSpawnerElementExpanded);
		}

	}
	
}