using System;
using UnityEngine;
using System.Collections.Generic;
using PXL.UI;
using UniRx;
using PXL.Utility;

namespace PXL.Objects.Spawner {

	public abstract class ObjectSpawner : MonoBehaviour {

		/// <summary>
		/// The key used to spawn an object
		/// </summary>
		public KeyCode SpawnKey = KeyCode.X;

		/// <summary>
		/// The key used to remove all objects
		/// </summary>
		public KeyCode RemoveAllKey = KeyCode.C;

		/// <summary>
		/// The GameObject which will be spawned
		/// </summary>
		public GameObject DefaultObjectPrefab;

		/// <summary>
		/// The Transform which will be parent of all spawned objects, if set.
		/// </summary>
		public Transform SpawnedObjectsContainer;

		/// <summary>
		/// Whether this spawner is able to spawn anything
		/// </summary>
		public bool IsSpawningEnabled = true;

		/// <summary>
		/// The amount of objects spawned at start
		/// </summary>
		public int StartAmount = 1;

		/// <summary>
		/// How many seconds to wait before this spawner gets activated
		/// </summary>
		public int StartSpawnDelay = 0;

		/// <summary>
		/// Whether the spawner should call <see cref="SpawnObject()"/> when all it's spawned objects have been destroyed
		/// </summary>
		public bool RespawnOnDepleted = true;

		/// <summary>
		/// How many seconds to wait before a new object is spawned after the spawner's spawned objects have been destroyed
		/// </summary>
		public int RespawnDelay = 0;

		/// <summary>
		/// The maximum amount of objects this spawner can spawn in total.
		/// -1 means infinite
		/// </summary>
		public int TotalSpawnLimit = -1;

		/// <summary>
		/// The maximum amount of active objects, spawned from this spawner, that are allowed at the same time.
		/// -1 means infinite
		/// </summary>
		public int ConcurrentSpawnLimit = -1;

		/// <summary>
		/// Minimum possible scale of the object
		/// </summary>
		public float MinScaleFactor = 0.5f;

		/// <summary>
		/// Maximum possible scale of the object
		/// </summary>
		public float MaxScaleFactor = 2.5f;

		/// <summary>
		/// The default scale of the object
		/// </summary>
		public float DefaultScaleFactor = 1f;

		/// <summary>
		/// How much the scale changes when de-/increasing
		/// </summary>
		protected float ScaleChangeAmount = 0.1f;

		/// <summary>
		/// The current set scale for objects
		/// </summary>
		protected float CurrentScaleFactor;

		/// <summary>
		/// All the objects spawned by this ObjectManager
		/// </summary>
		protected readonly List<InteractiveObject> SpawnedObjects = new List<InteractiveObject>();

		/// <summary>
		/// The currently active prefab which will be spawned
		/// </summary>
		protected GameObject CurrentObjectPrefab;

		/// <summary>
		/// The total count of all objects spawned by this spawner
		/// </summary>
		public ObservableProperty<int> TotalSpawnCount = new ObservableProperty<int>();

		/// <summary>
		/// The total count of all objects which have been spawned by this spawner and despawned already
		/// </summary>
		public ObservableProperty<int> TotalDespawnCount = new ObservableProperty<int>();

		/// <summary>
		/// All subscriptions to every spawned object's Destroy-Observable
		/// </summary>
		private readonly IDictionary<GameObject, IDisposable> objectDestroySubscriptions = new Dictionary<GameObject, IDisposable>();

		/// <summary>
		/// Invoked when the object scale changes
		/// </summary>
		public ObservableProperty<float> ObjectScale = new ObservableProperty<float>();

		/// <summary>
		/// Invoked when an object is spawned
		/// </summary>
		public IObservable<InteractiveObject> ObjectSpawned { get { return objectSpawnedSubject; } }
		private readonly ISubject<InteractiveObject> objectSpawnedSubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Invoked when an object is despawned
		/// </summary>
		public IObservable<InteractiveObject> ObjectDespawned { get { return objectDespawnedSubject; } }
		private readonly ISubject<InteractiveObject> objectDespawnedSubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Invoked when the spawning of an object is initiated
		/// </summary>
		public IObservable<Unit> SpawnInitiated { get { return spawnInitiatedSubject; } }
		private readonly ISubject<Unit> spawnInitiatedSubject = new Subject<Unit>();

		/// <summary>
		/// Factory to spawn objects
		/// </summary>
		protected readonly ObjectFactory ObjectFactory = new ObjectFactory();

		/// <summary>
		/// The length of the spawn delay when removing all objects
		/// </summary>
		private const float RemoveAllSpawnDelay = 0.5f;

		/// <summary>
		/// Setup the spawn position and spawn the first object
		/// </summary>
		protected virtual void Start() {
			DefaultObjectPrefab.AssertNotNull();
			ObjectFactory.Prefab = DefaultObjectPrefab;
			ObjectFactory.Position = transform.position;

			CurrentObjectPrefab = DefaultObjectPrefab;
			SetObjectScale(DefaultScaleFactor);

			InitiateInitialSpawns();
		}

		/// <summary>
		/// Disables spawning and sets up the timer for the <see cref="StartAmount"/> number of objects to be spawned
		/// </summary>
		protected virtual void InitiateInitialSpawns() {
			IsSpawningEnabled = false;

			Observable.Timer(TimeSpan.FromSeconds(StartSpawnDelay)).Subscribe(_ => {
				IsSpawningEnabled = true;
				for (var i = 0; i < StartAmount; i++)
					SpawnObject();
			});
		}

