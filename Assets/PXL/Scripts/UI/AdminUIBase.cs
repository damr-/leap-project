using System.Collections.Generic;
using System.Linq;
using PXL.Objects;
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
		/// ALl ObjectManagers in this scene
		/// </summary>
		protected List<ObjectManager> ObjectManager = new List<ObjectManager>();

		protected virtual void Start() {
			ObjectManager = FindObjectsOfType<ObjectManager>().ToList();
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
