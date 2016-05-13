using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Spawner {

	public class ToggleSpawnOnRotation : MonoBehaviour {

		/// <summary>
		/// The rotation this object has to have for the spawner to be enabled
		/// </summary>
		public Vector3 TargetDirection;

		/// <summary>
		/// The ObjectSpawner component which will be toggled
		/// </summary>
		public AutomatedObjectSpawner ObjectSpawner;

		public float MinSpawnRate = 0.25f;

		public float MaxSpawnRate = 10f;

		private void Start() {
			for (var i = 0; i < 3; i++)
				TargetDirection[i] = Mathf.Clamp((int)TargetDirection[i], -1, 1);

			ObjectSpawner.AssertNotNull("Missing spawner reference!");
		}

		private void Update() {
			var dot = Vector3.Dot(transform.up, TargetDirection);
			var newEnabledState = dot > 0;

			if (ObjectSpawner.enabled != newEnabledState)
				ObjectSpawner.enabled = newEnabledState;

			if (ObjectSpawner.enabled)
				ObjectSpawner.OverrideSpawnFrequency(dot.Remap(0f, 1f, MinSpawnRate, MaxSpawnRate));
		}

	}

}