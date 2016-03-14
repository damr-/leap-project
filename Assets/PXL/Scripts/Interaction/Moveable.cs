﻿using PXL.Utility;
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
		/// Sets up subscriptions
		/// </summary>
		private void Start() {
			lastPosition = transform.position;
			Grabbable.IsGrabbed.Subscribe(HandleGrabStateChange);
		}

		/// <summary>
		/// Updates the state, position and rotation of the object, if grabbed
		/// </summary>
		private void Update() {
			if (!Grabbable.IsGrabbed)
				return;

			transform.position = CalculateObjectPosition();
			transform.rotation = trackedTarget.rotation * rotOffset;

			CheckMovement();
		}

		/// <summary>
		/// Called when the grabbed state of the object changes
		/// </summary>
		/// <param name="grabbed"></param>
		private void HandleGrabStateChange(bool grabbed) {
			if (grabbed) {
				trackedTarget = Grabbable.CurrentHand.palm;
				posOffset = transform.position - Touchable.GetAverageFingerPosition(Grabbable.CurrentHand);
				rotOffset = Quaternion.Inverse(transform.rotation) * trackedTarget.rotation;
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
		/// <param name="offsetPercent">How many percent of the original offset should be kept when holding it</param>
		private Vector3 CalculateObjectPosition(float offsetPercent = 1f) {
			return Touchable.GetAverageFingerPosition(Grabbable.CurrentHand) - posOffset.magnitude * trackedTarget.up * 1 * offsetPercent;
		}

		/// <summary>
		/// Remove the tracked target when the object is destroyed
		/// </summary>
		private void OnDisable() {
			trackedTarget = null;
		}

	}

}