using UnityEngine;
using System.Collections.Generic;
using PXL.Objects;
using UniRx;
using PXL.Utility;
using PXL.UI;

namespace PXL.Objects {

	public class ObjectManager : MonoBehaviour {

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
		public float MinScaleFactor = 0.2f;
		
		/// <summary>
		/// Maximum possible scale of the object
		/// </summary>
		public float MaxScaleFactor = 2f;

		/// <summary>
		/// The default scale of the object
		/// </summary>
		public float DefaultScaleFactor = 1f;

		/// <summary>
		/// How much the scale changes when de-/increasing
		/// </summary>
		public float ScaleChangeAmount = 0.1f;

		/// <summary>
		/// The current set scale for objects
		/// </summary>
		private float currentScaleFactor;

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

			currentObjectPrefab = DefaultObjectPrefab;
			SetObjectScale(DefaultScaleFactor);

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
		/// Increases the object scale of future objects by <see cref="ScaleChangeAmount"/>
		/// </summary>
		public void IncreaseObjectScale() {
			SetObjectScale(currentScaleFactor + ScaleChangeAmount);
		}

		/// <summary>
		/// Decreases the object scale of future objects by <see cref="ScaleChangeAmount"/>
		/// </summary>
		public void DecreaseObjectScale() {
			SetObjectScale(currentScaleFactor - ScaleChangeAmount);
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
			currentScaleFactor = newScale;
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