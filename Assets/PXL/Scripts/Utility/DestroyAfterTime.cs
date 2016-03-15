using UnityEngine;

namespace PXL.Utility {

	public class DestroyAfterTime : MonoBehaviour {
	
		/// <summary>
		/// After how many seconds this object will be despawned
		/// </summary>
		public float LifeTime = 1f;
		
		/// <summary>
		/// What time the object began to live
		/// </summary>
		private float startTime;
		
		private void OnEnable() {
			startTime = UnityEngine.Time.time;
		}

		private void Update() {
			if (UnityEngine.Time.time - startTime > LifeTime)
				SimplePool.Despawn(gameObject);
		}

	}

}