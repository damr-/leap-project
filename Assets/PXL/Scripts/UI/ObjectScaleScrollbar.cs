using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(Scrollbar))]
	public class ObjectScaleScrollbar : AdminUiBase {
	
		/// <summary>
		/// The KeyCode for the key to increase the scale
		/// </summary>
		public KeyCode IncreaseKey = KeyCode.B;

		/// <summary>
		/// The KeyCode for the key to decrease the scale
		/// </summary>
		public KeyCode DecreaseKey = KeyCode.V;

		/// <summary>
		/// How much the scale changes when pressing keys
		/// </summary>
		public float ChangeAmount = 0.1f;

		/// <summary>
		/// The Scrollbar component of this object
		/// </summary>
		protected Scrollbar Scrollbar;

		protected override void Start() {
			base.Start();
			Scrollbar = this.TryGetComponent<Scrollbar>();
		}

		protected virtual void Update() {
			if (Input.GetKeyDown(IncreaseKey))
				MoveSlider(ChangeAmount);
			if (Input.GetKeyDown(DecreaseKey))
				MoveSlider(-ChangeAmount);
		}

		protected virtual void MoveSlider(float amount) {
			throw new System.NotImplementedException("This approach doesn't work (apparently)");
		}
	}

}
