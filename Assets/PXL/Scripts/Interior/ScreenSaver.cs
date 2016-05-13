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

		public bool LocalSpace;

		public Vector3 Pos {
			get { return LocalSpace ? transform.localPosition : transform.position; }
			set {
				if (LocalSpace)
					transform.localPosition = value;
				else
					transform.position = value;
			}
		}

		private void Start() {
			for (var i = 0; i < 3; i++) {
				velocity[i] = Speed * (MovementAxes[i] ? 1 : 0);
				if (Random.Range(0, 2) == 0)
					velocity[i] *= -1;
			}
			Pos = StartPosition;
		}

		private void Update() {
			UpdateVelocity(MovementAxes[0], Pos.x, MinX, MaxX, ref velocity.x);
			UpdateVelocity(MovementAxes[1], Pos.y, MinY, MaxY, ref velocity.y);
			UpdateVelocity(MovementAxes[2], Pos.z, MinZ, MaxZ, ref velocity.z);
			Pos += velocity * Time.deltaTime;
		}

		private void UpdateVelocity(bool axisUsed, float xPos, float min, float max, ref float currentVelocity) {
			if (axisUsed && (xPos < min || xPos > max))
				currentVelocity *= -1;
		}

		public void UpdateBoundary(string bounaryName) {
			switch (bounaryName) {
				case "minx":
					MinX = Pos.x;
					return;
				case "maxx":
					MaxX = Pos.x;
					return;
				case "miny":
					MinY = Pos.y;
					return;
				case "maxy":
					MaxY = Pos.y;
					return;
				case "minz":
					MinZ = Pos.z;
					return;
				case "maxz":
					MaxZ = Pos.z;
					return;
			}
		}

	}

}