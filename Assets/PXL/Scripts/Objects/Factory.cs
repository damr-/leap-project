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
		/// Spawns an instance of <see cref="Prefab"/> at <see cref="Position"/>
		/// </summary>
		public virtual GameObject Spawn() {
			return SimplePool.Spawn(Prefab, Position, Quaternion.identity);
		}

		/// <summary>
		/// Spawns an instance of the given <see cref="GameObject"/> at the given position.
		/// Does not use <see cref="Prefab"/> and <see cref="Position"/>.
		/// </summary>
		public virtual GameObject Spawn(GameObject prefab, Vector3 position) {
			return SimplePool.Spawn(prefab, position, Quaternion.identity);
		}
	}

}