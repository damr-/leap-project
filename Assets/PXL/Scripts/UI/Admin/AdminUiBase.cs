﻿using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.Admin {

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
		/// Shifts through all usermodes
		/// </summary>
		public static void ToggleMode() {
			Mode = (UserMode) ((int) ++Mode % Enum.GetNames(typeof(UserMode)).Length);
			//Mode =  Mode == UserMode.Admin ? UserMode.Rehabilitee : UserMode.Admin;
		}

	}

}