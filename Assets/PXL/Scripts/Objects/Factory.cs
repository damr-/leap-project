using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public abstract class Factory {
	
		/// <summary>
		/// The GameObject to spawn
		/// </summary>
		public GameObject prefab { get; set; }

		/// <summary>
		/// The position of the spawned objects
		/// </summary>
		public Vector3 position { get; set; }
		
		/// <summary>
		/// Spawns an isntance of <see cref="prefab"/> at <see cref="position"/>
		/// </summary>
		public virtual GameObject Spawn() {
			prefab.AssertNotNull();
			return SimplePool.Spawn(prefab, position, Quaternion.identity);
		}
	}

}