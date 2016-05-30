using UnityEngine;

namespace PXL.Utility {

	/// <summary>
	/// This script oscillates the intensity of the object's <see cref="Light"/> component over time with a certain speed.
	/// 
	/// It oscillates between a minimum and maximum intensity and the process can be started/stopped from the outside via functions.
	/// </summary>
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
		private Light lightComponent;

		private void Awake() {
			lightComponent = this.TryGetComponent<Light>();
			targetIntensity = MaxIntensity;
			lightComponent.intensity = MinIntensity;
		}

		private void Update() {
			if (!Oscillating)
				return;

			lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, targetIntensity, OscillateSpeed * UnityEngine.Time.deltaTime);
			UpdateTargetIntensity();
		}

		private void UpdateTargetIntensity() {
			if (Mathf.Abs(targetIntensity - lightComponent.intensity) >= ChangeMargin)
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