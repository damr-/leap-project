using PXL.Utility;
using UnityEngine;

namespace PXL.Interaction {

	public class MovementInfo {
		public Vector3 Delta;
		public Moveable Moveable;
		public Vector3 NewPosition;

		public MovementInfo(Moveable moveable, Vector3 delta, Vector3 newPosition) {
			Moveable = moveable;
			Delta = delta;
			NewPosition = newPosition;
		}
	}

	[RequireComponent(typeof(Grabbable))]
	public class Moveable : MonoBehaviour {

		/// <summary>
		/// How far an object has to move to invoke an event
		/// </summary>
		private const float MoveThresHold = 0.01f;

		/// <summary>
		/// On which axis the object should not be able to move
		/// </summary>
		public Vector3 FreezePosition = Vector3.zero;

		/// <summary>
		/// On which axis the object should not be able to rotate
		/// </summary>
		public Vector3 FreezeRotation = Vector3.zero;

		/// <summary>
		/// Position of the object in the last frame
		/// </summary>
		private Vector3 lastPosition;

		/// <summary>
		/// The percent of the distance to the grabbed object which will be kept while holding/moving
		/// </summary>
		public float OffsetPercent = 1f;

		/// <summary>
		/// Information about which position axis has an overwritten value
		/// </summary>
		public bool[] OverwritePosition = new bool[3];

		/// <summary>
		/// The default state of position when frozen
		/// </summary>
		public Vector3 OverwritePositionValues = Vector3.zero;

		/// <summary>
		/// Information about which rotation axis has an overwritten value
		/// </summary>
		public bool[] OverwriteRotation = new bool[3];

		/// <summary>
		/// The default state of rotation when frozen
		/// </summary>
		public Vector3 OverwriteRotationValues = Vector3.zero;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		public Grabbable Grabbable {
			get { return mGrabbable ?? (mGrabbable = this.TryGetComponent<Grabbable>()); }
		}
		private Grabbable mGrabbable;
		
		/// <summary>
		/// The position of the object in the previous frame
		/// </summary>
		private Vector3 oldPosition;

		/// <summary>
		/// The rotation of the object in the previous frame
		/// </summary>
		private Vector3 oldRotation;
		
		private void Start() {
			lastPosition = transform.position;
		}

		/// <summary>
		/// Updates the state, position and rotation of the object (if grabbed).
		/// Checks for the movement threshold.
		/// </summary>
		private void FixedUpdate() {
			if (!Grabbable.IsGrabbed)
				return;

			UpdatePosition();
			UpdateRotation();
			CheckMovement();
		}

		/// <summary>
		/// Calculates and sets the position of this object according to the tracked object, the frozen axes and the overwritten
		/// frozen values
		/// </summary>
		private void UpdatePosition() {
			var newPosition = transform.position;

			for (var i = 0; i < 3; i++) {
				if (!(FreezePosition[i] > 0)) 
					continue;
				var positionValue = OverwritePosition[i] ? OverwritePositionValues[i] : oldPosition[i];
				newPosition[i] = positionValue;
			}

			transform.position = newPosition;
			oldPosition = transform.position;
		}

		/// <summary>
		/// Calculates and sets the rotation of this object according to the tracked object, the frozen axes and the overwritten
		/// frozen values
		/// </summary>
		private void UpdateRotation() {
			var newRotation = transform.rotation.eulerAngles;
			
			for (var i = 0; i < 3; i++) {
				if (!(FreezeRotation[i] > 0))
					continue;
				var rotationValue = OverwriteRotation[i] ? OverwriteRotationValues[i] : oldRotation[i];
				newRotation[i] = rotationValue;
			}

			transform.rotation = Quaternion.Euler(newRotation);
			oldRotation = transform.rotation.eulerAngles;
		}

		/// <summary>
		/// Checks whether the object has been moved beyond a certain threshold. If yes, invokes the observable
		/// </summary>
		private void CheckMovement() {
			if (!(Vector3.Distance(transform.position, lastPosition) > MoveThresHold))
				return;

			Grabbable.InteractionHand.MoveObject(
				new MovementInfo(this,
					transform.position - lastPosition,
					transform.position));

			lastPosition = transform.position;
		}

	}

}