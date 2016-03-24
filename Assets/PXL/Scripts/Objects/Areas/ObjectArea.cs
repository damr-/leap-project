using System.Collections.Generic;
using UnityEngine;
using PXL.Utility;

namespace PXL.Objects.Areas {

	public abstract class ObjectArea : TargetArea {

		/// <summary>
		/// The object type to look for
		/// </summary>
		public ObjectType TargetObjectType;

		/// <summary>
		/// All known GameObjects and their InteractiveObject component
		/// </summary>
		protected static readonly IDictionary<GameObject, InteractiveObject> InteractiveObjects = new Dictionary<GameObject, InteractiveObject>();

		/// <summary>
		/// Called when the other object is valid and has the correct tag
		/// </summary>
		/// <param name="other">The Collider of the overlapping object</param>
		protected override void HandleValidOther(Collider other) {
			var interactiveObject = other.TryGetComponent<InteractiveObject>();
			interactiveObject.AssertNotNull("GameObject '" + other.gameObject.name + "' has tag '" + TargetTag + "' but no component InteractiveObject!");

			if (HasCorrectType(interactiveObject)) {
				HandleValidObjectType(interactiveObject);
			}
			else {
				HandleInvalidObjectType(interactiveObject);
			}
		}

		/// <summary>
		/// Returns whether the given <see cref="InteractiveObject"/> has the correct <see cref="ObjectType"/>
		/// </summary>
		protected bool HasCorrectType(InteractiveObject interactiveObject) {
			return TargetObjectType == ObjectType.All || interactiveObject.ObjectType == TargetObjectType;
		}

		/// <summary>
		/// Called when the other object is valid, has the correct tag and the correct object type
		/// </summary>
		protected abstract void HandleValidObjectType(InteractiveObject interactiveObject);

		/// <summary>
		/// Called when the other object is valid, has the correct tag but the wrong object type
		/// </summary>
		protected abstract void HandleInvalidObjectType(InteractiveObject interactiveObject);

	}

}