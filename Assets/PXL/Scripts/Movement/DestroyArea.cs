using UnityEngine;

public class DestroyArea : TargetArea {

	/**
	* This area destroys the other object if it is a cube
	*/
	protected override void HandleValidOther(Collider other) {
		other.TryGetComponent<ObjectBehaviour>().DestroyObject();
	}
}
