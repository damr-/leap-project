using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Areas {

	public class StackArea : ObjectArea {

		/// <summary>
		/// The light of the area
		/// </summary>
		public Light AreaLight;

		/// <summary>
		/// Number of required objects to be stacked upon each other
		/// </summary>
		public int RequiredObjectsAmount = 3;

		/// <summary>
		/// List of sorted objects, by scale
		/// </summary>
		protected List<InteractiveObject> SortedObjects = new List<InteractiveObject>();

		/// <summary>
		/// The maximum velocity magnitude a rigidbody can have to be called stationary
		/// </summary>
		public float StationaryEpsilon = 0.05f;

		public IObservable<Unit> StackedCorrectly { get { return stackedCorrectlySubject; } }
		private readonly ISubject<Unit> stackedCorrectlySubject = new Subject<Unit>();

		public IObservable<InteractiveObject> StackedIncorrectly { get { return stackedIncorrectlySubject; } }
		private readonly ISubject<InteractiveObject> stackedIncorrectlySubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Whether we can call the StackedIncorrectly Observable
		/// </summary>
		private bool canInvokeIncorrectObservable = true;

		/// <summary>
		/// Subscription for calling the StackedIncorrectly Observable
		/// </summary>
		private IDisposable refreshSubscription = Disposable.Empty;

		protected virtual void Start() {
			AreaLight.AssertNotNull("The area light is missing");
		}

		protected override void Update() {
			base.Update();

			if (GameMode.GameOver || !AreaActive)
				return;

			SortedObjects = Objects.Select(o => InteractiveObjects.GetOrAddFromGameObject(o.Key)).ToList();

			SortObjectsIfNeeded();

			if (!AllObjectsDropped()) {
				return;
			}

			InteractiveObject wrongObject;
			if (!StackedCorrecly(out wrongObject)) {
				if (canInvokeIncorrectObservable) {
					stackedIncorrectlySubject.OnNext(wrongObject);
					canInvokeIncorrectObservable = false;
					refreshSubscription.Dispose();
					refreshSubscription = Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
						canInvokeIncorrectObservable = true;
					});
				}
				return;
			}
			if (Objects.Count != RequiredObjectsAmount) {
				return;
			}

			stackedCorrectlySubject.OnNext(Unit.Default);
			SortedObjects.Clear();
		}

		/// <summary>
		/// Returns whether all objects are stacked correctly
		/// </summary>
		protected virtual bool StackedCorrecly(out InteractiveObject wrongObject) {
			wrongObject = null;
			for (var i = 0; i < SortedObjects.Count - 1; i++) {
				var o = SortedObjects.ElementAt(i);
				var next = SortedObjects.ElementAt(i + 1);

				if (o.Scale <= next.Scale) {
					wrongObject = next;
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Sorts the objects inside the area
		/// </summary>
		protected virtual void SortObjectsIfNeeded() {
			if (!IsSortNeeded()) {
				SortedObjects = SortedObjects.OrderBy(o => o.transform.position.y).ToList();
			}
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
			return SortedObjects.Select(o => o.GetComponent<Grabbable>()).All(grabbable => grabbable.IsStationary(StationaryEpsilon));
		}

		/// <summary>
		/// Adds the new object to <see cref="SortedObjects" />
		/// </summary>
		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
		}

		/// <summary>
		/// <see cref="StackArea" /> doesn't worry about invalid object types
		/// </summary>
		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {
		}

	}

}