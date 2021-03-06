﻿using System;
using UnityEngine;
using System.Collections.Generic;
using PXL.UI.Admin;
using UniRx;
using PXL.Utility;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// Base class for a spawner which creates and keeps track of objects.
	/// It provides functionality to spawn a new object or remove all currently existing ones.
	/// 
	/// It can be set that these functions are only available when the admin mode is active and 
	/// it can be set whether this spawner appears in the admin's hand menu.
	///
	/// The spawner can be set to spawn a certain amount of object in the beginning and to respawn 
	/// objects as soon as all existing ones have been removed or decreased to less than a certain amount.
	/// 
	/// </summary>
	public class ObjectSpawner : MonoBehaviour {

		/// <summary>
		/// The key used to spawn an object
		/// </summary>
		public KeyCode SpawnKey = KeyCode.X;

		/// <summary>
		/// The key used to remove all objects
		/// </summary>
		public KeyCode RemoveAllKey = KeyCode.C;

		/// <summary>
		/// Whether the input is only registered in admin mode
		/// </summary>
		public bool AdminModeRequired;

		/// <summary>
		/// Whether this spawner will be visible and editable in the admin hand menu
		/// </summary>
		public bool InHandMenu = true;

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
		/// At what frequency the <see cref="StartAmount"/> number of objects will be spawned after the <see cref="StartSpawnDelay"/>
		/// </summary>
		public float StartSpawnFrequency = 10f;

		/// <summary>
		/// Whether the spawner should call <see cref="SpawnObject()"/> when all it's spawned objects have been destroyed
		/// </summary>
		public bool RespawnOnDepleted = true;

		/// <summary>
		/// How many seconds to wait before a new object is spawned after the spawner's spawned objects have been destroyed
		/// </summary>
		public float RespawnDelay = 0;

		/// <summary>
		/// The minimum objects spawned by this spawner that should exist at all times
		/// </summary>
		public int MinObjectAmount = 0;

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
		/// The current scale for objects
		/// </summary>
		public ObservableProperty<float> CurrentScaleFactor = new ObservableProperty<float>();

		/// <summary>
		/// All the objects spawned by this ObjectManager
		/// </summary>
		protected readonly List<InteractiveObject> SpawnedObjects = new List<InteractiveObject>();

		/// <summary>
		/// The total count of all objects spawned by this spawner
		/// </summary>
		public ObservableProperty<int> TotalSpawnCount = new ObservableProperty<int>();

		/// <summary>
		/// The total count of all objects which have been spawned by this spawner and despawned already
		/// </summary>
		public ObservableProperty<int> TotalDespawnCount = new ObservableProperty<int>();

		/// <summary>
		/// All disposables to every spawned object's Destroy-Observable
		/// </summary>
		private readonly IDictionary<GameObject, IDisposable> objectDestroyDisposables =
			new Dictionary<GameObject, IDisposable>();

		/// <summary>
		/// Invoked when an object is spawned
		/// </summary>
		public IObservable<InteractiveObject> ObjectSpawned {
			get { return objectSpawnedSubject; }
		}
		private readonly ISubject<InteractiveObject> objectSpawnedSubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Invoked when an object is despawned
		/// </summary>
		public IObservable<InteractiveObject> ObjectDespawned {
			get { return objectDespawnedSubject; }
		}
		private readonly ISubject<InteractiveObject> objectDespawnedSubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Invoked when the spawning of an object is initiated
		/// </summary>
		public IObservable<Unit> SpawnInitiated {
			get { return spawnInitiatedSubject; }
		}
		private readonly ISubject<Unit> spawnInitiatedSubject = new Subject<Unit>();

		/// <summary>
		/// Factory to spawn objects
		/// </summary>
		protected ObjectFactory ObjectFactory;

		/// <summary>
		/// The length of the spawn delay when removing all objects
		/// </summary>
		private const float RemoveAllSpawnDelay = 0.5f;

		/// <summary>
		/// How many objects are respawned per second when all objects got removed
		/// </summary>
		public float RespawnFrequency = 2f;

		/// <summary>
		/// Whether the rotation of the objects should be set
		/// </summary>
		public bool SetObjectRotation;

		/// <summary>
		/// The rotation of the spawned objects
		/// </summary>
		public Vector3 ObjectRotation;

		/// <summary>
		/// Whether the objects can be removed at this moment
		/// </summary>
		private bool canRemoveObjects = true;

		/// <summary>
		/// The currently used GameObject for newly spawned objects
		/// </summary>
		public GameObject CurrentObjectPrefab { get; private set; }

		/// <summary>
		/// Disposable for spawning new objects when all got removed
		/// </summary>
		private IDisposable removeAllDelayDisposable = Disposable.Empty;

		/// <summary>
		/// The disposable before starting to spawn
		/// </summary>
		private IDisposable startSpawnDelayDisposable = Disposable.Empty;

		/// <summary>
		/// The disposable for the interval to spawn the start amount of objects
		/// </summary>
		private IDisposable startSpawnDisposable = Disposable.Empty;

		/// <summary>
		/// The disposable for the respawn delay
		/// </summary>
		private IDisposable respawnDelayDisposable = Disposable.Empty;

		/// <summary>
		/// The disposable for the respawn of all objects
		/// </summary>
		private IDisposable respawnDisposable = Disposable.Empty;

		/// <summary>
		/// Setup the spawn position and spawn the first object
		/// </summary>
		protected virtual void Start() {
			DefaultObjectPrefab.AssertNotNull();

			ObjectFactory = new ObjectFactory {
				Prefab = DefaultObjectPrefab,
				Position = transform.position
			};

			CurrentObjectPrefab = DefaultObjectPrefab;
			SetObjectScale(DefaultScaleFactor);

			if (SetObjectRotation)
				ObjectFactory.Rotation = ObjectRotation;

			InitiateInitialSpawns();
		}

		/// <summary>
		/// Disables spawning and sets up the timer for the <see cref="StartAmount"/> number of objects to be spawned
		/// </summary>
		protected virtual void InitiateInitialSpawns() {
			IsSpawningEnabled = false;
			canRemoveObjects = false;

			startSpawnDelayDisposable = Observable.Timer(TimeSpan.FromSeconds(StartSpawnDelay)).Subscribe(_ => {
				IsSpawningEnabled = true;

				SpawnObject();
				startSpawnDisposable = Observable.Interval(TimeSpan.FromSeconds(1f / StartSpawnFrequency)).Subscribe(__ => {
					if (SpawnedObjects.Count >= StartAmount) {
						startSpawnDisposable.Dispose();
						canRemoveObjects = true;
						return;
					}
					SpawnObject();
				});
			});
		}

		/// <summary>
		/// Checks if the Admin-mode is active and a key is pressed
		/// </summary>
		protected virtual void Update() {
			if (AdminModeRequired && !AdminBase.IsAdmin)
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

			if (!RespawnOnDepleted || !IsSpawningEnabled || SpawnedObjects.Count >= MinObjectAmount)
				return;

			IsSpawningEnabled = false;
			respawnDelayDisposable.Dispose();
			canRemoveObjects = false;
			respawnDelayDisposable = Observable.Timer(TimeSpan.FromSeconds(RespawnDelay)).Subscribe(_ => {
				IsSpawningEnabled = true;
				RespawnWhileTooFew();
			});
		}

		/// <summary>
		/// Prevents spawning and removes all existing objects.
		/// Enables spawning again after a small delay, if respawning is enabled
		/// </summary>
		public virtual void RemoveAllObjects() {
			if (!canRemoveObjects)
				return;

			IsSpawningEnabled = false;

			while (SpawnedObjects.Count > 0)
				SpawnedObjects[0].Kill();

			if (!RespawnOnDepleted)
				return;

			removeAllDelayDisposable.Dispose();
			canRemoveObjects = false;
			removeAllDelayDisposable = Observable.Timer(TimeSpan.FromSeconds(RemoveAllSpawnDelay)).Subscribe(_ => {
				IsSpawningEnabled = true;
				RespawnWhileTooFew();
			});
		}

		/// <summary>
		/// Spawns new objects as long as the amount of <see cref="SpawnedObjects"/> is less than <see cref="MinObjectAmount"/>
		/// </summary>
		private void RespawnWhileTooFew() {
			respawnDisposable = Observable.Interval(TimeSpan.FromSeconds(1 / RespawnFrequency)).Subscribe(_ => {
				if (SpawnedObjects.Count >= MinObjectAmount) {
					canRemoveObjects = true;
					respawnDisposable.Dispose();
					return;
				}
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

			if (newObject == null) {
				SpawnObject(offset);
				return;
			}

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
			var interactiveObject = newObject.GetComponentInChildren<InteractiveObject>();

			var health = newObject.GetComponent<Health.Health>();
			health.AssertNotNull("Object is missing a Health component!");

			var disposable = health.Death.Subscribe(_ => {
				objectDestroyDisposables[health.gameObject].Dispose();
				objectDestroyDisposables.Remove(health.gameObject);
				HandleObjectDespawned(interactiveObject);
			});

			objectDestroyDisposables.Add(new KeyValuePair<GameObject, IDisposable>(newObject, disposable));

			SpawnedObjects.Add(interactiveObject);

			if (SpawnedObjectsContainer != null)
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
		/// Changes the object scale to the given new scale
		/// </summary>
		protected void SetObjectScale(float newScale) {
			if (newScale < MinScaleFactor || newScale > MaxScaleFactor)
				return;
			CurrentScaleFactor.Value = newScale;
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
		/// Disposes all disposables
		/// </summary>
		protected virtual void OnDisable() {
			IsSpawningEnabled = false;

			removeAllDelayDisposable.Dispose();
			startSpawnDelayDisposable.Dispose();
			startSpawnDisposable.Dispose();
			respawnDelayDisposable.Dispose();
			respawnDisposable.Dispose();

			foreach (var entry in objectDestroyDisposables)
				entry.Value.Dispose();
			objectDestroyDisposables.Clear();
		}

	}

}