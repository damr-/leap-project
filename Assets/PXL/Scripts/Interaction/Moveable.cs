using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	public class MovementInfo {
		public Moveable Moveable;
		public Vector3 Delta;
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
		/// On which axis the object should not be able to move
		/// </summary>
		public Vector3 FreezePosition = Vector3.zero;

		/// <summary>
		/// On which axis the object should not be able to rotate
		/// </summary>
		public Vector3 FreezeRotation = Vector3.zero;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		public Grabbable Grabbable {
			get { return mGrabbable ?? (mGrabbable = this.TryGetComponent<Grabbable>()); }
		}

		private Grabbable mGrabbable;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}

		private Touchable mTouchable;

		/// <summary>
		/// The Transform this object tracks when grabbed
		/// </summary>
		private Transform trackedTarget;

		/// <summary>
		/// Position of the object in the last frame
		/// </summary>
		private Vector3 lastPosition;

		/// <summary>
		/// How far an object has to move to invoke an event
		/// </summary>
		private const float MoveThresHold = 0.05f;

		/// <summary>
		/// The offset of the object's position when being picked up
		/// </summary>
		private Vector3 posOffset;

		/// <summary>
		/// The offset of the object's rotation when begin picked up
		/// </summary>
		private Quaternion rotOffset;

		/// <summary>
		/// The percent of the distance to the grabbed object which will be kept while holding/moving
		/// </summary>
		public float OffsetPercent = 1f;

		/// <summary>
		/// The default state of position when frozen
		/// </summary>
		public Vector3 OverwritePositionValues = Vector3.zero;

		/// <summary>
		/// The default state of rotation when frozen
		/// </summary>
		public Vector3 OverwriteRotationValues = Vector3.zero;

		/// <summary>
		/// Information about which position axis has an overwritten value
		/// </summary>
		public bool[] OverwritePosition = new bool[3];

		/// <summary>
		/// Information about which rotation axis has an overwritten value
		/// </summary>
		public bool[] OverwriteRotation = new bool[3];

		/// <summary>
		/// Sets up subscriptions
		/// </summary>
		private void Start() {
			Grabbable.IsGrabbed.Subscribe(HandleGrabStateChange);
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
		/// Calculates and sets the position of this object according to the tracked object, the frozen axes and the overwritten frozen values
		/// </summary>
		private void UpdatePosition() {
			var oldPosition = transform.position;
			var newPosition = CalculateObjectPosition();

			for (var i = 0; i < 3; i++) {
				var defaultPositionValue = OverwritePosition[i] ? OverwritePositionValues[i] : oldPosition[i];
				newPosition[i] = FreezePosition[i] > 0 ? defaultPositionValue : newPosition[i];
			}

			transform.position = newPosition;
		}

		/// <summary>
		/// Calculates and sets the rotation of this object according to the tracked object, the frozen axes and the overwritten frozen values
		/// </summary>
		private void UpdateRotation() {
			var oldRotation = transform.rotation.eulerAngles;
			var newRotation = (trackedTarget.rotation * rotOffset).eulerAngles;

			for (var i = 0; i < 3; i++) {
				var defaultRotationValue = OverwriteRotation[i] ? OverwriteRotationValues[i] : oldRotation[i];
				newRotation[i] = FreezeRotation[i] > 0 ? defaultRotationValue : newRotation[i];
			}

			transform.rotation = Quaternion.Euler(newRotation);
		}

		/// <summary>
		/// Called when the grabbed state of the object changes
		/// </summary>
		private void HandleGrabStateChange(bool grabbed) {
			if (grabbed) {
				trackedTarget = Grabbable.CurrentHand.palm;
				posOffset = transform.position - Touchable.GetAverageFingerPosition(Grabbable.CurrentHand);
				rotOffset = Quaternion.Inverse(trackedTarget.rotation) * transform.rotation;
			}
			else {
				trackedTarget = null;
			}
		}

		/// <summary>
		/// Checks whether the object has been moved beyond a certain threshold and if yes, emits the subject
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

		/// <summary>
		/// Returns the position of the object whilst being held
		/// </summary>
		private Vector3 CalculateObjectPosition() {
			return Touchable.GetAverageFingerPosition(Grabbable.CurrentHand) - posOffset.magnitude * trackedTarget.up * OffsetPercent;
		}

		/// <summary>
		/// Remove the tracked target when the object is destroyed
		/// </summary>
		private void OnDisable() {
			trackedTarget = null;
		}

	}

}