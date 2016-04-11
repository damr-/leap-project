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

		/// <summary>
		/// The light component which color will be changed
		/// </summary>
		public Light Light;

		/// <summary>
		/// The color of the light on success
		/// </summary>
		public Color SuccessColor;

		private void Start() {
			Light.AssertNotNull("Light reference missing!");

			StackArea.AreaStatus.Subscribe(status => {
				if(status == StackArea.Status.GameWon)
					Light.color = SuccessColor;
			});
		}

	}

}