using UnityEngine.UI;

namespace PXL.UI {

	public abstract class AdminDropdownUi : AdminUiBase {
	
		/// <summary>
		/// The Dropdown Component of the child UI element
		/// </summary>
		protected Dropdown Dropdown;

		protected override void Start() {
			base.Start();

			Dropdown = GetComponentInChildren<Dropdown>();
			Dropdown.ClearOptions();

			AddDropdownEntries();

			Dropdown.value = 1;
		}

		protected abstract void AddDropdownEntries();
	}

}