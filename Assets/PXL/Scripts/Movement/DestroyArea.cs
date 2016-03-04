using UnityEngine;

public class DestroyArea : TargetArea {

	/**
	* This area destroys the other object if it is one
	*/
	protected override void HandleValidOther(Collider other) {
		ObjectBehaviour objectBehaviour = other.TryGetComponent<ObjectBehaviour>();
		if (objectBehaviour == null) {
			Debug.LogWarning("GameObject '" + other.gameObject.name + "' has tag '" + targetTag + "' but no component ObjectBehaviour!");
		}
		else {
			objectBehaviour.DestroyObject();
		}
	}
}
