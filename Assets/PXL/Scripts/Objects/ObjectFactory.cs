using System.Collections.Generic;
using UnityEngine;

namespace PXL.Objects {

	/// <summary>
	/// A factory to spawn an object.
	/// It keeps track of the default scales and resets the object's scale before scaling it up to <see cref="Scale"/>
	/// It also sets the rigidbody's velocity to zero.
	/// </summary>
	public class ObjectFactory : Factory {

		/// <summary>
		/// Scale of the object
		/// </summary>
		public float Scale { get; set; }

		/// <summary>
		/// All the possible objects and their default scale
		/// </summary>
		public readonly IDictionary<ObjectType, Vector3> DefaultObjectScales = new Dictionary<ObjectType, Vector3>();

		/// <summary>
		/// Spawns the set prefab at position with the defined scale and resets velocity
		/// </summary>
		public override GameObject Spawn() {
			var newObject = base.Spawn();
			var interactiveObject = newObject.GetComponentInChildren<InteractiveObject>();
			var objectType = interactiveObject.ObjectType;

			if (!DefaultObjectScales.ContainsKey(objectType))
				DefaultObjectScales.Add(objectType, newObject.transform.localScale);
			else
				newObject.transform.localScale = DefaultObjectScales[objectType];

			var s = newObject.transform.localScale;
			newObject.transform.localScale = new Vector3(s.x * Scale, s.y * Scale, s.z * Scale);

			interactiveObject.Scale = newObject.transform.localScale.x;

			var rigidbody = newObject.GetComponent<Rigidbody>();
			rigidbody.velocity = Vector3.zero;

			return newObject;
		}

	}

}