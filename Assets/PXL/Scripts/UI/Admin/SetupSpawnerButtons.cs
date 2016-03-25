using PXL.Utility;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SetupSpawnerButtons : AdminUiBase {

		/// <summary>
		/// Prefab for the spawned UI element
		/// </summary>
		public GameObject SpawnerButtonsPrefab;

		/// <summary>
		/// The Rect Transform of the admin panel
		/// </summary>
		public RectTransform AdminPanelTransform;

		protected override void Start() {
			base.Start();

			SpawnerButtonsPrefab.AssertNotNull();
			AdminPanelTransform.AssertNotNull();

			ObjectSpawners.ForEach(objectSpawner => {
				var spawnerButtonsPrefab = (GameObject)Instantiate(SpawnerButtonsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				spawnerButtonsPrefab.transform.SetParent(transform, true);

				var buttonSpawner = spawnerButtonsPrefab.GetComponent<SpawnerButton>();
				buttonSpawner.SetSpawner(objectSpawner);
				var r = AdminPanelTransform.sizeDelta;
				AdminPanelTransform.sizeDelta = new Vector2(r.x, r.y + 50);
			});
		}

	}

}