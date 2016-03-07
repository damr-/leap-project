using UniRx;
using UnityEngine.UI;

namespace PXL.UI {

	public class ObjectScaleLabel : AdminUIBase {
	
		/// <summary>
		/// How much the number will be scaled for more natural results
		/// </summary>
		public float scaleFactor = 100f;
		
		/// <summary>
		/// The text component of the UI element
		/// </summary>
		private Text text;

		protected override void Start() {
			base.Start();

			text = GetComponent<Text>();
			objectManager.ObjectScale.Subscribe(SetText);
		}
		
		/// <summary>
		/// Sets the text of the text component to the given scale
		/// </summary>
		/// <param name="objectScale">The currently selected scale for objects</param>
		private void SetText(float objectScale) {
			text.text = (objectScale * scaleFactor).ToString("0.0");
		}
	}

}