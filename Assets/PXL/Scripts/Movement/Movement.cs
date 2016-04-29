using UnityEngine;

namespace PXL.Movement {

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
		public float Speed = 0.05f;

		/// <summary>
		/// The current velocity
		/// </summary>
		private Vector3 velocity;

		private void Update() {
			velocity.x = GetSpeedFromInput(RightKey, LeftKey);
			velocity.z = GetSpeedFromInput(ForwardKey, BackwardKey);
			velocity.y = GetSpeedFromInput(UpKey, DownKey);

			transform.position += velocity * Time.deltaTime;
		}

		/// <summary>
		/// Returns the valocity for a certain axis, <see cref="Speed"/> when <see cref="positiveKey"/> is pressed, -<see cref="Speed"/> when <see cref="negativeKey"/> is pressed.
		/// Returns 0 when none of both are pressed
		/// </summary>
		private float GetSpeedFromInput(KeyCode positiveKey, KeyCode negativeKey) {
			if (Input.GetKey(positiveKey))
				return Speed;
			if (Input.GetKey(negativeKey))
				return -Speed;
			return 0f;
		}

	}

}