using UnityEngine;
using System.Collections;

public abstract class AdminUIBase : MonoBehaviour {

	/**
	* Reference to the desired manager
	*/
	public ObjectManager objectManager;

	protected virtual void Start() {
		if (objectManager == null)
			throw new MissingReferenceException("No manager set!");
	}
}
