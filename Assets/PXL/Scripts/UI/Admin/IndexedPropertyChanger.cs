namespace PXL.UI.Admin {

	public abstract class IndexedPropertyChanger : PropertyChanger {

		/// <summary>
		/// The index of the currently selected property
		/// </summary>
		protected int CurrentPropertyIndex;

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