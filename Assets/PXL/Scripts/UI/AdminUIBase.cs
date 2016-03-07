using PXL.Objects;
using UnityEngine;

namespace PXL.UI {

	public abstract class AdminUIBase : MonoBehaviour {

		/// <summary>
		/// All existing modes
		/// </summary>
		public enum Mode {
			ADMIN = 0,
			REHABILITEE = 1
		}
		
		/// <summary>
		/// The currently active mode
		/// </summary>
		public static Mode mode { get; set; }
		
		/// <summary>
		/// Returns whether the admin mode is active
		/// </summary>
		public static bool IsAdmin { get { return mode == Mode.ADMIN; } }
		
		/// <summary>
		/// The used ObjectManager
		/// </summary>
		public ObjectManager objectManager;

		protected virtual void Start() {
			if (objectManager == null)
				throw new MissingReferenceException("No manager set!");

			mode = Mode.ADMIN;
		}
		
		/// <summary>
		/// TOggles between admin and rehavilitee mode
		/// </summary>
		public static void ToggleMode() {
			mode = (mode == Mode.ADMIN) ? Mode.REHABILITEE : Mode.ADMIN;
		}
	}

}
