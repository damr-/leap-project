using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class ObjectScalePanel : AdminUiBase {

		/// <summary>
		/// The Button component of the decrease button
		/// </summary>
		public Button DecreaseButton;

		/// <summary>
		/// The Button component of the increase button
		/// </summary>
		public Button IncreaseButton;
	
		/// <summary>
		/// The Text component where the scale is shown
		/// </summary>
		public Text text;

		/// <summary>
		/// The KeyCode for the key to increase the scale
		/// </summary>
		public readonly KeyCode IncreaseKey = KeyCode.M;

		/// <summary>
		/// The KeyCode for the key to decrease the scale
		/// </summary>
		public readonly KeyCode DecreaseKey = KeyCode.N;

		protected override void Start() {
			base.Start();
			ObjectManager.ForEach(o => o.ObjectScale.Subscribe(SetText));

			DecreaseButton.AssertNotNull();
			IncreaseButton.AssertNotNull();
			text.AssertNotNull();
		}

		protected virtual void Update() {
			if (Input.GetKeyDown(IncreaseKey)) {
				IncreaseButton.onClick.Invoke();
			}
			if (Input.GetKeyDown(DecreaseKey)) {
				DecreaseButton.onClick.Invoke();
			}
		}

		/// <summary>
		/// Sets the text of the text component to the given scale
		/// </summary>
		/// <param name="objectScale">The currently selected scale for objects</param>
		private void SetText(float objectScale) {
			text.text = objectScale.ToString("0.0");
		}
	}

}
