using UnityEngine;

namespace PXL.Interior {

	/// <summary>
	/// This script makes an object move on certain axes between minimum and maximum values (definable for each axis)
	/// </summary>
	public class LinearMovement : MonoBehaviour {

		/// <summary>
		/// The speed of the movement
		/// </summary>
		public float Speed = 0.2f;

		/// <summary>
		/// The starting position of the object
		/// </summary>
		public Vector3 StartPosition;

		/// <summary>
		/// Which axes are used for the movement
		/// </summary>
		public bool[] MovementAxes = new bool[3];

		/// <summary>
		/// The minimum allowed value for the object's x-Position
		/// </summary>
		public float MinX;

		/// <summary>
		/// The maximum allowed value for the object's x-Position
		/// </summary>
		public float MaxX;

		/// <summary>
		/// The minimum allowed value for the object's y-Position
		/// </summary>
		public float MinY;

		/// <summary>
		/// The maximum allowed value for the object's y-Position
		/// </summary>
		public float MaxY;

		/// <summary>
		/// The minimum allowed value for the object's z-Position
		/// </summary>
		public float MinZ;

		/// <summary>
		/// The maximum allowed value for the object's z-Position
		/// </summary>
		public float MaxZ;

		/// <summary>
		/// The current velocity of the object
		/// </summary>
		private Vector3 velocity = Vector3.zero;

		/// <summary>
		/// WHether to use local or world space for the coordinates
		/// </summary>
		public bool LocalSpace;

		/// <summary>
		/// The current position of the object, taking <see cref="LocalSpace"/> into account
		/// </summary>
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