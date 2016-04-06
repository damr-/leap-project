using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.UI.Admin {

	public class NavigateSpawnerElements : MonoBehaviour {

		/// <summary>
		/// The SetupSpawnerElements component which creates all the elements
		/// </summary>
		public SetupSpawnerElements SetupSpawnerElements;

		/// <summary>
		/// The key used to select the next element
		/// </summary>
		protected readonly KeyCode NextElementKey = KeyCode.DownArrow;

		/// <summary>
		/// The key used to select the previous element
		/// </summary>
		protected readonly KeyCode PreviousElementKey = KeyCode.UpArrow;

		/// <summary>
		/// The key used to expand the current element
		/// </summary>
		protected readonly KeyCode ToggleExpandElementKey = KeyCode.Return;

		/// <summary>
		/// The key used to collapse the current element
		/// </summary>
		protected readonly KeyCode CollapseElementKey = KeyCode.Backspace;

		/// <summary>
		/// The currently selected element
		/// </summary>
		private SpawnerElement currentElement;

		/// <summary>
		/// The index of the currently selected element
		/// </summary>
		private int currentElementIndex;

		/// <summary>
		/// All known elements
		/// </summary>
		private List<SpawnerElement> elements = new List<SpawnerElement>();

		private IDisposable subscription = Disposable.Empty;

		private void Start() {
			SetupSpawnerElements.AssertNotNull("Missing SetupSpawnerElements reference!");

			subscription = Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => {
				Debug.Log("Seems like elements have already been spawned. Manually retreiving elements.");
				elements = GetComponentsInChildren<SpawnerElement>().ToList();
				SetCurrentElement(0);
			});

			SetupSpawnerElements.SpawnFinished.Subscribe(spawnedElements => {
				elements = spawnedElements;
				SetCurrentElement(0);
				subscription.Dispose();
			});

		}

		private void Update() {
			if (Input.GetKeyDown(NextElementKey)) {
				if (currentElement.Expanded) {
					if (currentElement.SpawnerElementExpanded.TrySelectNextEntry())
						return;
					if (currentElementIndex < elements.Count - 1)
						currentElement.Collapse();
				}
				SetCurrentElement(currentElementIndex + 1);
			}
			if (Input.GetKeyDown(PreviousElementKey)) {
				if (currentElement.Expanded) {
					if (currentElement.SpawnerElementExpanded.TrySelectPreviousEntry())
						return;
					if (currentElementIndex > 0)
						currentElement.Collapse();
				}
				SetCurrentElement(currentElementIndex - 1);
			}
			if (Input.GetKeyDown(ToggleExpandElementKey)) {
				if (currentElement.Expanded)
					currentElement.Collapse();
				else
					currentElement.Expand();
			}
			if (Input.GetKeyDown(CollapseElementKey)) {
				currentElement.Collapse();
			}
		}

		/// <summary>
		/// Sets the element with the given index as the currently selected one
		/// </summary>
		private void SetCurrentElement(int index) {
			if (index < 0 || index >= elements.Count)
				return;
			SetCurrentElement(elements.ElementAt(index));
		}

		/// <summary>
		/// Collapses the currently selected elemet, selects the given element expands it.
		/// </summary>
		public void ExpandElement(SpawnerElement element) {
			currentElement.Collapse();
			SetCurrentElement(element);
			currentElement.Expand();
		}

		/// <summary>
		/// Deselects the currently selected element, sets the given element as the current one and selects it
		/// </summary>
		/// <param name="newElement"></param>
		private void SetCurrentElement(SpawnerElement newElement) {
			if (currentElement != null)
				currentElement.Deselect();

			currentElement = newElement;
			currentElement.Select();
			currentElementIndex = elements.IndexOf(newElement);
		}

	}

}