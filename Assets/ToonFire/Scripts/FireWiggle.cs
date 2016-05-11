using PXL.Utility;
using UnityEngine;

public class FireWiggle : MonoBehaviour {

	private float elapsedTime;
	private float wiggleTime;
	private float initialStartSpeed;
	private float initialEmissionRate;
	private float initialLifetime;
	private float initialSize;

	private ParticleSystem ps;

	private void Start() {
		ps = this.TryGetComponent<ParticleSystem>();

		initialStartSpeed = ps.startSpeed;
		initialEmissionRate = ps.GetEmissionRate();
		initialLifetime = ps.startLifetime;
		initialSize = ps.startSize;
	}

	private void Update() {
		elapsedTime += Time.deltaTime;
		wiggleTime += Time.deltaTime;


		if (elapsedTime > 2f + (2f - Mathf.Sin(wiggleTime))) {
			ps.SetEmissionRate(ps.GetEmissionRate() + (initialEmissionRate * 0.4f - ps.GetEmissionRate()) / 30f);
			ps.startLifetime += (initialLifetime * 0.9f - ps.startLifetime) / 30f;

			if (!(ps.GetEmissionRate() < initialEmissionRate * .42f)) 
				return;

			ps.SetEmissionRate(initialEmissionRate * 1.1f);
			ps.startLifetime = initialLifetime * 1.1f;
			ps.startSpeed = initialStartSpeed * 0.7f;
			ps.startSize = initialSize * 1.1f;
			elapsedTime = 0f;
		}
		else {
			ps.SetEmissionRate(ps.GetEmissionRate() + (initialEmissionRate - ps.GetEmissionRate()) / 30f);
			ps.startLifetime += (initialLifetime - ps.startLifetime) / 30f;
			ps.startSpeed += (initialStartSpeed - ps.startSpeed) / 30f;
			ps.startSize += (initialSize - ps.startSize) / 30f;
		}
	}

}