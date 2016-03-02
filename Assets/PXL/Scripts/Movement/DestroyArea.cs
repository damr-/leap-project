using UnityEngine;

public class DestroyArea : TargetArea {
	protected override void HandleValidOther(Collider other) {
		other.GetComponent<CubeBehaviour>().DestroyCube();
	}
}
