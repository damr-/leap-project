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
		public float StartSpeed = 1f;

		/// <summary>
		/// How often per second the speed is increased
		/// </summary>
		public float SpeedIncreaseFrequency = 1f;

		/// <summary>
		/// The increase of the speed per <see cref="SpeedIncreaseFrequency"/>
		/// </summary>
		public float SpeedIncreaseAmount = 0.05f;
		
		/// <summary>
		/// The maximum possible speed of this ball
		/// </summary>
		public float MaxSpeed = 4;

		/// <summary>
		/// The current speed of the ball
		/// </summary>
		private float currentSpeed;

		/// <summary>
		/// The last known velocity of the ball
		/// </summary>
		private Vector3 oldVelocity;

		/// <summary>
		/// Disposable for the speed increase interval timer
		/// </summary>
		private IDisposable increaseSpeedDisposable = Disposable.Empty;

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

			currentSpeed = StartSpeed;

			Rigidbody.velocity = Vector3.forward * currentSpeed;

			increaseSpeedDisposable = Observable.Interval(TimeSpan.FromSeconds(1/SpeedIncreaseFrequency)).Subscribe(_ => IncreaseSpeed());
		}

		/// <summary>
		/// Increases the speed of the ball b<see cref="SpeedIncreaseAmount"/> and stops increasing it if it's at <see cref="MaxSpeed"/>
		/// </summary>
		private void IncreaseSpeed() {
			currentSpeed += SpeedIncreaseAmount;

			if (currentSpeed < MaxSpeed)
				return;

			currentSpeed = MaxSpeed;
			increaseSpeedDisposable.Dispose();
		}

		/// <summary>
		/// Stores the current velocity of the ball
		/// </summary>
		private void LateUpdate() {
			oldVelocity = Rigidbody.velocity;
		}

		/// <summary>
		/// Sets up the needed settings for the <see cref="Rigidbody"/>
		/// </summary>
		private void SetupRigidbody() {
			Rigidbody.isKinematic = false;
			Rigidbody.useGravity = false;
			Rigidbody.drag = 0f;
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
		/// Handles the deactivation of this object
		/// </summary>
		private void OnDisable() {
			increaseSpeedDisposable.Dispose();
		}
	}

}