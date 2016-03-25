using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public abstract class PropertyChanger : AdminUiBase {

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
		/// The Text component where the title is displayed
		/// </summary>
		public Text PropertyLabelText;

		/// <summary>
		/// The Image component of the background/panel
		/// </summary>
		public Image BackgroundImage;

		/// <summary>
		/// Color for the <see cref="BackgroundImage"/> when this PropertyChanger is selected
		/// </summary>
		public Color SelectedColor = new Color(255/255f, 201/255f, 133/255f, 197/255f);

		/// <summary>
		/// The default background color of <see cref="BackgroundImage"/>
		/// </summary>
		private readonly Color defaultColor = new Color(1f, 1f, 1f, 100/255f);

		/// <summary>
		/// Whether this PropertyChanger is currently selected
		/// </summary>
		public bool IsSelected {
			get { return mIsSelected; }
			set {
				mIsSelected = value;
				BackgroundImage.color = value ? SelectedColor : defaultColor;
			}
		}
		private bool mIsSelected;

		protected override void Start() {
			base.Start();
			AssertReferences();
		}

		/// <summary>
		/// Checks for active admin mode and key strokes
		/// </summary>
		protected virtual void Update() {
			if (!IsAdmin || !IsSelected)
				return;

			if ((IncreaseKey == KeyCode.Plus && IsPlusKeyDown()) || Input.GetKeyDown(IncreaseKey)) {
				IncreaseButton.onClick.Invoke();
			}

			if (DecreaseKey == KeyCode.Plus && IsPlusKeyDown() || Input.GetKeyDown(DecreaseKey)) {
				DecreaseButton.onClick.Invoke();
			}
		}

		protected virtual void AssertReferences() {
			DecreaseButton.AssertNotNull("Decrease button missing");
			IncreaseButton.AssertNotNull("Increase button missing");
			PropertyText.AssertNotNull("Property text missing");
			PropertyLabelText.AssertNotNull("Property label missing");
			BackgroundImage.AssertNotNull("Background image missing");
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