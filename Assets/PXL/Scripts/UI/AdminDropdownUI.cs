using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public abstract class AdminDropdownUi : AdminUiBase {
	
		/// <summary>
		/// The Dropdown Component of the child UI element
		/// </summary>
		protected Dropdown Dropdown;

		/// <summary>
		/// The current Index in the dropdown menu
		/// </summary>
		protected int CurrentIndex;

		/// <summary>
		/// The key used to increase <see cref="CurrentIndex"/>
		/// </summary>
		public KeyCode IncreaseKey = KeyCode.W;

		/// <summary>
		/// The key used to decrease <see cref="CurrentIndex"/>
		/// </summary>
		public KeyCode DecreaseKey = KeyCode.S;

		protected override void Start() {
			base.Start();

			Dropdown = GetComponentInChildren<Dropdown>();
			Dropdown.ClearOptions();

			AddDropdownEntries();

			Dropdown.value = 1;
			CurrentIndex = 1;
		}

		protected abstract void AddDropdownEntries();
	}

}