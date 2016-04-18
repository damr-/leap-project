using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.World.Hand {

	public class HandSpawnerElement : MonoBehaviour {
	
		/// <summary>
		/// The <see cref="HandSpawnerElementExpanded"/> of this object
		/// </summary>
		public HandSpawnerElementExpanded HandSpawnerElementExpanded {
			get { return mHandSpawnerElementExpanded ?? (mHandSpawnerElementExpanded = GetComponentInChildren<HandSpawnerElementExpanded>(true)); }
		}
		private HandSpawnerElementExpanded mHandSpawnerElementExpanded;

		/// <summary>
		/// The <see cref="HandSpawnerElementCompact"/> of this object
		/// </summary>
		public HandSpawnerElementCompact HandSpawnerElementCompact {
			get { return mHandSpawnerElementCompact ?? (mHandSpawnerElementCompact = GetComponentInChildren<HandSpawnerElementCompact>(true)); }
		}
		private HandSpawnerElementCompact mHandSpawnerElementCompact;
		

		/// <summary>
		/// Whether the element is currently expanded
		/// </summary>
		public bool Expanded;

		public void SetObjectSpawner(ObjectSpawner objectSpawner) {
			HandSpawnerElementExpanded.SetSpawner(objectSpawner);
			HandSpawnerElementCompact.SetSpawner(objectSpawner);
		}

		public void Expand() {
			if (!Expanded)
				SetExpanded(true);
		}

		public void Collapse() {
			if (Expanded)
				SetExpanded(false);
		}

		private void SetExpanded(bool expanded) {
			HandSpawnerElementExpanded.gameObject.SetActive(expanded);
			HandSpawnerElementCompact.gameObject.SetActive(!expanded);
			Expanded = expanded;
		}

	}
	
}