using PXL.UI.Admin;
using PXL.Utility;
using UnityEngine;

namespace PXL.UI.World.Hand {

	public class SetupHandSpawnerElements : AdminBase {

		/// <summary>
		/// The <see cref="HandSpawnerElementExpanded"/> elemend of the hand menu
		/// </summary>
		public HandSpawnerElementExpanded HandSpawnerElementExpanded;

		/// <summary>
		/// Prefab for the spawned UI element
		/// </summary>
		public GameObject SpawnerElementPrefab;

		protected override void Start() {
			base.Start();
			
			HandSpawnerElementExpanded.AssertNotNull();
			SpawnerElementPrefab.AssertNotNull();

			ObjectSpawners.ForEach(objectSpawner => {
				var spawnerElementTransform = ((GameObject)Instantiate(SpawnerElementPrefab, Vector3.zero, Quaternion.identity)).transform;
				spawnerElementTransform.SetParent(transform, false);

				var spawnerElement = spawnerElementTransform.GetComponent<HandSpawnerElement>();
				spawnerElement.Setup(objectSpawner, HandSpawnerElementExpanded);
			});
		}
	}

}