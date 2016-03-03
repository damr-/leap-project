using UnityEngine;
using UniRx;

public class ObjectBehaviour : MonoBehaviour {
	/**
	* At what distance from the origin the object despawns
	*/
	public float despawnDistance = 1f;

	/**
	* The GameObject to spawn when the object is destroyed
	*/
	public GameObject destroyEffectObject;

	private ISubject<ObjectBehaviour> destroySubject = new Subject<ObjectBehaviour>();
	public IObservable<ObjectBehaviour> ObjectDestroyed {
		get {
			return destroySubject;
		}
	}

	/**
	* Check whether the distance to the origin is big enough to be destroyed
	*/
	private void Update() {
		if (Vector3.Distance(Vector3.zero, transform.position) > despawnDistance) {
			DestroyObject();
		}
	}

	/**
	* Updates the Subject, spawns the death effect GameObject and despawns itself
	*/
	public void DestroyObject() {
		destroySubject.OnNext(this);
		GameObject particleSystemObject = SimplePool.Spawn(destroyEffectObject, transform.position, Quaternion.identity);
		ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
		particleSystem.Play();
		SimplePool.Despawn(gameObject);
	}
}