using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public class DestroyAtDistance : MonoBehaviour {

		/// <summary>
		/// At what distance from the origin the object despawns
		/// </summary>
		public float DespawnDistance = 2f;

		/// <summary>
		/// The origin to measure the distance from.
		/// </summary>
		public Vector3 Origin;

		/// <summary>
		/// Optional transform to be used as origin
		/// </summary>
		public Transform OriginTransform;

		/// <summary>
		/// The actual origin position used to calculate the distance
		/// </summary>
		private Vector3 origin;

		/// <summary>
		/// Whether this object should be despawned and not destroyed
		/// </summary>
		public bool Despawn = true;

		private void Start() {
			origin = OriginTransform == null ? Origin : OriginTransform.position;
		}

		private void Update() {
			if (Vector3.Distance(origin, transform.position) > DespawnDistance) {
				HandleTooFarAway();
			}
		}

		/// <summary>
		/// Called when the <see cref="Health"/> component invokes the Death Observable
		/// </summary>
		private void HandleTooFarAway() {
			if (Despawn) {
				SimplePool.Despawn(gameObject);
			}
			else {
				Destroy(gameObject);
			}
		}
	}

}