using System.Collections.Generic;
using UnityEngine;

namespace PXL.Objects {

	public class ObjectFactory : Factory {

		/// <summary>
		/// Scale of the object
		/// </summary>
		public float Scale { get; set; }

		/// <summary>
		/// All the possible objects and their default shape
		/// </summary>
		public readonly IDictionary<ObjectType, Vector3> DefaultObjectScales = new Dictionary<ObjectType, Vector3>();

		/// <summary>
		/// Spawns the set prefab at position with the defined scale and resets velocity
		/// </summary>
		public override GameObject Spawn() {
			var newObject = base.Spawn();
			var objectBehaviour = newObject.GetComponent<ObjectBehaviour>();
			var objectType = objectBehaviour.ObjectType;

			if (!DefaultObjectScales.ContainsKey(objectType)) {
				DefaultObjectScales.Add(objectType, newObject.transform.localScale);
			}
			else {
				newObject.transform.localScale = DefaultObjectScales[objectType];
			}

			var s = newObject.transform.localScale;
			newObject.transform.localScale = new Vector3(s.x * Scale, s.y * Scale, s.z * Scale);

			objectBehaviour.Scale = newObject.transform.localScale.x;

			newObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

			return newObject;
		}
	}

}