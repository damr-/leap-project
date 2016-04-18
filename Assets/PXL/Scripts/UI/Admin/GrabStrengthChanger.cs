using PXL.Interaction;

namespace PXL.UI.Admin {

	public class GrabStrengthChanger : PropertyChanger {

		/// <summary>
		/// How much the grab strength changes when de- or increasing
		/// </summary>
		private const float StrengthChangeAmount = 0.05f;

		/// <summary>
		/// The default min grab strength
		/// </summary>
		private const float DefaultStrength = 0.25f;

		public override void NextValue() {
			SetGrabStrength(Grabbable.MinGrabStrength + StrengthChangeAmount);
		}

		public override void PreviousValue() {
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
		private void SetGrabStrength(float newStrength) {
			if (newStrength < 0.1f || newStrength > 0.8f)
				return;
			Grabbable.MinGrabStrength = newStrength;
			SetText(newStrength);
		}
		
		/// <summary>
		/// Sets the text of the text component to the given grab strength
		/// </summary>
		private void SetText(float strength) {
			if (PropertyText != null)
				PropertyText.text = strength.ToString("0.00");
		}
	}

}