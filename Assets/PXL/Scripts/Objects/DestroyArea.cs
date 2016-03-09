﻿using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public class DestroyArea : TargetArea {

		/// <summary>
		/// The object type to look for
		/// </summary>
		public ObjectType TargetObjectType;

		/// <summary>
		/// Destroy the overlapping object if it has an ObjectBehaviour Component
		/// </summary>
		/// <param name="other">The Collider of the overlapping object</param>
		protected override void HandleValidOther(Collider other) {
			var objectBehaviour = other.TryGetComponent<ObjectBehaviour>();
			objectBehaviour.AssertNotNull("GameObject '" + other.gameObject.name + "' has tag '" + TargetTag + "' but no component ObjectBehaviour!");

			if(TargetObjectType == ObjectType.All || objectBehaviour.ObjectType == TargetObjectType)
				objectBehaviour.DestroyObject();
		}
	}

}
