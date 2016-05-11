using UnityEngine;

namespace PXL.Interior {

	public class ScreenSaver : MonoBehaviour {

		public float Speed = 0.2f;

		public Vector3 StartPosition;

		public bool[] MovementAxes = new bool[3];

		public float MinX;

		public float MaxX;

		public float MinY;

		public float MaxY;

		public float MinZ;

		public float MaxZ;

		private Vector3 velocity = Vector3.zero;

		private void Start() {
			for (var i = 0; i < 3; i++) {
				velocity[i] = Speed * (MovementAxes[i] ? 1 : 0);
				if (Random.Range(0, 2) == 0)
					velocity[i] *= -1;
			}
			transform.position = StartPosition;
		}

		private void Update() {
			UpdateVelocity(MovementAxes[0], transform.position.x, MinX, MaxX, ref velocity.x);
			UpdateVelocity(MovementAxes[1], transform.position.y, MinY, MaxY, ref velocity.y);
			UpdateVelocity(MovementAxes[2], transform.position.z, MinZ, MaxZ, ref velocity.z);
			transform.position += velocity * Time.deltaTime;
		}

		private void UpdateVelocity(bool axisUsed, float xPos, float min, float max, ref float currentVelocity) {
			if (axisUsed && (xPos < min || xPos > max))
				currentVelocity *= -1;
		}

		public void UpdateBoundary(string bounaryName) {
			switch (bounaryName) {
				case "minx":
					MinX = transform.position.x;
					return;
				case "maxx":
					MaxX = transform.position.x;
					return;
				case "miny":
					MinY = transform.position.y;
					return;
				case "maxy":
					MaxY = transform.position.y;
					return;
				case "minz":
					MinZ = transform.position.z;
					return;
				case "maxz":
					MaxZ = transform.position.z;
					return;
			}
		}

	}

}