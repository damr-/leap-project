using UnityEngine;

public class Flickering : MonoBehaviour {

	private float initialIntensity;
	private const float f = 0.1666f;
	private Light l;

	private void Start() {
		l = GetComponent<Light>();
		initialIntensity = l.intensity;
	}
	
	private void Update() {
		l.intensity = initialIntensity + Mathf.Sin(f * (1 - Mathf.Sin(f * 25f)) * 3f) * initialIntensity / 5f;
	}

}