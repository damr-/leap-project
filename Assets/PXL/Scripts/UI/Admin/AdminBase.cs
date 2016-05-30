using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;

namespace PXL.UI.Admin {

	/// <summary>
	/// All existing modes
	/// </summary>
	public enum UserMode {
		Admin = 0,
		User = 1
	}

	/// <summary>
	/// The base for any script which needs access to all the ObjectSpawners in the scene
	/// </summary>
	public abstract class AdminBase : MonoBehaviour {

		/// <summary>
		/// The currently active mode
		/// </summary>
		public static ObservableProperty<UserMode> Mode = new ObservableProperty<UserMode>();

		/// <summary>
		/// Returns whether the admin mode is active
		/// </summary>
		public static bool IsAdmin {
			get { return Mode == UserMode.Admin; }
		}

		/// <summary>
		/// ALl ObjectManagers in this scene
		/// </summary>
		protected List<ObjectSpawner> ObjectSpawners = new List<ObjectSpawner>();

		protected virtual void Start() {
			ObjectSpawners = FindObjectsOfType<ObjectSpawner>().ToList();
		}

		/// <summary>
		/// Sets the current mode to the next one in the available <see cref="UserMode"/>
		/// </summary>
		public static void ToggleMode() {
			Mode.Value = (UserMode)((int)++Mode.Value % Enum.GetNames(typeof(UserMode)).Length);
		}

	}

}