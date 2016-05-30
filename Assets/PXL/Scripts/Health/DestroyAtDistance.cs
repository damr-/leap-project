using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	/// <summary>
	/// This script takes care of despawning or destroying this object when
	/// it's too far away from a certain position or transform.
	/// If it checks the distance to a transform, changes in the transform's position on runtime are also taken into account.
	/// </summary>
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
		/// Whether this object should be despawned and not destroyed
		/// </summary>
		public bool Despawn = true;

		private void Update() {
			var origin = OriginTransform == null ? Origin : OriginTransform.position;
			if (Vector3.Distance(origin, transform.position) > DespawnDistance)
				HandleTooFarAway();
		}

		/// <summary>
		/// Called when the <see cref="Health"/> component invokes the Death Observable
		/// </summary>
		private void HandleTooFarAway() {
			if (Despawn)
				SimplePool.Despawn(gameObject);
			else
				Destroy(gameObject);
		}

	}

}