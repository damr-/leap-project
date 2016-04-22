using System.Collections.Generic;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SetupSpawnerElements : AdminBase {

		/// <summary>
		/// Prefab for the spawned UI element
		/// </summary>
		public GameObject SpawnerElementPrefab;

		/// <summary>
		/// The Rect Transform of the admin panel
		/// </summary>
		public RectTransform AdminPanelTransform;

		public IObservable<List<SpawnerElement>> SpawnFinished { get { return spawnFinishedsSubject; } }
		private readonly ISubject<List<SpawnerElement>> spawnFinishedsSubject = new Subject<List<SpawnerElement>>();

		protected override void Start() {
			base.Start();

			SpawnerElementPrefab.AssertNotNull();
			AdminPanelTransform.AssertNotNull();

			var spawnedElements = new List<SpawnerElement>();

			ObjectSpawners.ForEach(objectSpawner => {
				var spawnerElementTransform = (GameObject)Instantiate(SpawnerElementPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				spawnerElementTransform.transform.SetParent(transform, true);

				var spawnerElement = spawnerElementTransform.GetComponent<SpawnerElement>();
				spawnerElement.SetObjectSpawner(objectSpawner);
				spawnerElement.Collapse();

				spawnedElements.Add(spawnerElement);

				var r = AdminPanelTransform.sizeDelta;
				AdminPanelTransform.sizeDelta = new Vector2(r.x, r.y + 50);
			});

			spawnFinishedsSubject.OnNext(spawnedElements);
		}

	}

}