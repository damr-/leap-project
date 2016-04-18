using PXL.UI.Admin;
using PXL.Utility;
using UnityEngine;

namespace PXL.UI.World.Hand {

	public class SetupHandSpawnerElements : AdminUiBase {

		/// <summary>
		/// Prefab for the spawned UI element
		/// </summary>
		public GameObject SpawnerElementPrefab;

		protected override void Start() {
			base.Start();

			SpawnerElementPrefab.AssertNotNull();

			ObjectSpawners.ForEach(objectSpawner => {
				var spawnerElementTransform = ((GameObject)Instantiate(SpawnerElementPrefab, Vector3.zero, Quaternion.identity)).transform;
				spawnerElementTransform.SetParent(transform, false);

				var spawnerElement = spawnerElementTransform.GetComponent<HandSpawnerElement>();
				spawnerElement.SetObjectSpawner(objectSpawner);
				spawnerElement.Collapse();
			});
		}
	}

}