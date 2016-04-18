using UniRx;

namespace PXL.UI.Admin {

	public class ObjectScaleChanger : PropertyChanger {

		protected override void Start() {
			base.Start();
			ObjectSpawner.ObjectScale.Subscribe(SetText);
			SetText(ObjectSpawner.ObjectScale.Value);
		}

		public override void NextValue() {
			ObjectSpawner.IncreaseObjectScale();
		}

		public override void PreviousValue() {
			ObjectSpawner.DecreaseObjectScale();
		}
		
		/// <summary>
		/// Sets the text of the text component to the given scale
		/// </summary>
		/// <param name="objectScale">The currently selected scale for objects</param>
		private void SetText(float objectScale) {
			if (PropertyText != null)
				PropertyText.text = objectScale.ToString("0.00");
		}
	}

}
