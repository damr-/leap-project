using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SwitchSpawnerElement : MonoBehaviour {

		/// <summary>
		/// The key used to select the next element
		/// </summary>
		protected readonly KeyCode NextElementKey = KeyCode.DownArrow;

		/// <summary>
		/// The key used to select the previous element
		/// </summary>
		protected readonly KeyCode PreviousElementKey = KeyCode.UpArrow;

		/// <summary>
		/// The currently selected element
		/// </summary>
		private SpawnerElement currentElement;

		/// <summary>
		/// The index of the currently selected element
		/// </summary>
		private int currentIndex;

		/// <summary>
		/// All known elements
		/// </summary>
		private List<SpawnerElement> elements = new List<SpawnerElement>();

		private void Start() {
			Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => {
				elements = GetComponentsInChildren<SpawnerElement>().ToList();
				if (elements.Count == 0)
					return;
				SetCurrentElement(0);
			});
		}

		private void Update() {
			if (Input.GetKeyDown(NextElementKey)) {
				SetCurrentElement(currentIndex + 1);
			}
			if (Input.GetKeyDown(PreviousElementKey)) {
				SetCurrentElement(currentIndex - 1);
			}
		}

		/// <summary>
		/// Sets the element with the given index as the currently selected one
		/// </summary>
		private void SetCurrentElement(int index) {
			if (index < 0 || index >= elements.Count)
				return;

			if (currentElement != null)
				currentElement.Deselect();
			currentElement = elements.ElementAt(index);
			currentElement.Select();
			currentIndex = index;
		}

	}

}