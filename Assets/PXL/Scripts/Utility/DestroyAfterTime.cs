using UnityEngine;

namespace PXL.Utility {

	public class DestroyAfterTime : MonoBehaviour {
	
		/// <summary>
		/// After how many seconds this object will be despawned
		/// </summary>
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

}