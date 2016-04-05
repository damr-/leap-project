using PXL.Utility;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SetupSpawnerElements : AdminUiBase {

		/// <summary>
		/// Prefab for the spawned UI element
		/// </summary>
		public GameObject SpawnerElementPrefab;

		/// <summary>
		/// The Rect Transform of the admin panel
		/// </summary>
		public RectTransform AdminPanelTransform;

		protected override void Start() {
			base.Start();

			SpawnerElementPrefab.AssertNotNull();
			AdminPanelTransform.AssertNotNull();

			ObjectSpawners.ForEach(objectSpawner => {
				var spawnerElementTransform = (GameObject)Instantiate(SpawnerElementPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				spawnerElementTransform.transform.SetParent(transform, true);

				var spawnerElement = spawnerElementTransform.GetComponent<SpawnerElement>();
				spawnerElement.SetObjectSpawner(objectSpawner);

				spawnerElement.Collapse();

				//var buttonSpawner = spawnerButtonsPrefab.GetComponentInChildren<SpawnerElementExpanded>();
				//buttonSpawner.SetSpawner(objectSpawner);

				var r = AdminPanelTransform.sizeDelta;
				AdminPanelTransform.sizeDelta = new Vector2(r.x, r.y + 50);
			});
		}

	}

}