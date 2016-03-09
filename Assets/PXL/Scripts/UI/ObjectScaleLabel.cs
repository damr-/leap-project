using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(Text))]
	public class ObjectScaleLabel : AdminUiBase {
	
		/// <summary>
		/// How much the number will be scaled for more natural results
		/// </summary>
		public float ScaleFactor = 100f;
		
		/// <summary>
		/// The text component of the UI element
		/// </summary>
		private Text text;

		protected override void Start() {
			base.Start();

			text = this.TryGetComponent<Text>();
			ObjectManager.ObjectScale.Subscribe(SetText);
		}
		
		/// <summary>
		/// Sets the text of the text component to the given scale
		/// </summary>
		/// <param name="objectScale">The currently selected scale for objects</param>
		private void SetText(float objectScale) {
			text.text = (objectScale * ScaleFactor).ToString("0.0");
		}
	}

}