using UnityEngine;

public class ObjectFactory : Factory {

	/**
	* Scale of the object 
	*/
	public float scale { get; set; }

	/**
	* Spawns the set prefab at position with the defined scale and resets velocity
	*/
	public override GameObject Spawn() {
		GameObject newObject = base.Spawn();
		newObject.transform.localScale = new Vector3(scale, scale, scale);
		newObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		return newObject;
	}
}
