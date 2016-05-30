using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	/// <summary>
	/// This Factory provides basic functionality for spawning an object from a given prefab at a position with a certain rotation.
	/// </summary>
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
		/// The rotation of the spawned object
		/// </summary>
		public Vector3 Rotation { get; set; }

		/// <summary>
		/// Spawns an instance of <see cref="Prefab"/> at <see cref="Position"/> with <see cref="Rotation"/>
		/// </summary>
		public virtual GameObject Spawn() {
			return SimplePool.Spawn(Prefab, Position, Quaternion.Euler(Rotation));
		}

	}

}