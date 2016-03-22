﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;
using UniRx.Triggers;

namespace PXL.Objects.Areas {

	public class StackArea : ObjectArea {

		/// <summary>
		/// Number of required objects to be stacked upon each other
		/// </summary>
		public int RequiredObjectsAmount = 3;

		/// <summary>
		/// List of sorted objects, by scale
		/// </summary>
		protected List<InteractiveObject> SortedObjects = new List<InteractiveObject>();

		/// <summary>
		/// The color of the light on success
		/// </summary>
		public Color SuccessColor;

		/// <summary>
		/// The light of the area
		/// </summary>
		public Light AreaLight;

		protected virtual void Start() {
			AreaLight.AssertNotNull("The area light is missing");
		}

		protected override void UpdateArea() {
			base.UpdateArea();

			SortedObjects = SortedObjects.Purge();

			if (GameMode.GameWon || !AreaActive || SortedObjects.Count != RequiredObjectsAmount)
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
			return SortedObjects.Select(o => o.GetComponent<Grabbable>()).All(grabbable => grabbable.IsStationary);
		}

		/// <summary>
		/// Adds the new object to <see cref="SortedObjects"/>
		/// </summary>
		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
			if (!SortedObjects.Contains(interactiveObject))
				SortedObjects.Add(interactiveObject);
		}

		/// <summary>
		/// <see cref="StackArea"/> doesn't worry about invalid object types
		/// </summary>
		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {}

		/// <summary>
		/// Removes the object from <see cref="SortedObjects"/>
		/// </summary>
		protected override void OnTriggerExit(Collider other) {
			var interactiveObject = other.GetComponent<InteractiveObject>();
			if (interactiveObject != null && SortedObjects.Contains(interactiveObject)) {
				SortedObjects.Remove(interactiveObject);
			}
			
			base.OnTriggerExit(other);
		}

		/// <summary>
		/// Called when the game is won
		/// </summary>
		protected virtual void HandleGameWon() {
			GameMode.SetGameOver(true);
			SortedObjects.Clear();
			AreaLight.color = SuccessColor;
		}
	}

}