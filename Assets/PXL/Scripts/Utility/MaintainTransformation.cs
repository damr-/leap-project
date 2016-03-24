using UnityEngine;

namespace PXL.Utility {

	public class MaintainTransformation : MonoBehaviour {

		public bool MaintainPosition;

		public bool DefinedPosition;

		public Vector3 Position;

		public bool MaintainRotation;

		public bool DefinedRotation;

		public Vector3 Rotation;

		public bool MaintainScale;

		public bool DefinedScale;

		public Vector3 Scale;

		private void Start() {
			if (!DefinedPosition)
				Position = transform.position;
			if (!DefinedRotation)
				Rotation = transform.rotation.eulerAngles;
			if (!DefinedScale)
				Scale = transform.localScale;
		}

		private void Update() {
			if (MaintainPosition)
				transform.position = Position;

			if (MaintainRotation)
				transform.rotation = Quaternion.Euler(Rotation);

			if (MaintainScale)
				transform.localScale = Scale;
		}

	}

}