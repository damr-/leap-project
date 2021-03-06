﻿using PXL.Utility;
using UnityEngine;

namespace PXL.Movement {

	/// <summary>
	/// This script allows to move this object with the givne keys and a certain speed in worldspace.
	/// Also provides a function to overrite the current velocity and keep it running until it's overwritte with <see cref="Vector3.zero"/>
	/// </summary>
	public class Movement : MonoBehaviour {

		/// <summary>
		/// The key used to go forwards
		/// </summary>
		public KeyCode ForwardKey = KeyCode.W;

		/// <summary>
		/// The key used to go backwards
		/// </summary>
		public KeyCode BackwardKey = KeyCode.S;

		/// <summary>
		/// The key used to strafe left
		/// </summary>
		public KeyCode LeftKey = KeyCode.A;

		/// <summary>
		/// The key used to strafe right
		/// </summary>
		public KeyCode RightKey = KeyCode.D;

		/// <summary>
		/// They key used to go up
		/// </summary>
		public KeyCode UpKey = KeyCode.E;

		/// <summary>
		/// The key used to go down
		/// </summary>
		public KeyCode DownKey = KeyCode.Q;

		/// <summary>
		/// The speed of the movement
		/// </summary>
		public float Speed = 0.2f;

		/// <summary>
		/// The current velocity
		/// </summary>
		private Vector3 velocity;

		private Vector3 overwriteVelocity = Vector3.zero;

		private void Update() {
			velocity.x = overwriteVelocity.x.ApproxEquals() ? GetSpeedFromInput(RightKey, LeftKey) : overwriteVelocity.x;
			velocity.y = overwriteVelocity.y.ApproxEquals() ? GetSpeedFromInput(UpKey, DownKey) : overwriteVelocity.y;
			velocity.z = overwriteVelocity.z.ApproxEquals() ? GetSpeedFromInput(ForwardKey, BackwardKey) : overwriteVelocity.z;

			transform.position += velocity * Time.deltaTime;
		}

		/// <summary>
		/// Returns the valocity for a certain axis, <see cref="Speed"/> when <see cref="positiveKey"/> is pressed, -<see cref="Speed"/> when <see cref="negativeKey"/> is pressed.
		/// Returns 0 when neither one of them is pressed
		/// </summary>
		private float GetSpeedFromInput(KeyCode positiveKey, KeyCode negativeKey) {
			if (Input.GetKey(positiveKey))
				return Speed;
			if (Input.GetKey(negativeKey))
				return -Speed;
			return 0f;
		}

		/// <summary>
		/// Sets the given velocity as the current overwrite velocity, which will be applied every frame
		/// </summary>
		public void OverwriteVelocity(Vector3 newVelocity) {
			overwriteVelocity = newVelocity;
		}
		
	}

}