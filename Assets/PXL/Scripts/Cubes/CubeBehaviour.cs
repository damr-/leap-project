using UnityEngine;
using UniRx;

public class CubeBehaviour : MonoBehaviour {
	
	/**
	* At what distance from the origin the cube despawns
	*/
	public float despawnDistance = 1f;

	/**
	* The GameObject to spawn when the cube is destroyed
	*/
	public GameObject destroyEffectObject;
	
	private ISubject<CubeBehaviour> destroySubject = new Subject<CubeBehaviour>();
	public IObservable<CubeBehaviour> CubeDestroyed {
		get {
			return destroySubject;
		}
	}

	/**
	* Check whether the distance to the origin is big enough to be destroyed
	*/
	private void Update() {
		if (Vector3.Distance(Vector3.zero, transform.position) > despawnDistance) {
			DestroyCube();
		}
	}

	/**
	* Updates the Subject, spawns the death effect GameObject and despawns itself
	*/
	public void DestroyCube() {
		destroySubject.OnNext(this);
		GameObject particleSystemObject = SimplePool.Spawn(destroyEffectObject, transform.position, Quaternion.identity);
		ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
        particleSystem.Play();
		SimplePool.Despawn(gameObject);
	}
}
