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
			if(Rigidbody.velocity.Equal(Vector3.zero))
				Rigidbody.velocity = Vector3.forward * Speed;

			if (Rigidbody.velocity.magnitude < (Vector3.forward*Speed).magnitude)
				Rigidbody.velocity *= 1.01f;

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