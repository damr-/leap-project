using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Areas {

	[RequireComponent(typeof(StackArea))]
	public class ChangeColorOnStackedCorrectly : MonoBehaviour {

		/// <summary>
		/// The StackArea of this object
		/// </summary>
		private StackArea StackArea {
			get {
				return mStackArea ?? (mStackArea = this.TryGetComponent<StackArea>());
			}
		}
		private StackArea mStackArea;

		public Light Light;

		/// <summary>
		/// The color of the light on success
		/// </summary>
		public Color SuccessColor;

		private void Start() {
			Light.AssertNotNull("Light reference missing!");

			StackArea.StackedCorrectly.Subscribe(_ => {
				Light.color = SuccessColor;
			});
		}

	}

}