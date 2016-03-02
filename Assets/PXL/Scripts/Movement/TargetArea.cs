using UnityEngine;

public abstract class TargetArea : MonoBehaviour {

	public Tags.TagType targetTagType;

	private string targetTag;

	private void Awake() {
		targetTag = Tags.getTagString(targetTagType);
    }
	
	private void OnTriggerEnter(Collider other) {
		HandleTriggerEntered(other);
	}

	protected virtual void HandleTriggerEntered(Collider other) {
		if (other.gameObject.CompareTag(targetTag)) {
			HandleValidOther(other);
		}
	}

	protected abstract void HandleValidOther(Collider other);
}
