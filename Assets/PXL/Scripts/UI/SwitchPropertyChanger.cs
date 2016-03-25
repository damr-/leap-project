using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PXL.UI {

	public class SwitchPropertyChanger : MonoBehaviour {

		protected readonly KeyCode NextPropertyChangerKey = KeyCode.DownArrow;

		protected readonly KeyCode PreviousPropertyChangerKey = KeyCode.UpArrow;

		private List<PropertyChanger> propertyChangers = new List<PropertyChanger>();

		private PropertyChanger currentPropertyChanger;

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