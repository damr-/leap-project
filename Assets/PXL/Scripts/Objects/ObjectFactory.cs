using UnityEngine;

namespace PXL.Objects {

	public class ObjectFactory : Factory {

		/// <summary>
		/// Scale of the object
		/// </summary>		
		public float Scale { get; set; }

		/// <summary>
		/// Spawns the set prefab at position with the defined scale and resets velocity
		/// </summary>
		public override GameObject Spawn() {
			var newObject = base.Spawn();
			Vector3 s = newObject.transform.localScale;
            newObject.transform.localScale = new Vector3(s.x * Scale, s.y * Scale, s.z * Scale);
			newObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			return newObject;
		}
	}

}