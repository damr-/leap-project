using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class CubeManager : MonoBehaviour {

	public KeyCode spawnKey = KeyCode.X;
	public KeyCode removeAllKey = KeyCode.C;

	/**
	* The cube GameObject which will be spawned.
	*/
	public GameObject cubePrefab;

	/**
	* The Transform where the cubes will be spawned
	* If it's missing, the position of the spaner will be taken
	*/
	public Transform spawnPositionObject;

	/**
	* Minimum possible scale of the cube
	*/
	public float minCubeScale = 0.025f;

	/**
	* Maximum possible scale of the cube
	*/
	public float maxCubeScale = 0.1f;

	/**
	* The default scale of the cube
	*/
	public float defaultCubeScale = 0.05f;

	private Vector3 spawnPosition;
	private List<CubeBehaviour> spawnedCubes = new List<CubeBehaviour>();
	private CompositeDisposable cubeDestroySubscriptions = new CompositeDisposable();

	/**
	* Spawn delay when removing all cubes
	*/
	private bool delayedSpawn = false;
	private float delayStartTime;
	private float spawnDelay = 0.5f;

	public ObservableProperty<float> CubeScale { get { return cubeScale; } }
	private ObservableProperty<float> cubeScale = new ObservableProperty<float>();

	public ObservableProperty<Color> CubeColor { get { return cubeColor; } }
	private ObservableProperty<Color> cubeColor = new ObservableProperty<Color>();

	private CubeFactory cubeFactory = new CubeFactory();

	/**
	* Setup the spawn position and spawn the first cube
	*/
	void Start() {
		spawnPosition = (spawnPositionObject == null) ? transform.position : spawnPositionObject.transform.position;
		cubeFactory.prefab = cubePrefab;
		cubeFactory.position = spawnPosition;
		SetCubeScale(defaultCubeScale.Remap(minCubeScale, maxCubeScale, 0, 1));

		SpawnCube();
	}

	/**
	* Checks for keyboard input and executes a delayed spawn if necessary
	*/
	void Update() {
		if (Input.GetKeyDown(spawnKey))
			SpawnCube();
		if (Input.GetKeyDown(removeAllKey))
			RemoveAllCubes();

		if (delayedSpawn && Time.time - delayStartTime > spawnDelay && spawnedCubes.Count <= 0) {
			delayedSpawn = false;
			SpawnCube();
		}
	}

	/**
	* Called when a cube was destroyed
	* Removes it from the list and spawns one if possible and necessary
	*/
	private void CubeDespawned(CubeBehaviour cubeBehaviour) {
		spawnedCubes.Remove(cubeBehaviour);

		if (!delayedSpawn && spawnedCubes.Count() <= 0) {
			SpawnCube();
		}
	}

	/**
	* Removes all existing cubes
	*/
	public void RemoveAllCubes() {
		delayedSpawn = true;
		delayStartTime = Time.time;
		while (spawnedCubes.Count > 0) {
			spawnedCubes[0].DestroyCube();
		}
	}

	/**
	* Tells the factory to spawn a cube
	*/
	private void SpawnCube() {
		GameObject newCube = cubeFactory.Spawn();

		CubeBehaviour cubeBehaviour = newCube.GetComponent<CubeBehaviour>();
		cubeBehaviour.CubeDestroyed.Subscribe(c => CubeDespawned(c)).AddTo(cubeDestroySubscriptions);

		spawnedCubes.Add(cubeBehaviour);
	}

	/**
	* Sets the scale the cubes get applied when spawned, from 0 to 1
	* The number is mapped to the range minCubeScale to maxCubeScale
	*/
	public void SetCubeScale(float newScale) {
		newScale = newScale.Remap(0, 1, minCubeScale, maxCubeScale);
		CubeScale.Value = newScale;
		cubeFactory.scale = newScale;
	}

	/**
	* Sets the color cubes will have applied
	*/
	public void SetCubeColor(int menuIndex) {
		Color newColor = CubeColorPanel.availableColors.ElementAt(menuIndex).Key;

		CubeColor.Value = newColor;
		cubeFactory.color = newColor;
	}
}
