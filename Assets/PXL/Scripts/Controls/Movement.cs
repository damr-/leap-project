using UnityEngine;

namespace PXL.Controls {

	public class Movement : MonoBehaviour {

		public KeyCode ForwardKey = KeyCode.W;

		public KeyCode BackwardKey = KeyCode.S;

		public KeyCode LeftKey = KeyCode.A;

		public KeyCode RightKey = KeyCode.D;

		public float Speed = 0.05f;

		public Vector3 Velocity;

		private void Update() {
			if (Input.GetKey(ForwardKey))
				Velocity.z = Speed;
			else if (Input.GetKey(BackwardKey))
				Velocity.z = -Speed;
			else
				Velocity.z = 0f;

			if (Input.GetKey(RightKey))
				Velocity.x = Speed;
			else if (Input.GetKey(LeftKey))
				Velocity.x = -Speed;
			else
				Velocity.x = 0f;

			transform.position += Velocity * Time.deltaTime;
		}

	}

}