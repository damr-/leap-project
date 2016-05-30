using UnityEngine;

namespace PXL.UI.World.Hand {

	/// <summary>
	/// This script provides functionality for an object to track another one.
	/// 
	/// There are multiple techniques to track another object or position which can be chosen from.
	/// </summary>
	public class TrackRotation : MonoBehaviour {

		/// <summary>
		/// The technique used to track a transform or a position
		/// </summary>
		public enum TrackingTechnique {
			TargetTransform = 0,
			CustomPosition = 1,
			OutermostParentTransform = 2,
			OutermostParentPosition = 3
		}

		public TrackingTechnique Technique = TrackingTechnique.OutermostParentPosition;

		/// <summary>
		/// The target transform to rotate around
		/// </summary>
		public Transform TargetTransform;

		/// <summary>
		/// The custom origin to rotate around
		/// </summary>
		public Vector3 CustomPosition = new Vector3(
			-0.05f,
			0.12f,
			-0.057f
		);

		/// <summary>
		/// The transform of the outermost parent
		/// </summary>
		private Transform outermostParent;

		/// <summary>
		/// The initial position of the outermost parent
		/// </summary>
		private Vector3 initialOutermostParentPosition;

		/// <summary>
		/// The actual position the object rotates around
		/// </summary>
		private Vector3 targetPosition;

		/// <summary>
		/// Stores the outermost parent and sets up static positions, if the technique is set that way
		/// </summary>
		private void Start() {
			var o = transform;
			while (o.parent != null)
				o = o.parent;
			outermostParent = o;
			initialOutermostParentPosition = o.position;

			switch (Technique) {
				case TrackingTechnique.OutermostParentPosition:
					targetPosition = initialOutermostParentPosition;
					break;
				case TrackingTechnique.CustomPosition:
					targetPosition = CustomPosition;
					break;
			}
		}

		/// <summary>
		/// Updates the <see cref="targetPosition"/> if a transform is tracked and sets the object's rotation accordingly
		/// </summary>
		private void LateUpdate() {
			switch (Technique) {
				case TrackingTechnique.TargetTransform:
					targetPosition = TargetTransform.position;
					break;
				case TrackingTechnique.OutermostParentTransform:
					targetPosition = outermostParent.position;
					break;
			}

			transform.rotation = Quaternion.LookRotation(transform.position - targetPosition);
		}

	}

}