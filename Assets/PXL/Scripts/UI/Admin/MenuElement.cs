using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.Admin {

	public class MenuElement : AdminUiBase {

		/// <summary>
		/// The Image component of the background/panel
		/// </summary>
		public Image BackgroundImage;

		/// <summary>
		/// Color for the <see cref="BackgroundImage"/> when this PropertyChanger is selected
		/// </summary>
		public Color SelectedColor = new Color(255 / 255f, 201 / 255f, 133 / 255f, 197 / 255f);

		/// <summary>
		/// The default background color of <see cref="BackgroundImage"/>
		/// </summary>
		protected readonly Color DefaultColor = new Color(1f, 1f, 1f, 100 / 255f);

		/// <summary>
		/// Whether this PropertyChanger is currently selected
		/// </summary>
		public bool IsSelected {
			get { return mIsSelected; }
			set {
				mIsSelected = value;
				if(BackgroundImage != null)
					BackgroundImage.color = value ? SelectedColor : DefaultColor;
			}
		}

		private bool mIsSelected;

		protected override void Start() {
			base.Start();
			AssertReferences();
		}

		/// <summary>
		/// Asserts that all necessary references are set up and throws exceptions if not
		/// </summary>
		protected virtual void AssertReferences() {
		}

	}

}