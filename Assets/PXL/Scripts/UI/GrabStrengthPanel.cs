using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class GrabStrengthPanel : AdminUiBase {

		/// <summary>
		/// The Button component of the decrease button
		/// </summary>
		public Button DecreaseButton;

		/// <summary>
		/// The Button component of the increase button
		/// </summary>
		public Button IncreaseButton;

		/// <summary>
		/// The Text component where the grab strength is shown
		/// </summary>
		public Text Text;

		/// <summary>
		/// The KeyCode for the key to increase the grab strength
		/// </summary>
		public readonly KeyCode IncreaseKey = KeyCode.Period;

		/// <summary>
		/// The KeyCode for the key to decrease the grab strength
		/// </summary>
		public readonly KeyCode DecreaseKey = KeyCode.Comma;

		/// <summary>
		/// How much the grab strength changes when de- or increasing
		/// </summary>
		private const float StrengthChangeAmount = 0.05f;

		/// <summary>
		/// The default min grab strength
		/// </summary>
		private const float DefaultStrength = 0.25f;

		protected override void Start() {
			base.Start();

			DecreaseButton.AssertNotNull();
			IncreaseButton.AssertNotNull();
			Text.AssertNotNull();
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
		/// Sets the text of the text component to the given grab strength
		/// </summary>
		private void SetText(float strength) {
			Text.text = strength.ToString("0.00");
		}

		/// <summary>
		/// Increases <see cref="Grabbable.MinGrabStrength"/> by <see cref="StrengthChangeAmount"/>
		/// </summary>
		public void IncreaseStrength() {
			SetGrabStrength(Grabbable.MinGrabStrength + StrengthChangeAmount);
		}

		/// <summary>
		/// Decreases <see cref="Grabbable.MinGrabStrength"/> by <see cref="StrengthChangeAmount"/>
		/// </summary>
		public void DecreaseStrength() {
			SetGrabStrength(Grabbable.MinGrabStrength - StrengthChangeAmount);
		}

		/// <summary>
		/// Resets <see cref="Grabbable.MinGrabStrength"/> to <see cref="DefaultStrength"/>
		/// </summary>
		public void ResetStrength() {
			SetGrabStrength(DefaultStrength);
		}

		/// <summary>
		/// Sets <see cref="Grabbable.MinGrabStrength"/> to the given value and updates the UI text
		/// </summary>
		/// <param name="newStrength"></param>
		private void SetGrabStrength(float newStrength) {
			if (newStrength < 0.1f || newStrength > 0.8f)
				return;
			Grabbable.MinGrabStrength = newStrength;
			SetText(newStrength);
		}
	}

}