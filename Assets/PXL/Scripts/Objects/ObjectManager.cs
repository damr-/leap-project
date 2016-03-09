using UnityEngine;
using System.Collections.Generic;
using UniRx;
using PXL.Utility;
using PXL.UI;

namespace PXL.Objects {

	public class ObjectManager : MonoBehaviour {

		public KeyCode SpawnKey = KeyCode.X;
		public KeyCode RemoveAllKey = KeyCode.C;

		/// <summary>
		/// The GameObject which will be spawned
		/// </summary>
		public GameObject DefaultObjectPrefab;
		
		/// <summary>
		/// The Transform where the objects will be spawned
		/// If it's missing, the position of the spaner will be taken
		/// </summary>
		public Transform SpawnPositionObject;

		/// <summary>
		/// The amount of objects spawned at the start
		/// </summary>
		public int StartAmount = 2;
		
		/// <summary>
		/// Minimum possible scale of the object
		/// </summary>
		public float MinScale = 0.025f;
		
		/// <summary>
		/// Maximum possible scale of the object
		/// </summary>
		public float MaxScale = 0.1f;
		
		/// <summary>
		/// The default scale of the object
		/// </summary>
		public float DefaultScale = 0.075f;

		/// <summary>
		/// The actual used position to spawn objects
		/// </summary>
		private Vector3 spawnPosition;

		/// <summary>
		/// All the objects spawned by this ObjectManager
		/// </summary>
		private readonly List<ObjectBehaviour> spawnedObjects = new List<ObjectBehaviour>();

		/// <summary>
		/// All subscriptions to every spawned object's Destroy-Observable
		/// </summary>
		private readonly CompositeDisposable objectDestroySubscriptions = new CompositeDisposable();

		/// <summary>
		/// Whether the spawn delay is currently active
		/// </summary>
		private bool delayedSpawn;

		/// <summary>
		/// At what time the spawn delay started
		/// </summary>
		private float delayStartTime;

		/// <summary>
		/// The length of the spawn delay when removing all objects
		/// </summary>
		private const float SpawnDelay = 0.5f;

		/// <summary>
		/// The currently active prefab which will be spawned
		/// </summary>
		private GameObject currentObjectPrefab;

		public ObservableProperty<float> ObjectScale { get { return objectScale; } }
		private readonly ObservableProperty<float> objectScale = new ObservableProperty<float>();

		public IObservable<ObjectBehaviour> ObjectSpawned { get { return objectSpawnedSubject; } }
		private readonly ISubject<ObjectBehaviour> objectSpawnedSubject = new Subject<ObjectBehaviour>();

		public IObservable<Unit> SpawnInitiated { get { return spawnInitiatedSubject; } }
		private readonly ISubject<Unit> spawnInitiatedSubject = new Subject<Unit>();

		/// <summary>
		/// Factory to spawn objects
		/// </summary>
		private readonly ObjectFactory objectFactory = new ObjectFactory();
		
		/// <summary>
		/// Setup the spawn position and spawn the first object
		/// </summary>
		private void Start() {
			spawnPosition = SpawnPositionObject == null ? transform.position : SpawnPositionObject.transform.position;
			
			DefaultObjectPrefab.AssertNotNull();
            objectFactory.Prefab = DefaultObjectPrefab;
			objectFactory.Position = spawnPosition;
			SetObjectScale(DefaultScale.Remap(MinScale, MaxScale, 0, 1));
			currentObjectPrefab = DefaultObjectPrefab;

			for (var i = 0; i < StartAmount; i++)
				SpawnObject();
		}
		
		/// <summary>
		/// Checks for keyboard input and executes a delayed spawn if necessary
		/// </summary>
		private void Update() {
			if (AdminUiBase.IsAdmin) {
				if (Input.GetKeyDown(SpawnKey))
					SpawnObject();
				if (Input.GetKeyDown(RemoveAllKey))
					RemoveAllObjects();
			}
			
			if (delayedSpawn && Time.time - delayStartTime > SpawnDelay && spawnedObjects.Count <= 0) {
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

			if (!delayedSpawn && spawnedObjects.Count <= 0) {
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

			objectFactory.Prefab = currentObjectPrefab;

			var newObject = objectFactory.Spawn();

			var objectBehaviour = newObject.GetComponent<ObjectBehaviour>();
			objectBehaviour.ObjectDestroyed.Subscribe(ObjectDespawned).AddTo(objectDestroySubscriptions);

			spawnedObjects.Add(objectBehaviour);

			objectSpawnedSubject.OnNext(objectBehaviour);
		}
		
		/// <summary>
		/// Sets the scale the objects get applied when spawned
		/// The number is mapped to the range minScale to maxScale
		/// </summary>
		/// <param name="sliderValue">New scale for the objects in range [0, 1]. Will be mapped to [minScale, maxScale]</param>
		public void SetObjectScale(float sliderValue) {
			var newScale = sliderValue.Remap(0, 1, MinScale, MaxScale);
			ObjectScale.Value = newScale;
			objectFactory.Scale = newScale;
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