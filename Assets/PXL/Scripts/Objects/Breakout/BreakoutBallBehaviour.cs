using System;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Breakout {

	[RequireComponent(typeof(Rigidbody))]
	public class BreakoutBallBehaviour : MonoBehaviour {

		/// <summary>
		/// Speed of the ball at the beginning
		/// </summary>
		public float Speed = 1f;

		/// <summary>
		/// The axis in which the ball shouldn't travel for too long because it breaks the game or similar
		/// </summary>
		public Vector3 BadDirection;

		public Vector3 CorrectionDirection;

		/// <summary>
		/// The last known velocity of the ball
		/// </summary>
		private Vector3 oldVelocity;

		/// <summary>
		/// The Rigidbody component of this object
		/// </summary>
		private Rigidbody Rigidbody {
			get { return mRigidbody ?? (mRigidbody = this.TryGetComponent<Rigidbody>()); }
		}
		private Rigidbody mRigidbody;

		private void Start() {
			SetupRigidbody();
			transform.position = new Vector3(transform.position.x, -0.245f, transform.position.z);
		}

		private void OnEnable() {
			Start();
		}

		/// <summary>
		/// Stores the current velocity of the ball
		/// </summary>
		private void LateUpdate() {
			if (Rigidbody.velocity.Equal(Vector3.zero))
				Rigidbody.velocity = Vector3.forward * Speed;

			var diff = Rigidbody.velocity.magnitude - (Vector3.forward * Speed).magnitude;
			if (diff < 0) {
				Rigidbody.velocity *= 1.01f - diff;
			}

			oldVelocity = Rigidbody.velocity;
		}

		/// <summary>
		/// Sets up the needed settings for the <see cref="Rigidbody"/>
		/// </summary>
		private void SetupRigidbody() {
			if (Rigidbody == null)
				return;

			Rigidbody.useGravity = false;
			Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		}

		/// <summary>
		/// Handles the collision with other objects
		/// </summary>
		private void OnCollisionEnter(Collision collision) {
			var contact = collision.contacts[0];
			var reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);
			Rigidbody.velocity = reflectedVelocity;

			var angle = Vector3.Angle(Rigidbody.velocity, BadDirection);
			if (angle < 10 || angle > 170) {
				Rigidbody.velocity += CorrectionDirection;
			}
		}

		/// <summary>
		/// Reset the rigidbody settings to default
		/// </summary>
		private void OnDisable() {
			Rigidbody.useGravity = true;
			Rigidbody.constraints = RigidbodyConstraints.None;
		}
	}

}