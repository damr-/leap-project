using PXL.Objects.Spawner;
using UniRx;

namespace PXL.UI.Admin {

	/// <summary>
	/// This script provides the functionality to change the scale of an object spawner's future spawned objects.
	/// 
	/// It also updates the text display which shows the current scale.
	/// </summary>
	public class ObjectScaleChanger : PropertyChanger {

		public override void SetObjectSpawner(ObjectSpawner objectSpawner) {
			base.SetObjectSpawner(objectSpawner);

			ObjectSpawner.CurrentScaleFactor.Subscribe(SetText);
			SetText(ObjectSpawner.CurrentScaleFactor.Value);
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
