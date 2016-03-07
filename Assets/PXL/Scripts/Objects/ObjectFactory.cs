using UnityEngine;

namespace PXL.Objects {

	public class ObjectFactory : Factory {

		/// <summary>
		/// Scale of the object
		/// </summary>		
		public float scale { get; set; }

		/// <summary>
		/// Spawns the set prefab at position with the defined scale and resets velocity
		/// </summary>
		public override GameObject Spawn() {
			GameObject newObject = base.Spawn();
			newObject.transform.localScale = new Vector3(scale, scale, scale);
			newObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			return newObject;
		}
	}

}