using UnityEngine;
using UniRx;
using PXL.Utility;

namespace PXL.Objects {

	public class ObjectBehaviour : MonoBehaviour {
		/// <summary>
		/// At what distance from the origin the object despawns
		/// </summary>
		public float despawnDistance = 1f;
		
		/// <summary>
		/// The GameObject to spawn when the object is destroyed
		/// </summary>
		public GameObject destroyEffectObject;

		private ISubject<ObjectBehaviour> destroySubject = new Subject<ObjectBehaviour>();
		public IObservable<ObjectBehaviour> ObjectDestroyed {
			get {
				return destroySubject;
			}
		}

		/// <summary>
		/// Check whether the distance to the origin is big enough to be destroyed
		/// </summary>
		private void Update() {
			if (Vector3.Distance(Vector3.zero, transform.position) > despawnDistance) {
				DestroyObject();
			}
		}

		/// <summary>
		/// Updates the Subject, spawns the death effect GameObject and despawns itself
		/// </summary>
		public void DestroyObject() {
			destroySubject.OnNext(this);
			GameObject particleSystemObject = SimplePool.Spawn(destroyEffectObject, transform.position, Quaternion.identity);
			ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
			particleSystem.Play();
			SimplePool.Despawn(gameObject);
		}
	}

}