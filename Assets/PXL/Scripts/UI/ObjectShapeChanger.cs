using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UniRx;
using System.Collections.Generic;
using PXL.Utility;

namespace PXL.UI {

	public class ObjectShapeChanger : AdminDropdownUi {

		[Serializable]
		public struct ObjectShape {
			public GameObject Obj;
			public string Name;

			public ObjectShape(GameObject o, string n) {
				Obj = o;
				Name = n;
			}
		}

		/// <summary>
		/// Whether a random shape has to be chosen every time an object is spawned
		/// </summary>
		protected bool ChooseRandomShape;

		/// <summary>
		/// The text element of the label
		/// </summary>
		public Text LabelText;

		/// <summary>
		/// All selectable objects
		/// </summary>
		public ObjectShape[] AvailableObjects;

		/// <summary>
		/// All the keys used for switching between shapes
		/// </summary>
		protected List<KeyCode> ChangeShapeKeys = new List<KeyCode>();

		/// <summary>
		/// All available keys for changing the shape
		/// </summary>
		protected readonly List<KeyCode> AvailableShapeKeys = new List<KeyCode> {
			KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
		};

		protected override void Start() {
			base.Start();

			LabelText.AssertNotNull("Missing the Text component of the shape label");

			if (AvailableObjects.Length > AvailableShapeKeys.Count)
				throw new IndexOutOfRangeException("There are too many shapes and too few keys specified!");

			for (var i = 0; i < AvailableObjects.Length; i++) {
				ChangeShapeKeys.Add(AvailableShapeKeys.ElementAt(i));
			}

			LabelText.text += AvailableObjects.Length%10 + ")";

			ObjectManager.SpawnInitiated.Subscribe(_ => RandomiseIfNeeded());
		}
		
		protected virtual void Update() {
			if (!IsAdmin)
				return;
			foreach (var item in ChangeShapeKeys.Select((value, index) => new { index, value })) {
				CheckKey(item.value, item.index);
			}
		}
		
		/// <summary>
		/// Checks whether the given key is pressed. If yes, sets the value of the dropdown menu
		/// </summary>
		private void CheckKey(KeyCode key, int index) {
			if (Input.GetKeyDown(key))
				Dropdown.value = index;
		}
		
		/// <summary>
		/// Sets a new object in the ObjectManager
		/// </summary>
		/// <param name="menuIndex">The new index of the menu</param>
		public void SelectionChanged(int menuIndex) {
			var newPrefab = AvailableObjects.ElementAt(menuIndex).Obj;
			ObjectManager.SetObjectPrefab(newPrefab);

			ChooseRandomShape = menuIndex == 0;
		}
		
		/// <summary>
		/// Adds all object entries to the dropdown list
		/// </summary>
		protected override void AddDropdownEntries() {
			var optionsList = AvailableObjects.Select(entry => new Dropdown.OptionData(entry.Name)).ToList();
			Dropdown.AddOptions(optionsList);
		}
		
		/// <summary>
		/// Change the currently used shape to a random one if necessary
		/// </summary>
		protected virtual void RandomiseIfNeeded() {
			if (ChooseRandomShape)
				ObjectManager.SetObjectPrefab(GetRandomShape());
		}
		
		/// <summary>
		/// Returns a random shape
		/// </summary>
		public GameObject GetRandomShape() {		
			return AvailableObjects.GetRandomElement(1).Obj;
		}
	}

}
