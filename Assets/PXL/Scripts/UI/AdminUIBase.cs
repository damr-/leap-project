using UnityEngine;

public abstract class AdminUIBase : MonoBehaviour {

	public enum Mode {
		ADMIN = 0,
		REHABILITEE = 1
	}

	/**
	* The currently activem mode
	*/
	public static Mode mode { get; set; }

	/**
	* Returns true if admin mode is active
	*/
	public static bool IsAdmin { get { return mode == Mode.ADMIN; } }

	/**
	* Reference to the desired manager
	*/
	public ObjectManager objectManager;

	protected virtual void Start() {
		if (objectManager == null)
			throw new MissingReferenceException("No manager set!");

		mode = Mode.ADMIN;
	}

	/**
	* Toggles between admin and rehabilitee mode
	*/
	public static void ToggleMode() {
		mode = (mode == Mode.ADMIN) ? Mode.REHABILITEE : Mode.ADMIN;
	}
}
