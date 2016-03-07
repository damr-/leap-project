using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UniRx;
using System.Collections.Generic;
using PXL.Utility;

namespace PXL.UI {

	public class ObjectShapeChanger : AdminDropdownUI {

		[Serializable]
		public struct ObjectShape {
			public GameObject obj;
			public string name;

			public ObjectShape(GameObject o, string n) {
				obj = o;
				name = n;
			}
		}

		/// <summary>
		/// Whether a random shape has to be chosen every time an object is spawned
		/// </summary>
		protected bool chooseRandomShape = false;
		
		/// <summary>
		/// All selectable objects
		/// </summary>
		public ObjectShape[] availableObjects;
		
		/// <summary>
		/// All the keys used for switching between shapes
		/// </summary>
		protected List<KeyCode> changeShapeKeys = new List<KeyCode> {
			KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6
		};

		protected override void Start() {
			base.Start();
			objectManager.SpawnInitiated.Subscribe(_ => RandomiseIfNeeded());
		}
		
		protected virtual void Update() {
			if (!AdminUIBase.IsAdmin)
				return;
			foreach (var item in changeShapeKeys.Select((value, index) => new { index, value })) {
				CheckKey(item.value, item.index);
			}
		}
		
		/// <summary>
		/// Checks whether the given key is pressed. If yes, sets the value of the dropdown menu
		/// </summary>
		private void CheckKey(KeyCode key, int index) {
			if (Input.GetKeyDown(key))
				dropdown.value = index;
		}
		
		/// <summary>
		/// Sets a new object in the ObjectManager
		/// </summary>
		/// <param name="menuIndex">The new index of the menu</param>
		public void SelectionChanged(int menuIndex) {
			GameObject newPrefab = availableObjects.ElementAt(menuIndex).obj;
			objectManager.SetObjectPrefab(newPrefab);

			chooseRandomShape = (menuIndex == 0) ? true : false;
		}
		
		/// <summary>
		/// Adds all object entries to the dropdown list
		/// </summary>
		protected override void AddDropdownEntries() {
			List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
			foreach (ObjectShape entry in availableObjects) {
				optionsList.Add(new Dropdown.OptionData(entry.name));
			}
			dropdown.AddOptions(optionsList);
		}
		
		/// <summary>
		/// Change the currently used shape to a random one if necessary
		/// </summary>
		protected virtual void RandomiseIfNeeded() {
			if (chooseRandomShape)
				objectManager.SetObjectPrefab(GetRandomShape());
		}
		
		/// <summary>
		/// Returns a random shape
		/// </summary>
		public GameObject GetRandomShape() {		
			return availableObjects.GetRandomElement(1).obj;
		}
	}

}
