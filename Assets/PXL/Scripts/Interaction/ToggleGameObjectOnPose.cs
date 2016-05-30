using UnityEngine;
using System.Collections.Generic;
using PXL.Utility;

namespace PXL.Interaction {

	/// <summary>
	/// This script toggles the active state of the given gameObjects as soon as the observed hands 
	/// maintain the desired pose.
	/// </summary>
	public class ToggleGameObjectOnPose : HandPoseObserver {
	
		/// <summary>
		/// Whether the target objects are currently enabled
		/// </summary>
		private bool objectsEnabled;

		/// <summary>
		/// The Objects which will be de/activated
		/// </summary>
		public List<GameObject> GameObjects = new List<GameObject>();

		protected override void OnEnable() {
			base.OnEnable();
			objectsEnabled = false;
		}

		protected override void Start() {
			GameObjects.ForEach(g => g.AssertNotNull("GameObject reference missing!"));
			base.Start();
		}

		protected override void HandleCorrectPose() {
			EnableObjects(true);
		}

		protected override void HandleIncorrectPose() {
			EnableObjects(false);
		}

		/// <summary>
		/// Set the active state on all <see cref="GameObjects"/> to the given <see cref="newEnabledState"/>.
		/// Only sets it if it differs from their currrent state (<see cref="objectsEnabled"/>)
		/// </summary>
		private void EnableObjects(bool newEnabledState) {
			if (objectsEnabled == newEnabledState)
				return;

			objectsEnabled = newEnabledState;
			GameObjects.ForEach(c => c.SetActive(newEnabledState));
		}

		protected override void OnDisable() {
			base.OnDisable();
			EnableObjects(false);
		}

	}

}