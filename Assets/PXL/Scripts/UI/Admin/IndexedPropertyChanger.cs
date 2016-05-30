namespace PXL.UI.Admin {

	/// <summary>
	/// A <see cref="PropertyChanger"/> whici changes the index when the value is in/decreased.
	/// 
	/// Can be used to cycle through arrays or collections.
	/// </summary>
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