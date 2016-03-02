using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class CubeSpawner : MonoBehaviour {

	public KeyCode spawnKey = KeyCode.X;
	public KeyCode removeAllKey = KeyCode.C;

	public GameObject cubePrefab;

	public Transform spawnPositionObject;

	private Vector3 spawnPosition;

	private List<CubeBehaviour> spawnedCubes = new List<CubeBehaviour>();

	private CompositeDisposable cubeDestroySubscriptions = new CompositeDisposable();

	private bool waitWithSpawn = false;
	private float lastTime;
	private float spawnDelay = 0.5f;

	void Start() {
		spawnPosition = (spawnPositionObject == null) ? transform.position : spawnPositionObject.transform.position;
		SpawnCube();
	}

	void Update() {
		if (Input.GetKeyDown(spawnKey))
			SpawnCube();
		if (Input.GetKeyDown(removeAllKey))
			RemoveAllCubes();

		if (waitWithSpawn && Time.time - lastTime > spawnDelay && spawnedCubes.Count <= 0) {
			waitWithSpawn = false;
			SpawnCube();
		}
    }

	public void CubeDespawned(CubeBehaviour cubeBehaviour) {
		spawnedCubes.Remove(cubeBehaviour);

		if (!waitWithSpawn && spawnedCubes.Count() <= 0) {
			SpawnCube();
		}
	}

	public void SpawnCube() {
		CubeBehaviour newCube = SimplePool.Spawn(cubePrefab, spawnPosition, Quaternion.identity).GetComponent<CubeBehaviour>();
		newCube.CubeDestroyed.Subscribe(c => CubeDespawned(c)).AddTo(cubeDestroySubscriptions);
		spawnedCubes.Add(newCube);
	}

	public void RemoveAllCubes() {
		waitWithSpawn = true;
		lastTime = Time.time;
		while (spawnedCubes.Count > 0) {
			spawnedCubes[0].DestroyCube();
		}
	}

	private void OnDisable() {
		cubeDestroySubscriptions.Dispose();
	}
}
