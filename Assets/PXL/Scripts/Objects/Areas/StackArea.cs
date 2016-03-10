using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;

namespace PXL.Objects.Areas {

	public class StackArea : ObjectArea {

		/// <summary>
		/// Number of required objects to be stacked upon each other
		/// </summary>
		public int RequiredObjectsAmount = 3;

		/// <summary>
		/// The objectManagers which spawned the cubes
		/// </summary>
		public List<ObjectManager> ObjectManagers = new List<ObjectManager>();

		/// <summary>
		/// List of sorted objects, by scale
		/// </summary>
		public List<ObjectBehaviour> SortedObjects = new List<ObjectBehaviour>();

		/// <summary>
		/// The color of the light on success
		/// </summary>
		public Color SuccessColor;

		/// <summary>
		/// The light of the area
		/// </summary>
		public Light AreaLight;

		/// <summary>
		/// The gamemode of this level
		/// </summary>
		public GameMode GameMode;

		protected virtual void Start() {
			if (ObjectManagers.Count == 0) {
				throw new MissingReferenceException("There are no ObjectManagers assigned!");
			}

			AreaLight.AssertNotNull("The area light is missing");
			GameMode.AssertNotNull("GameMode reference is missing!");
		}

		protected virtual void Update() {
			if (GameMode.GameWon || SortedObjects.Count != RequiredObjectsAmount)
				return;

			SortObjectsIfNeeded();
			if (!StackedCorrecly() || !AllObjectsDropped())
				return;

			HandleGameWon();
		}

		/// <summary>
		/// Returns whether all objects are stacked correctly
		/// </summary>
		/// <returns>True if each item is at a higher vertical position, the smaller it is</returns>
		protected virtual bool StackedCorrecly() {
			for (var i = 0; i < SortedObjects.Count - 1; i++) {
				var o = SortedObjects.ElementAt(i);
				var next = SortedObjects.ElementAt(i + 1);
				if (o.Scale <= next.Scale) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Sorts the objects inside the area 
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
		protected virtual bool AllObjectsDropped() {
			var result = SortedObjects.Select(o => o.GetComponent<Grabbable>()).All(grabbable => !grabbable.IsGrabbed);
			result &= SortedObjects.Select(o => o.GetComponent<Rigidbody>()).All(r => r.velocity.Equal(Vector3.zero));
			return result;
		}

		/// <summary>
		/// Adds the new object to the sorted objects List
		/// </summary>
		protected override void HandleValidObjectType(ObjectBehaviour objectBehaviour) {
			if (!SortedObjects.Contains(objectBehaviour))
				SortedObjects.Add(objectBehaviour);
		}

		/// <summary>
		/// Removes the object from the sorted list
		/// </summary>
		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit(other);
			var objectBehaviour = other.GetComponent<ObjectBehaviour>();
			if (objectBehaviour != null && SortedObjects.Contains(objectBehaviour))
				SortedObjects.Remove(objectBehaviour);
		}

		/// <summary>
		/// Called when the game is won
		/// </summary>
		protected virtual void HandleGameWon() {
			GameMode.GameOver(true);
			SortedObjects.Clear();
			AreaLight.color = SuccessColor;
		}
	}

}