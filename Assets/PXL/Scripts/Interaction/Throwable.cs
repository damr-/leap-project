using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Interaction {

	[RequireComponent(typeof(Grabbable))]
	public class Throwable : MonoBehaviour {

		/// <summary>
		/// The force applied to this object
		/// </summary>
		public float Force = 20f;

		/// <summary>
		/// How many times per second the position will be captured
		/// </summary>
		private const float MaxDeltaCount = 5f;

		/// <summary>
		/// The minimum magnitude the object's movement has to have to disable the drop delay
		/// </summary>
		private const float DisableDropDelayMagnitude = 0.025f;

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
		public Rigidbody Rigidbody {
			get { return mRigidbody ?? (mRigidbody = this.TryGetComponent<Rigidbody>()); }
		}
		private Rigidbody mRigidbody;

		/// <summary>
		/// The last known positions of this object
		/// </summary>
		private readonly List<Vector3> positionDeltas = new List<Vector3>();

		/// <summary>
		/// The object's position in the last frame
		/// </summary>
		private Vector3 lastPosition;

		private void Start() {
			Grabbable.IsGrabbed.Subscribe(HandleGrabStateChange);
		}

		private void HandleGrabStateChange(bool grabbed) {
			if (grabbed)
				positionDeltas.Clear();
			else
				Rigidbody.AddForce(GetMotionDirection() * Force, ForceMode.Impulse);
		}

		/// <summary>
		/// Calculates the delta movement and updates the stored deltas according to the motion.
		/// </summary>
		private void Update() {
			if (!Grabbable.IsGrabbed)
				return;

			var deltaPos = transform.position - lastPosition;

			for (var i = 0; i < 3; i++)
				deltaPos[i] = deltaPos[i].Truncate(4);

			positionDeltas.Add(deltaPos);
			if (positionDeltas.Count > MaxDeltaCount)
				positionDeltas.RemoveAt(0);

			if (Grabbable.EnableDropDelay)
				if (GetMotionDirection().magnitude > DisableDropDelayMagnitude)
					Grabbable.EnableDropDelay = false;
			else
				if (GetMotionDirection().magnitude <= DisableDropDelayMagnitude)
					Grabbable.EnableDropDelay = true;

			lastPosition = transform.position;
		}

		public Vector3 GetMotionDirection() {
			return positionDeltas.Aggregate(Vector3.zero, (current, next) => current + next);
		}

	}

}