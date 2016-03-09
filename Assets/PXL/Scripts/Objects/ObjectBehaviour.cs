using UnityEngine;
using UniRx;
using PXL.Utility;

namespace PXL.Objects {

	public enum ObjectType {
		All = 0,
		Cube = 1,
		Sphere = 2,
		Cylinder = 3,
		Capsule = 4,
		Pyramid = 5,
		Ring = 6,
	}

	public class ObjectBehaviour : MonoBehaviour {
		/// <summary>
		/// At what distance from the origin the object despawns
		/// </summary>
		public float DespawnDistance = 2f;

		/// <summary>
		/// The ObjectType of this object
		/// </summary>
		public ObjectType ObjectType;

		/// <summary>
		/// The scale of this object
		/// </summary>
		public float Scale { get; set; }

		/// <summary>
		/// The GameObject to spawn when the object is destroyed
		/// </summary>
		public GameObject DestroyEffectObject;

		private readonly ISubject<ObjectBehaviour> destroySubject = new Subject<ObjectBehaviour>();
		public IObservable<ObjectBehaviour> ObjectDestroyed {
			get {
				return destroySubject;
			}
		}

		private void Start() {
			DestroyEffectObject.AssertNotNull();
		}

		private void Update() {
			if (Vector3.Distance(Vector3.zero, transform.position) > DespawnDistance) {
				DestroyObject();
			}
		}

		/// <summary>
		/// Updates the Subject, spawns the death effect GameObject and despawns itself
		/// </summary>
		public void DestroyObject() {
			destroySubject.OnNext(this);
			var particleSystemObject = SimplePool.Spawn(DestroyEffectObject, transform.position, Quaternion.identity);
			var particleComponent = particleSystemObject.GetComponent<ParticleSystem>();
			particleComponent.Play();
			SimplePool.Despawn(gameObject);
		}
	}

}