using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;

namespace PXL.Objects.Areas {

	/// <summary>
	/// This script describes an area in which all objects have to be stacked upon each other, 
	/// decreasing in scale while the vertical position is increasing.
	/// </summary>
	public class StackArea : ObjectArea {

		/// <summary>
		/// The current status of the objects inside the area
		/// </summary>
		public enum Status {
			NotStationary = 0,
			StackedIcorrectly = 1,
			NotEnoughObjects = 2,
			Checking = 3,
			GameWon = 4
		}

		/// <summary>
		/// Number of required objects to be stacked upon each other
		/// </summary>
		public int RequiredObjectsAmount = 3;

		/// <summary>
		/// The maximum velocity magnitude a rigidbody can have to be called stationary
		/// </summary>
		public float StationaryEpsilon = 0.001f;

		/// <summary>
		/// List of sorted objects, by scale
		/// </summary>
		public List<InteractiveObject> SortedObjects = new List<InteractiveObject>();

		/// <summary>
		/// The current status of this area
		/// </summary>
		public ObservableProperty<Status> AreaStatus = new ObservableProperty<Status>();

		/// <summary>
		/// The incorrectly placed object
		/// </summary>
		public InteractiveObject IncorrectObject;

		protected override void Update() {
			base.Update();

			if (GameState.GameOver || !AreaActive)
				return;

			SortedObjects = Objects.Select(o => InteractiveObjects.GetOrAddFromGameObject(o.Key)).ToList();

			SortObjectsIfNeeded();

			if (!AllObjectsStationary()) {
				SetStatus(Status.NotStationary);
				return;
			}

			InteractiveObject wrongObject;
			if (!StackedCorrecly(out wrongObject)) {
				IncorrectObject = wrongObject;
				SetStatus(Status.StackedIcorrectly);
				return;
			}

			if (SortedObjects.Count < RequiredObjectsAmount) {
				SetStatus(Status.NotEnoughObjects);
				return;
			}

			SetStatus(Status.Checking);

			HandleStackedCorrectly();
		}

		/// <summary>
		/// Called when the objective has been reached
		/// </summary>
		protected virtual void HandleStackedCorrectly() {
			SetStatus(Status.GameWon);
			SortedObjects.Clear();
		}

		/// <summary>
		/// Sets the status to the given one, if it's not the same
		/// </summary>
		protected void SetStatus(Status newStatus) {
			if (AreaStatus.Value != newStatus)
				AreaStatus.Value = newStatus;
		}

		/// <summary>
		/// Returns whether all objects are stacked correctly
		/// </summary>
		protected virtual bool StackedCorrecly(out InteractiveObject wrongObject) {
			for (var i = 0; i < SortedObjects.Count - 1; i++) {
				var currentObject = SortedObjects.ElementAt(i);
				var nextObject = SortedObjects.ElementAt(i + 1);

				if (!(currentObject.Scale <= nextObject.Scale))
					continue;

				wrongObject = nextObject;
				return false;
			}
			wrongObject = null;
			return true;
		}

		/// <summary>
		/// Sorts the objects inside the area by their Y-coordinate, ascending
		/// </summary>
		protected virtual void SortObjectsIfNeeded() {
			if (IsSortNeeded())
				SortedObjects = SortedObjects.OrderBy(o => o.transform.position.y).ToList();
		}

		/// <summary>
		/// Returns whether sorting the objects is needed
		/// </summary>
		protected virtual bool IsSortNeeded() {
			for (var i = 0; i < SortedObjects.Count - 1; i++) {
				if (SortedObjects[i].transform.position.y > SortedObjects[i + 1].transform.position.y)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns whether all objects in the area have been dropped
		/// </summary>
		/// <returns>True if no objects is still grabbed, false if otherwise</returns>
		protected virtual bool AllObjectsStationary() {
			return SortedObjects.Select(o => o.GetComponent<Grabbable>()).All(grabbable => grabbable.IsStationary(StationaryEpsilon));
		}

		protected override void HandleValidObjectType(InteractiveObject interactiveObject) { }

		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) { }

	}

}