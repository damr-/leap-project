using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PXL.UI.Admin {

	public class SwitchPropertyChanger : MonoBehaviour {

		/// <summary>
		/// The key used to select the next property changer
		/// </summary>
		protected readonly KeyCode NextPropertyChangerKey = KeyCode.DownArrow;

		/// <summary>
		/// The key used to select the previous property changer
		/// </summary>
		protected readonly KeyCode PreviousPropertyChangerKey = KeyCode.UpArrow;

		/// <summary>
		/// All known property changers
		/// </summary>
		private List<PropertyChanger> propertyChangers = new List<PropertyChanger>();

		/// <summary>
		/// The currently selected property changer
		/// </summary>
		private PropertyChanger currentPropertyChanger;

		/// <summary>
		/// The index of the currently selected property changer
		/// </summary>
		private int currentIndex;

		private void Start() {
			propertyChangers = GetComponentsInChildren<PropertyChanger>().ToList();
			if (propertyChangers.Count == 0)
				return;
			SetCurrentPropertyChanger(0);
		}

		private void Update() {
			if (Input.GetKeyDown(NextPropertyChangerKey)) {
				SetCurrentPropertyChanger(currentIndex + 1);
			}
			if (Input.GetKeyDown(PreviousPropertyChangerKey)) {
				SetCurrentPropertyChanger(currentIndex - 1);
			}
		}

		private void SetCurrentPropertyChanger(int index) {
			if (index < 0 || index >= propertyChangers.Count)
				return;

			if (currentPropertyChanger != null)
				currentPropertyChanger.IsSelected = false;
			currentPropertyChanger = propertyChangers.ElementAt(index);
			currentPropertyChanger.IsSelected = true;
			currentIndex = index;
		}

	}

}