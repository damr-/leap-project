namespace PXL.Objects.Areas {

	/// <summary>
	/// A StackArea which doesn't automatically change it's <see cref="StackArea.Status"/> when the objects are stacked correctly. 
	/// Instead this has to be done by calling <see cref="SetAreaStackedCorrectly"/> manually
	/// </summary>
	public class CheckingStackArea : StackArea {

		protected override void HandleStackedCorrectly() {}

		/// <summary>
		/// Marks the area as stacked correctly
		/// </summary>
		public void SetAreaStackedCorrectly() {
			SetStatus(Status.GameWon);
			SortedObjects.Clear();
		}

	}

}