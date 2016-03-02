using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

	public float lifeTime = 1f;

	private float startTime;

	private void OnEnable() {
		startTime = Time.time;
	}

	private void Update() {
		if (Time.time - startTime > lifeTime)
			SimplePool.Despawn(gameObject);
	}

}
