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

		///// <summary>
		///// The <see cref="HandSpawnerElementExpanded"/> elemend of the hand menu
		///// </summary>
		//protected HandSpawnerElementExpanded HandSpawnerElementExpanded;

		//protected ObjectSpawner ObjectSpawner;

		/// <summary>
		/// Whether the element is currently expanded
		/// </summary>
		public bool Expanded;

		/// <summary>
		/// Sets the ObjectSpawner reference of this SpawnerElement
		/// </summary>
		public void Setup(ObjectSpawner objectSpawner, HandSpawnerElementExpanded handSpawnerElementExpanded) {
			//ObjectSpawner = objectSpawner;
			//HandSpawnerElementExpanded = handSpawnerElementExpanded;
			//handSpawnerElementExpanded.SetSpawner(objectSpawner);
			HandSpawnerElementCompact.SetupCompactElement(objectSpawner, handSpawnerElementExpanded);
		}

		///// <summary>
		///// Expand the element if it isn't yet
		///// </summary>
		//public void Expand() {
		//	if (!Expanded)
		//		SetExpanded(true);
		//}

		///// <summary>
		///// Collapse the element if it isn't yet
		///// </summary>
		//public void Collapse() {
		//	if (Expanded)
		//		SetExpanded(false);
		//}

		//private void SetExpanded(bool expanded) {
		//	HandSpawnerElementExpanded.SetSpawner(ObjectSpawner);

		//	HandSpawnerElementExpanded.gameObject.SetActive(expanded);
		//	HandSpawnerElementCompact.gameObject.SetActive(!expanded);
		//	Expanded = expanded;
		//}

	}
	
}