using UnityEngine;

namespace PXL.Utility {

	[RequireComponent(typeof(Light))]
	public class OscillateLightIntensity : MonoBehaviour {

		/// <summary>
		/// The speed of the oscillation
		/// </summary>
		public float oscillateSpeed = 1.5f;

		/// <summary>
		/// The minimum intensity when oscillating
		/// </summary>
		public float minIntensity = 4f;

		/// <summary>
		/// The maximum intensity when oscillating
		/// </summary>
		public float maxIntensity = 6f;

		/// <summary>
		/// At what distance from min/max Intensity the direction of the oscillation already turns around.
		/// Used to prevent comparing two float variables directly
		/// </summary>
		public float changeMargin = 0.1f;

		/// <summary>
		/// Whether the light is currently oscillating.
		/// </summary>
		public bool oscillating;

		/// <summary>
		/// The current intensity target the light is oscillating towards
		/// </summary>
		private float targetIntensity;

		/// <summary>
		/// The light component of this object
		/// </summary>
		new private Light light;

		private void Awake() {
			light = this.TryGetComponent<Light>();
			targetIntensity = maxIntensity;
			light.intensity = minIntensity;
		}

		private void Update() {
			if (oscillating) {
				light.intensity = Mathf.Lerp(light.intensity, targetIntensity, oscillateSpeed * Time.deltaTime);
				UpdateTargetIntensity();
			}
		}
		
		void UpdateTargetIntensity() {
			if (Mathf.Abs(targetIntensity - light.intensity) < changeMargin) {
				targetIntensity = (targetIntensity == maxIntensity) ? minIntensity : maxIntensity;
			}
		}
		/// <summary>
		/// Start oscillating towards <see cref="maxIntensity"/>
		/// </summary>
		public void StartOscillating() {
			oscillating = true;
			targetIntensity = maxIntensity;
		}

		/// <summary>
		/// Stop oscillating
		/// </summary>
		public void StopOscillating() {
			oscillating = false;
		}
	}

}