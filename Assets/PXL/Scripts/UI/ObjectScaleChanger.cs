using UniRx;

namespace PXL.UI {

	public class ObjectScaleChanger : PropertyChanger {

		protected override void Start() {
			base.Start();
			ObjectSpawners.ForEach(o => o.ObjectScale.Subscribe(SetText));
		}

		public override void NextValue() {
			ObjectSpawners.ForEach(os => {
				os.IncreaseObjectScale();
			});

		}

		public override void PreviousValue() {
			ObjectSpawners.ForEach(os => {
				os.DecreaseObjectScale();
			});

		}

		public void ResetValue() {
			ObjectSpawners.ForEach(os => {
				os.ResetObjectScale();
			});
		}
		
		/// <summary>
		/// Sets the text of the text component to the given scale
		/// </summary>
		/// <param name="objectScale">The currently selected scale for objects</param>
		private void SetText(float objectScale) {
			PropertyText.text = objectScale.ToString("0.0");
		}
	}

}
