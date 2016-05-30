using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	/// <summary>
	/// This script provides the functionality to change a property either via keys or buttons.
	/// 
	/// How the property is changed in the end has to be implemented in an inheriting class.
	/// </summary>
	public abstract class PropertyChanger : AdminBase {

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
		protected ObjectSpawner ObjectSpawner;

		protected override void Start() {
			base.Start();
			AssertReferences();
		}

		/// <summary>
		/// Checks for active admin mode and key strokes
		/// </summary>
		protected virtual void Update() {
			if (!IsAdmin)
				return;

			if (Input.GetKeyDown(IncreaseKey))
				IncreaseButton.onClick.Invoke();

			if (Input.GetKeyDown(DecreaseKey))
				DecreaseButton.onClick.Invoke();
		}

		private void AssertReferences() {
			DecreaseButton.AssertNotNull("Decrease button missing");
			IncreaseButton.AssertNotNull("Increase button missing");
		}

		public abstract void NextValue();

		public abstract void PreviousValue();

		/// <summary>
		/// Returns whether the plus ('+') key was just pressed down
		/// </summary>
		protected bool IsPlusKeyDown() {
			if (!Input.anyKeyDown)
				return false;
			return Input.inputString == "+";
		}

		/// <summary>
		/// Sets the objectspawner to the given one
		/// </summary>
		/// <param name="objectSpawner">The new object spawner</param>
		public virtual void SetObjectSpawner(ObjectSpawner objectSpawner) {
			ObjectSpawner = objectSpawner;
		}

	}

}