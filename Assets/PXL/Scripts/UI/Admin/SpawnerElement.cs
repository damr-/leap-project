using System.Linq;
using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SpawnerElement : MonoBehaviour {

		protected SpawnerElementExpanded SpawnerElementExpanded {
			get { return mSpawnerElementExpanded ?? (mSpawnerElementExpanded = GetComponentInChildren<SpawnerElementExpanded>(true)); }
		}
		private SpawnerElementExpanded mSpawnerElementExpanded;

		protected SpawnerElementCompact SpawnerElementCompact {
			get { return mSpawnerElementCompact ?? (mSpawnerElementCompact = GetComponentInChildren<SpawnerElementCompact>(true)); }
		}
		private SpawnerElementCompact mSpawnerElementCompact;

		public bool Selected;

		public bool Expanded;

		public ObjectSpawner ObjectSpawner { get; private set; }

		public void SetObjectSpawner(ObjectSpawner objectSpawner) {
			ObjectSpawner = objectSpawner;
			SpawnerElementExpanded.SetSpawner(objectSpawner);
			SpawnerElementCompact.SetSpawner(objectSpawner);

			var propertyChangers = GetComponentsInChildren<PropertyChanger>().ToList();
			propertyChangers.ForEach(p => p.ObjectSpawner = objectSpawner);
		}

		public void Select() {
			SetSelected(true);
		}

		public void Deselect() {
			SetSelected(false);
		}

		private void SetSelected(bool selected) {
			SpawnerElementCompact.IsSelected = selected;
		}

		public void Expand() {
			SetExpanded(true);
		}

		public void Collapse() {
			SetExpanded(false);
		}

		private void SetExpanded(bool expanded) {
			SpawnerElementExpanded.gameObject.SetActive(expanded);
			SpawnerElementCompact.gameObject.SetActive(!expanded);
		}

	}

}