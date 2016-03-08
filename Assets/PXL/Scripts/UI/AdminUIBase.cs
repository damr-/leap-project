using PXL.Objects;
using PXL.Utility;
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
		/// The referenced ObjectManager
		/// </summary>
		public ObjectManager objectManager;

		protected virtual void Start() {
			objectManager.AssertNotNull();
			mode = Mode.ADMIN;
		}
		
		/// <summary>
		/// Toggles between admin and rehabilitee mode
		/// </summary>
		public static void ToggleMode() {
			mode = (mode == Mode.ADMIN) ? Mode.REHABILITEE : Mode.ADMIN;
		}
	}

}
