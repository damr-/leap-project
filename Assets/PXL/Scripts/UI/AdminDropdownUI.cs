using UnityEngine.UI;

namespace PXL.UI {

	public abstract class AdminDropdownUI : AdminUIBase {
	
		/// <summary>
		/// The Dropdown Component of the child UI element
		/// </summary>
		protected Dropdown dropdown;

		protected override void Start() {
			base.Start();

			dropdown = GetComponentInChildren<Dropdown>();
			dropdown.ClearOptions();

			AddDropdownEntries();

			dropdown.value = 1;
		}

		protected abstract void AddDropdownEntries();
	}

}