		/// <summary>
		/// Checks if the Admin-mode is active and a key is pressed
		/// </summary>
		protected virtual void Update() {
			if (!AdminUiBase.IsAdmin)
				return;

			if (Input.GetKeyDown(SpawnKey))
				SpawnObject();
			if (Input.GetKeyDown(RemoveAllKey))
				RemoveAllObjects();
		}


		/// <summary>
		/// Returns whether this spawner can spawn an object
		/// </summary>
		protected virtual bool CanSpawn() {
			if (ConcurrentSpawnLimit != -1 && SpawnedObjects.Count >= ConcurrentSpawnLimit)
				return false;

			if (TotalSpawnLimit != -1 && TotalSpawnCount >= TotalSpawnLimit)
				return false;

			return IsSpawningEnabled;
		}

		/// <summary>
		/// Called when a object was destroyed
		/// Removes the object from the list and spawns one if possible and necessary
		/// </summary>
		protected virtual void HandleObjectDespawned(InteractiveObject interactiveObject) {
			SpawnedObjects.Remove(interactiveObject);

			objectDespawnedSubject.OnNext(interactiveObject);
			TotalDespawnCount.Value++;

			if (!RespawnOnDepleted || SpawnedObjects.Count != 0 || !IsSpawningEnabled)
				return;

			IsSpawningEnabled = false;
			Observable.Timer(TimeSpan.FromSeconds(RespawnDelay)).Subscribe(_ => {
				IsSpawningEnabled = true;
				SpawnObject();
			});
		}

		/// <summary>
		/// Prevents spawning and removes all existing objects.
		/// Enables spawning again after a small delay
		/// </summary>
		public virtual void RemoveAllObjects() {
			IsSpawningEnabled = false;

			while (SpawnedObjects.Count > 0) {
				SpawnedObjects[0].Kill();
			}

			Observable.Timer(TimeSpan.FromSeconds(RemoveAllSpawnDelay)).Subscribe(_ => {
				IsSpawningEnabled = true;
				SpawnObject();
			});
		}

		/// <summary>
		/// Initiate the spawning of a new object without an offset
		/// </summary>
		public virtual void SpawnObject() {
			SpawnObject(Vector3.zero);
		}

		/// <summary>
		/// Initiate the spawning of a new object with the given offset
		/// </summary>
		public void SpawnObject(Vector3 offset) {
			if (!CanSpawn())
				return;

			spawnInitiatedSubject.OnNext(Unit.Default);

			var newObject = CreateObject(offset);

			SetupSpawnedObject(newObject);

			TotalSpawnCount.Value++;
		}

		/// <summary>
		/// Sets the prefab and the offset of the factory and creates a new object with it
		/// </summary>
		private GameObject CreateObject(Vector3 offset) {
			ObjectFactory.Prefab = CurrentObjectPrefab;
			ObjectFactory.Position = transform.position + offset;
			return ObjectFactory.Spawn();
		}

		/// <summary>
		/// Sets up subscriptions on the new object, adds it to the list of spawned objects and sets up component properties
		/// </summary>
		private void SetupSpawnedObject(GameObject newObject) {
			var interactiveObject = newObject.GetComponent<InteractiveObject>();

			var health = newObject.GetComponent<Health.Health>();
			health.AssertNotNull("Object is missing a Health component!");

			var subscription = health.Death.Subscribe(_ => {
				objectDestroySubscriptions[health.gameObject].Dispose();
                objectDestroySubscriptions.Remove(health.gameObject);
				HandleObjectDespawned(interactiveObject);
			});

			objectDestroySubscriptions.Add(new KeyValuePair<GameObject, IDisposable>(newObject, subscription));

			SpawnedObjects.Add(interactiveObject);

			if(SpawnedObjectsContainer != null)
				newObject.transform.SetParent(SpawnedObjectsContainer, true);

			objectSpawnedSubject.OnNext(interactiveObject);
		}

		/// <summary>
		/// Increases the object scale of future objects by <see cref="ScaleChangeAmount"/>
		/// </summary>
		public void IncreaseObjectScale() {
			SetObjectScale(CurrentScaleFactor + ScaleChangeAmount);
		}

		/// <summary>
		/// Decreases the object scale of future objects by <see cref="ScaleChangeAmount"/>
		/// </summary>
		public void DecreaseObjectScale() {
			SetObjectScale(CurrentScaleFactor - ScaleChangeAmount);
		}

		/// <summary>
		/// Resets the scale factor for objects to the default value
		/// </summary>
		public void ResetObjectScale() {
			SetObjectScale(DefaultScaleFactor);
		}

		/// <summary>
		/// Changes the object scale depending on the given bool and updates the variables
		/// </summary>
		private void SetObjectScale(float newScale) {
			if (newScale < MinScaleFactor || newScale > MaxScaleFactor)
				return;
			CurrentScaleFactor = newScale;
			ObjectScale.Value = newScale;
			ObjectFactory.Scale = newScale;
		}

		/// <summary>
		/// Sets the objectPrefab which will be spawned
		/// </summary>
		/// <param name="newPrefab">The GameObject of the new prefab</param>
		public void SetObjectPrefab(GameObject newPrefab) {
			CurrentObjectPrefab = newPrefab;
		}

		/// <summary>
		/// Clear the subscriptions when this ObjectManager is disabled
		/// </summary>
		private void OnDisable() {
			foreach (var entry in objectDestroySubscriptions) {
				entry.Value.Dispose();
			}
		}
	}

}