using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public abstract class Factory {
	
		/// <summary>
		/// The GameObject to spawn
		/// </summary>
		public GameObject Prefab { get; set; }

		/// <summary>
		/// The position of the spawned objects
		/// </summary>
		public Vector3 Position { get; set; }
		
		/// <summary>
		/// Spawns an isntance of <see cref="Prefab"/> at <see cref="Position"/>
		/// </summary>
		public virtual GameObject Spawn() {
			Prefab.AssertNotNull();
			return SimplePool.Spawn(Prefab, Position, Quaternion.identity);
		}
	}

}