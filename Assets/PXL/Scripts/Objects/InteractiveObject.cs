using UnityEngine;

namespace PXL.Objects {

	/// <summary>
	/// All available object types 
	/// </summary>
	public enum ObjectType {
		All = 0,
		Cube = 1,
		Sphere = 2,
		Cylinder = 3,
		Capsule = 4,
		Pyramid = 5,
		Ring = 6,
		Cup = 7,
		Bottle = 8,
		Weapon = 9,
	}

	/// <summary>
	/// This script provides information about what type of object this one is and what scale it currently has.
	/// </summary>
	public class InteractiveObject : MonoBehaviour {

		/// <summary>
		/// The ObjectType of this object
		/// </summary>
		public ObjectType ObjectType;

		/// <summary>
		/// The scale of this object
		/// </summary>
		public float Scale { get; set; }

	}

}