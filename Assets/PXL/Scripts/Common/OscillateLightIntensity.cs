using UnityEngine;

[RequireComponent(typeof(Light))]
public class OscillateLightIntensity : MonoBehaviour {

	public float oscillateSpeed = 1.5f;
	public float minIntensity = 4f;
	public float maxIntensity = 6f;
	public float changeMargin = 0.1f;

	public bool oscillating;

	private float targetIntensity;

	new private Light light;

	private void Awake() {
		light = GetComponent<Light>();
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

	public void StartOscillating() {
		oscillating = true;
		targetIntensity = maxIntensity;
	}

	public void StopOscillating() {
		oscillating = false;
		targetIntensity = minIntensity;
	}
}
