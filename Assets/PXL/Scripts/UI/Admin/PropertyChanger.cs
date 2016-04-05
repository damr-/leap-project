using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public abstract class PropertyChanger : MenuElement {

		/// <summary>
		/// The key used to select the next element or increase the value
		/// </summary>
		protected readonly KeyCode IncreaseKey = KeyCode.RightArrow;

		/// <summary>
		/// The key used to select the previous element or decrease the value
		/// </summary>
		protected readonly KeyCode DecreaseKey = KeyCode.LeftArrow;

		/// <summary>
		/// The Button component of the decrease button
		/// </summary>
		public Button DecreaseButton;

		/// <summary>
		/// The Button component of the increase button
		/// </summary>
		public Button IncreaseButton;

		/// <summary>
		/// The Text component where the property is displayed
		/// </summary>
		public Text PropertyText;

		/// <summary>
		/// The target <see cref="ObjectSpawner"/>, whose properties this script changes
		/// </summary>
		public ObjectSpawner ObjectSpawner;

		/// <summary>
		/// Checks for active admin mode and key strokes
		/// </summary>
		protected virtual void Update() {
			if (!IsAdmin || !IsSelected) {
				return;
			}

			if (Input.GetKeyDown(IncreaseKey)) {
				IncreaseButton.onClick.Invoke();
			}

			if (Input.GetKeyDown(DecreaseKey)) {
				DecreaseButton.onClick.Invoke();
			}
		}

		protected override void AssertReferences() {
			base.AssertReferences();
			DecreaseButton.AssertNotNull("Decrease button missing");
			IncreaseButton.AssertNotNull("Increase button missing");
			PropertyText.AssertNotNull("Property text missing");
		}

		public abstract void NextValue();

		public abstract void PreviousValue();

		protected bool IsPlusKeyDown() {
			if (!Input.anyKeyDown)
				return false;
			return Input.inputString == "+";
		}

	}

}