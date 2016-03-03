using UnityEngine;

public abstract class Factory : IFactory {

	/**
	* Prefab to spawn
	*/
	public GameObject prefab { get; set; }

	/**
	* Position of the spawned object
	*/
	public Vector3 position { get; set; }

	/**
	* Spawns the object at the position
	*/
	public virtual GameObject Spawn() {
		if (prefab == null)
			throw new MissingReferenceException("No prefab assigned for spawning!");

		return SimplePool.Spawn(prefab, position, Quaternion.identity);
	}
}
