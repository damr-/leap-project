using UnityEngine;
using UniRx;

public class CubeBehaviour : MonoBehaviour {

	public float despawnDistance = 1f;

	public GameObject destroyEffectObject;
	
	private ISubject<CubeBehaviour> destroySubject = new Subject<CubeBehaviour>();
	public IObservable<CubeBehaviour> CubeDestroyed {
		get {
			return destroySubject;
		}
	}

	private void Update() {
		if (Vector3.Distance(Vector3.zero, transform.position) > despawnDistance) {
			DestroyCube();
		}
	}

	public void DestroyCube() {
		destroySubject.OnNext(this);
		GameObject particleSystemObject = SimplePool.Spawn(destroyEffectObject, transform.position, Quaternion.identity);
		ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
        particleSystem.Play();
		SimplePool.Despawn(gameObject);
	}
}
