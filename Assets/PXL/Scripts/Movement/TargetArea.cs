using UnityEngine;

public abstract class TargetArea : MonoBehaviour {
	/**
	* Only GameObjects with this tag will be destroyed
	*/
	public Tags.TagType targetTagType;

	/**
	* The actual tag as string
	*/
	protected string targetTag;

	/**
	* Retreive the tag as string from the enum
	*/
	protected virtual void Awake() {
		targetTag = Tags.getTagString(targetTagType);
    }

	protected virtual void OnTriggerEnter(Collider other) {
		HandleTriggerEntered(other);
	}

	protected virtual void HandleTriggerEntered(Collider other) {
		if (other.gameObject.CompareTag(targetTag)) {
			HandleValidOther(other);
		}
	}

	/**
	* Abstract method so that different areas handle successful overlaps differently
	*/
	protected abstract void HandleValidOther(Collider other);
}
