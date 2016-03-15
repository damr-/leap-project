using UnityEngine;
using PXL.Utility;

namespace PXL.Objects.Areas {

	public abstract class ObjectArea : TargetArea {

		/// <summary>
		/// The object type to look for
		/// </summary>
		public ObjectType TargetObjectType;

		/// <summary>
		/// Called when the other object is valid and has the correct tag
		/// </summary>
		/// <param name="other">The Collider of the overlapping object</param>
		protected override void HandleValidOther(Collider other) {
			var objectBehaviour = other.TryGetComponent<ObjectBehaviour>();
			objectBehaviour.AssertNotNull("GameObject '" + other.gameObject.name + "' has tag '" + TargetTag + "' but no component ObjectBehaviour!");

			if (HasCorrectType(objectBehaviour)) {
				HandleValidObjectType(objectBehaviour);
			}
			else {
				HandleInvalidObjectType(objectBehaviour);
			}
		}

		/// <summary>
		/// Returns whether the given <see cref="ObjectBehaviour"/> has the correct <see cref="ObjectType"/>
		/// </summary>
		protected bool HasCorrectType(ObjectBehaviour objectBehaviour) {
			return TargetObjectType == ObjectType.All || objectBehaviour.ObjectType == TargetObjectType;
		}

		/// <summary>
		/// Called when the other object is valid, has the correct tag and the correct object type
		/// </summary>
		protected abstract void HandleValidObjectType(ObjectBehaviour objectBehaviour);

		/// <summary>
		/// Called when the other object is valid, has the correct tag but the wrong object type
		/// </summary>
		protected abstract void HandleInvalidObjectType(ObjectBehaviour objectBehaviour);

	}

}