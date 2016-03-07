using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using PXL.Utility;
using PXL.UI;

namespace PXL.Objects {

	public class ObjectManager : MonoBehaviour {

		public KeyCode spawnKey = KeyCode.X;
		public KeyCode removeAllKey = KeyCode.C;

		/// <summary>
		/// The GameObject which will be spawned
		/// </summary>
		public GameObject defaultObjectPrefab;
		
		/// <summary>
		/// The Transform where the objects will be spawned
		/// If it's missing, the position of the spaner will be taken
		/// </summary>
		public Transform spawnPositionObject;

		/// <summary>
		/// The amount of objects spawned at the start
		/// </summary>
		public int startAmount = 2;
		
		/// <summary>
		/// Minimum possible scale of the object
		/// </summary>
		public float minScale = 0.025f;
		
		/// <summary>
		/// Maximum possible scale of the object
		/// </summary>
		public float maxScale = 0.1f;
		
		/// <summary>
		/// The default scale of the object
		/// </summary>
		public float defaultScale = 0.05f;

		/// <summary>
		/// The actual used position to spawn objects
		/// </summary>
		private Vector3 spawnPosition;

		/// <summary>
		/// All the objects spawned by this ObjectManager
		/// </summary>
		private List<ObjectBehaviour> spawnedObjects = new List<ObjectBehaviour>();

		/// <summary>
		/// All subscriptions to every spawned object's Destroy-Observable
		/// </summary>
		private CompositeDisposable objectDestroySubscriptions = new CompositeDisposable();
		
		/// <summary>
		/// Whether the spawn delay is currently active
		/// </summary>
		private bool delayedSpawn = false;

		/// <summary>
		/// At what time the spawn delay started
		/// </summary>
		private float delayStartTime;
		
		/// <summary>
		/// The length of the spawn delay when removing all objects
		/// </summary>
		private float spawnDelay = 0.5f;
		
		/// <summary>
		/// The currently active prefab which will be spawned
		/// </summary>
		private GameObject currentObjectPrefab;

		public ObservableProperty<float> ObjectScale { get { return objectScale; } }
		private ObservableProperty<float> objectScale = new ObservableProperty<float>();

		public IObservable<ObjectBehaviour> ObjectSpawned { get { return objectSpawnedSubject; } }
		private ISubject<ObjectBehaviour> objectSpawnedSubject = new Subject<ObjectBehaviour>();

		public IObservable<Unit> SpawnInitiated { get { return spawnInitiatedSubject; } }
		private ISubject<Unit> spawnInitiatedSubject = new Subject<Unit>();

		/// <summary>
		/// Factory to spawn objects
		/// </summary>
		private ObjectFactory objectFactory = new ObjectFactory();
		
		/// <summary>
		/// Setup the spawn position and spawn the first object
		/// </summary>
		void Start() {
			spawnPosition = (spawnPositionObject == null) ? transform.position : spawnPositionObject.transform.position;
			objectFactory.prefab = defaultObjectPrefab;
			objectFactory.position = spawnPosition;
			SetObjectScale(defaultScale.Remap(minScale, maxScale, 0, 1));
			currentObjectPrefab = defaultObjectPrefab;

			for (int i = 0; i < startAmount; i++)
				SpawnObject();
		}
		
		/// <summary>
		/// Checks for keyboard input and executes a delayed spawn if necessary
		/// </summary>
		void Update() {
			if (AdminUIBase.IsAdmin) {
				if (Input.GetKeyDown(spawnKey))
					SpawnObject();
				if (Input.GetKeyDown(removeAllKey))
					RemoveAllObjects();
			}

			if (delayedSpawn && Time.time - delayStartTime > spawnDelay && spawnedObjects.Count <= 0) {
				delayedSpawn = false;
				SpawnObject();
			}
		}
		
		/// <summary>
		/// Called when a object was destroyed
		/// Removes the object from the list and spawns one if possible and necessary
		/// </summary>
		/// <param name="objectBehaviour">The ObjectBehaviour of the object which is about to be despawned</param>
		private void ObjectDespawned(ObjectBehaviour objectBehaviour) {
			spawnedObjects.Remove(objectBehaviour);

			if (!delayedSpawn && spawnedObjects.Count() <= 0) {
				SpawnObject();
			}
		}
		
		/// <summary>
		/// Removes all existing objects
		/// </summary>
		public void RemoveAllObjects() {
			delayedSpawn = true;
			delayStartTime = Time.time;
			while (spawnedObjects.Count > 0) {
				spawnedObjects[0].DestroyObject();
			}
		}
		
		/// <summary>
		/// Initiate the spawning of a new object
		/// </summary>
		public void SpawnObject() {
			spawnInitiatedSubject.OnNext(Unit.Default);

			objectFactory.prefab = currentObjectPrefab;

			GameObject newObject = objectFactory.Spawn();

			ObjectBehaviour objectBehaviour = newObject.GetComponent<ObjectBehaviour>();
			objectBehaviour.ObjectDestroyed.Subscribe(o => ObjectDespawned(o)).AddTo(objectDestroySubscriptions);

			spawnedObjects.Add(objectBehaviour);

			objectSpawnedSubject.OnNext(objectBehaviour);
		}
		
		/// <summary>
		/// Sets the scale the objects get applied when spawned
		/// The number is mapped to the range minScale to maxScale
		/// </summary>
		/// <param name="sliderValue">New scale for the objects in range [0, 1]. Will be mapped to [minScale, maxScale]</param>
		public void SetObjectScale(float sliderValue) {
			float newScale = sliderValue.Remap(0, 1, minScale, maxScale);
			ObjectScale.Value = newScale;
			objectFactory.scale = newScale;
		}
		
		/// <summary>
		/// Sets the objectPrefab which will be spawned
		/// </summary>
		/// <param name="newPrefab">The GameObject of the new prefab</param>
		public void SetObjectPrefab(GameObject newPrefab) {
			currentObjectPrefab = newPrefab;
		}
	}

}