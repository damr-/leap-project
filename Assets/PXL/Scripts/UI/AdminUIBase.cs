using PXL.Objects;
using PXL.Utility;
using UnityEngine;

namespace PXL.UI {

	/// <summary>
	/// All existing modes
	/// </summary>
	public enum UserMode {
		Admin = 0,
		Rehabilitee = 1
	}

	public abstract class AdminUiBase : MonoBehaviour {
		/// <summary>
		/// The currently active mode
		/// </summary>
		public static UserMode Mode { get; set; }
		
		/// <summary>
		/// Returns whether the admin mode is active
		/// </summary>
		public static bool IsAdmin { get { return Mode == UserMode.Admin; } }
		
		/// <summary>
		/// The referenced ObjectManager
		/// </summary>
		public ObjectManager ObjectManager;

		protected virtual void Start() {
			ObjectManager.AssertNotNull();
			Mode = UserMode.Admin;
		}
		
		/// <summary>
		/// Toggles between admin and rehabilitee mode
		/// </summary>
		public static void ToggleMode() {
			Mode = Mode == UserMode.Admin ? UserMode.Rehabilitee : UserMode.Admin;
		}
	}

}
