using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class ObjectManager : MonoBehaviour {

	public KeyCode spawnKey = KeyCode.X;
	public KeyCode removeAllKey = KeyCode.C;

	/**
	* The GameObject which will be spawned.
	*/
	public GameObject defaultObjectPrefab;

	/**
	* The Transform where the objects will be spawned
	* If it's missing, the position of the spaner will be taken
	*/
	public Transform spawnPositionObject;

	/**
	* The amount of objects spawned at the start
	*/
	public int startAmount = 2;

	/**
	* Minimum possible scale of the object
	*/
	public float minScale = 0.025f;

	/**
	* Maximum possible scale of the object
	*/
	public float maxScale = 0.1f;

	/**
	* The default scale of the object
	*/
	public float defaultScale = 0.05f;

	private Vector3 spawnPosition;
	private List<ObjectBehaviour> spawnedObjects = new List<ObjectBehaviour>();
	private CompositeDisposable objectDestroySubscriptions = new CompositeDisposable();

	/**
	* Spawn delay when removing all objects
	*/
	private bool delayedSpawn = false;
	private float delayStartTime;
	private float spawnDelay = 0.5f;

	/**
	* The currently active prefab
	*/
	private GameObject currentObjectPrefab;

	public ObservableProperty<float> ObjectScale { get { return objectScale; } }
	private ObservableProperty<float> objectScale = new ObservableProperty<float>();

	public IObservable<ObjectBehaviour> ObjectSpawned { get { return objectSpawnedSubject; } }
	private ISubject<ObjectBehaviour> objectSpawnedSubject = new Subject<ObjectBehaviour>();

	public IObservable<Unit> SpawnInitiated { get { return spawnInitiatedSubject; } }
	private ISubject<Unit> spawnInitiatedSubject = new Subject<Unit>();

	private ObjectFactory objectFactory = new ObjectFactory();

	/**
	* Setup the spawn position and spawn the first object
	*/
	void Start() {
		spawnPosition = (spawnPositionObject == null) ? transform.position : spawnPositionObject.transform.position;
		objectFactory.prefab = defaultObjectPrefab;
		objectFactory.position = spawnPosition;
		SetObjectScale(defaultScale.Remap(minScale, maxScale, 0, 1));
		currentObjectPrefab = defaultObjectPrefab;

		for(int i = 0; i < startAmount; i++)
			SpawnObject();
	}

	/**
	* Checks for keyboard input and executes a delayed spawn if necessary
	*/
	void Update() {
		if (Input.GetKeyDown(spawnKey))
			SpawnObject();
		if (Input.GetKeyDown(removeAllKey))
			RemoveAllObjects();

		if (delayedSpawn && Time.time - delayStartTime > spawnDelay && spawnedObjects.Count <= 0) {
			delayedSpawn = false;
			SpawnObject();
		}
	}

	/**
	* Called when a object was destroyed
	* Removes it from the list and spawns one if possible and necessary
	*/
	private void ObjectDespawned(ObjectBehaviour objectBehaviour) {
		spawnedObjects.Remove(objectBehaviour);

		if (!delayedSpawn && spawnedObjects.Count() <= 0) {
			SpawnObject();
		}
	}

	/**
	* Removes all existing objects
	*/
	public void RemoveAllObjects() {
		delayedSpawn = true;
		delayStartTime = Time.time;
		while (spawnedObjects.Count > 0) {
			spawnedObjects[0].DestroyObject();
		}
	}

	/**
	* Tells the factory to spawn an object
	*/
	public void SpawnObject() {
		spawnInitiatedSubject.OnNext(Unit.Default);

        objectFactory.prefab = currentObjectPrefab;

		GameObject newObject = objectFactory.Spawn();

		ObjectBehaviour objectBehaviour = newObject.GetComponent<ObjectBehaviour>();
		objectBehaviour.ObjectDestroyed.Subscribe(o => ObjectDespawned(o)).AddTo(objectDestroySubscriptions);

		spawnedObjects.Add(objectBehaviour);

		objectSpawnedSubject.OnNext(objectBehaviour);
	}

	/**
	* Sets the scale the objects get applied when spawned, from 0 to 1
	* The number is mapped to the range minScale to maxScale
	*/
	public void SetObjectScale(float sliderValue) {
		float newScale = sliderValue.Remap(0, 1, minScale, maxScale);
		ObjectScale.Value = newScale;
		objectFactory.scale = newScale;
	}

	/**
	* Sets the objectPrefab which will be spawned
	*/
	public void SetObjectPrefab(GameObject newPrefab) {
		currentObjectPrefab = newPrefab;
	}
}
