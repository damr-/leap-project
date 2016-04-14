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
		/// The Rect Transform of the admin panel
		/// </summary>
		public RectTransform AdminPanelTransform;

		/// <summary>
		/// The index of the currently selected element
		/// </summary>
		private int currentElementIndex;

		/// <summary>
		/// All known elements
		/// </summary>
		private List<SpawnerElement> elements = new List<SpawnerElement>();

		private IDisposable disposable = Disposable.Empty;

		private void Start() {
			SetupSpawnerElements.AssertNotNull("Missing SetupSpawnerElements reference!");
			AdminPanelTransform.AssertNotNull("Missing Admin Panel RectTransform");

			disposable = Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => {
				//Debug.Log("Seems like elements have already been spawned. Manually retreiving elements.");
				elements = GetComponentsInChildren<SpawnerElement>().ToList();
				SetCurrentElement(0);
			});

			SetupSpawnerElements.SpawnFinished.Subscribe(spawnedElements => {
				elements = spawnedElements;
				SetCurrentElement(0);
				disposable.Dispose();
			});

		}

		private void Update() {
			if (Input.GetKeyDown(NextElementKey)) {
				if (currentElement.Expanded) {
					if (currentElement.SpawnerElementExpanded.TrySelectNextEntry())
						return;
					if (currentElementIndex < elements.Count - 1)
						CollapseElement(currentElement);
				}
				SetCurrentElement(currentElementIndex + 1);
			}
			if (Input.GetKeyDown(PreviousElementKey)) {
				if (currentElement.Expanded) {
					if (currentElement.SpawnerElementExpanded.TrySelectPreviousEntry())
						return;
					if (currentElementIndex > 0)
						CollapseElement(currentElement);
				}
				SetCurrentElement(currentElementIndex - 1);
			}
			if (Input.GetKeyDown(ToggleExpandElementKey)) {
				if (currentElement.Expanded)
					CollapseElement(currentElement);
				else
					ExpandElement(currentElement);
			}
			if (Input.GetKeyDown(CollapseElementKey)) {
				if (currentElement.Expanded)
					CollapseElement(currentElement);
			}
		}

		/// <summary>
		/// Collapses the given element and reduces the size of the background
		/// </summary>
		private void CollapseElement(SpawnerElement element) {
			var r = AdminPanelTransform.sizeDelta;
			AdminPanelTransform.sizeDelta = new Vector2(r.x, r.y - 50);
			element.Collapse();
		}

		/// <summary>
		/// Expandes the given element and increases the size of the background
		/// </summary>
		/// <param name="element"></param>
		private void ExpandElement(SpawnerElement element) {
			var r = AdminPanelTransform.sizeDelta;
			AdminPanelTransform.sizeDelta = new Vector2(r.x, r.y + 50);
			element.Expand();
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
		public void ExpandOtherElement(SpawnerElement element) {
			CollapseElement(currentElement);
			SetCurrentElement(element);
			ExpandElement(currentElement);
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