using UnityEngine;

namespace PXL.Utility {

	[RequireComponent(typeof(Light))]
	public class OscillateLightIntensity : MonoBehaviour {

		/// <summary>
		/// The speed of the oscillation
		/// </summary>
		public float OscillateSpeed = 1.5f;

		/// <summary>
		/// The minimum intensity when oscillating
		/// </summary>
		public float MinIntensity = 4f;

		/// <summary>
		/// The maximum intensity when oscillating
		/// </summary>
		public float MaxIntensity = 6f;

		/// <summary>
		/// At what distance from min/max Intensity the direction of the oscillation already turns around.
		/// Used to prevent comparing two float variables directly
		/// </summary>
		public float ChangeMargin = 0.1f;

		/// <summary>
		/// Whether the light is currently oscillating.
		/// </summary>
		public bool Oscillating;

		/// <summary>
		/// The current intensity target the light is oscillating towards
		/// </summary>
		private float targetIntensity;

		/// <summary>
		/// Whether the intensity is currently de- or increasing
		/// </summary>
		private bool increasing = true;

		/// <summary>
		/// The light component of this object
		/// </summary>
		private new Light light;

		private void Awake() {
			light = this.TryGetComponent<Light>();
			targetIntensity = MaxIntensity;
			light.intensity = MinIntensity;
		}

		private void Update() {
			if (!Oscillating)
				return;

			light.intensity = Mathf.Lerp(light.intensity, targetIntensity, OscillateSpeed * UnityEngine.Time.deltaTime);
			UpdateTargetIntensity();
		}

		private void UpdateTargetIntensity() {
			if (Mathf.Abs(targetIntensity - light.intensity) >= ChangeMargin)
				return;
			
			targetIntensity = increasing ? MinIntensity : MaxIntensity;
			increasing = !increasing;
		}
		/// <summary>
		/// Start oscillating towards <see cref="MaxIntensity"/>
		/// </summary>
		public void StartOscillating() {
			Oscillating = true;
			targetIntensity = MaxIntensity;
		}

		/// <summary>
		/// Stop oscillating
		/// </summary>
		public void StopOscillating() {
			Oscillating = false;
		}
	}

}