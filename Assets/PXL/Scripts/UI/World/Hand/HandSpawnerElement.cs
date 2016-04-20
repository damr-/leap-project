using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.World.Hand {

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