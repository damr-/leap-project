using UnityEngine;

namespace PXL.Utility {

	/// <summary>
	/// A script which can make an object maintain it's position, scale and/or rotation throughout the game.
	/// 
	/// These transformation values can also be set to custom values.
	/// </summary>
	public class MaintainTransformation : MonoBehaviour {

		/// <summary>
		/// Whether the position is maintained
		/// </summary>
		public bool MaintainPosition;

		/// <summary>
		/// Whether the position is custom
		/// </summary>
		public bool DefinedPosition;

		/// <summary>
		/// The maintained position of the object
		/// </summary>
		public Vector3 MaintainedPosition;

		/// <summary>
		/// Whether the rotation should be maintained
		/// </summary>
		public bool MaintainRotation;

		/// <summary>
		/// Whether the rotation is custom
		/// </summary>
		public bool DefinedRotation;

		/// <summary>
		/// The maintained rotation
		/// </summary>
		public Vector3 MaintainedRotation;

		/// <summary>
		/// Whether the scale should be maintained
		/// </summary>
		public bool MaintainScale;

		/// <summary>
		/// Whether the scale is custom
		/// </summary>
		public bool DefinedScale;

		/// <summary>
		/// The maintained scale
		/// </summary>
		public Vector3 MaintainedScale;

		private void Start() {
			if (!DefinedPosition)
				MaintainedPosition = transform.position;
			if (!DefinedRotation)
				MaintainedRotation = transform.rotation.eulerAngles;
			if (!DefinedScale)
				MaintainedScale = transform.localScale;
		}

		private void Update() {
			if (MaintainPosition)
				transform.position = MaintainedPosition;

			if (MaintainRotation)
				transform.rotation = Quaternion.Euler(MaintainedRotation);

			if (MaintainScale)
				transform.localScale = MaintainedScale;
		}

	}

}