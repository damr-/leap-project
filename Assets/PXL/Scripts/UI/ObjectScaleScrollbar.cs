using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(Scrollbar))]
	public class ObjectScaleScrollbar : AdminUIBase {
	
		/// <summary>
		/// The KeyCode for the key to increase the scale
		/// </summary>
		public KeyCode increaseKey = KeyCode.B;

		/// <summary>
		/// The KeyCode for the key to decrease the scale
		/// </summary>
		public KeyCode decreaseKey = KeyCode.V;

		/// <summary>
		/// How much the scale changes when pressing keys
		/// </summary>
		public float changeAmount = 0.1f;

		/// <summary>
		/// The Scrollbar component of this object
		/// </summary>
		protected Scrollbar scrollbar;

		protected override void Start() {
			base.Start();
			scrollbar = this.TryGetComponent<Scrollbar>();
		}

		protected virtual void Update() {
			if (Input.GetKeyDown(increaseKey))
				MoveSlider(changeAmount);
			if (Input.GetKeyDown(decreaseKey))
				MoveSlider(-changeAmount);
		}

		protected virtual void MoveSlider(float amount) {
			throw new System.NotImplementedException("This approach doesn't work (apparently)");
		}
	}

}
