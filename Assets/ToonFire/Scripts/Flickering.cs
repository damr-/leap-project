using UnityEngine;

public class Flickering : MonoBehaviour {

	private float initialIntensity;
	private const float F = 0.1666f;
	private Light l;
	private float sum;
	private bool add = true;
	private float max = 1.12f;
	private float w = 3f;
	private float e = 3f;

	private void Start() {
		l = GetComponent<Light>();
		initialIntensity = l.intensity;
	}

	private void Update() {
		if (add) {
			sum += Time.deltaTime;
			if (sum > max) {
				add = false;
			}
		}
		else {
			sum -= Time.deltaTime;
			if (sum <= 0f) {
				add = true;
				max = Random.Range(0.8f, 1.4f);
				w = Random.Range(2f, 3.5f);
				e = Random.Range(2.5f, 4f);
			}
		}

		var v = Mathf.Sin(sum * (1 - Mathf.Sin(sum * w)) * e) * initialIntensity / 5f;
		l.intensity = initialIntensity + v;
	}

}