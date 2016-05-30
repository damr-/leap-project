using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// This script toggles the active state of an <see cref="Spawner.AutomatedObjectSpawner"/> depending on the rotation of the object.
	///
	/// The more it's tilted, the higher the spawn rate is as well.
	/// 
	/// Currently only supports tilting an object upside down.
	/// </summary>
	public class ToggleSpawnOnRotation : MonoBehaviour {

		/// <summary>
		/// The rotation this object has to have for the spawner to be enabled
		/// </summary>
		public Vector3 TargetDirection;

		/// <summary>
		/// The ObjectSpawner component which will be toggled
		/// </summary>
		public AutomatedObjectSpawner AutomatedObjectSpawner;

		/// <summary>
		/// The minimum spawn rate, used when the object is tilted downwards a little bit.
		/// </summary>
		public float MinSpawnRate = 0.25f;

		/// <summary>
		/// The maximum spawn rate, used when the object is upside down.
		/// </summary>
		public float MaxSpawnRate = 10f;

		private void Start() {
			for (var i = 0; i < 3; i++)
				TargetDirection[i] = Mathf.Clamp((int)TargetDirection[i], -1, 1);

			AutomatedObjectSpawner.AssertNotNull("Missing spawner reference!");
		}

		private void Update() {
			var dot = Vector3.Dot(transform.up, TargetDirection);
			var newEnabledState = dot > 0;

			if (AutomatedObjectSpawner.enabled != newEnabledState)
				AutomatedObjectSpawner.enabled = newEnabledState;

			if (!AutomatedObjectSpawner.enabled)
				return;

			if (!AutomatedObjectSpawner.IsSpawningEnabled)
				AutomatedObjectSpawner.IsSpawningEnabled = true;

			AutomatedObjectSpawner.OverrideSpawnFrequency(dot.Remap(0f, 1f, MinSpawnRate, MaxSpawnRate));
		}

	}

}