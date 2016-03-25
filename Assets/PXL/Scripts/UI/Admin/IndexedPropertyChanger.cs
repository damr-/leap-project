using PXL.Utility;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public abstract class IndexedPropertyChanger : PropertyChanger {
	
		/// <summary>
		/// Reference to the optional preview image
		/// </summary>
		public Image Preview;

		/// <summary>
		/// The index of the currently selected property
		/// </summary>
		protected int CurrentPropertyIndex;

		protected override void Start() {
			base.Start();
			Preview.AssertNotNull("The target preview Image component is missing!");
			ChangeProperty(1);
		}

		public override void NextValue() {
			ChangeProperty(CurrentPropertyIndex + 1);
		}

		public override void PreviousValue() {
			ChangeProperty(CurrentPropertyIndex - 1);
		}

		protected abstract void ChangeProperty(int index);

		protected abstract bool IsValidIndex(int index);

	}

}