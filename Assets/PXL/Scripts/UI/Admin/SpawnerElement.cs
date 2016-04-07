using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SpawnerElement : MonoBehaviour {

		/// <summary>
		/// The <see cref="SpawnerElementExpanded"/> of this object
		/// </summary>
		public SpawnerElementExpanded SpawnerElementExpanded {
			get { return mSpawnerElementExpanded ?? (mSpawnerElementExpanded = GetComponentInChildren<SpawnerElementExpanded>(true)); }
		}
		private SpawnerElementExpanded mSpawnerElementExpanded;

		/// <summary>
		/// The <see cref="SpawnerElementCompact"/> of this object
		/// </summary>
		public SpawnerElementCompact SpawnerElementCompact {
			get { return mSpawnerElementCompact ?? (mSpawnerElementCompact = GetComponentInChildren<SpawnerElementCompact>(true)); }
		}
		private SpawnerElementCompact mSpawnerElementCompact;

		/// <summary>
		/// Whether the element is currently selected
		/// </summary>
		public bool Selected;

		/// <summary>
		/// Whether the element is currently expanded
		/// </summary>
		public bool Expanded;

		/// <summary>
		/// The referenced ObjectSpawner
		/// </summary>
		public ObjectSpawner ObjectSpawner { get; private set; }

		/// <summary>
		/// Sets the referenced object spawner and sets up the spawner elements
		/// </summary>
		/// <param name="objectSpawner"></param>
		public void SetObjectSpawner(ObjectSpawner objectSpawner) {
			ObjectSpawner = objectSpawner;
			SpawnerElementExpanded.SetSpawner(objectSpawner);
			SpawnerElementCompact.SetSpawner(objectSpawner);
		}

		public void Select() {
			if(!Selected)
				SetSelected(true);
		}

		public void Deselect() {
			if(Selected)
				SetSelected(false);
		}

		private void SetSelected(bool selected) {
			SpawnerElementCompact.IsSelected = selected;
			Selected = selected;
		}

		public void Expand() {
			if(!Expanded)
				SetExpanded(true);
		}

		public void Collapse() {
			if(Expanded)
				SetExpanded(false);
		}

		private void SetExpanded(bool expanded) {
			SpawnerElementExpanded.gameObject.SetActive(expanded);
            SpawnerElementCompact.gameObject.SetActive(!expanded);
			Expanded = expanded;

			if (expanded)
				SpawnerElementExpanded.SelectEntry(0);
		}

		public void MouseExpand() {
			GetComponentInParent<NavigateSpawnerElements>().ExpandOtherElement(this);
        }

	}

}