using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UniRx;
using System.Collections.Generic;

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

	protected bool chooseRandomObject = false;

	/**
	* All selectable objects
	*/
	public ObjectShape[] availableObjects;
	
	protected override void Start() {
		base.Start();

		objectManager.SpawnInitiated.Subscribe(_ => RandomiseIfNeeded());

		dropdown.value = 1;
	}

	protected virtual void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1))
			dropdown.value = 0;
		if (Input.GetKeyDown(KeyCode.Alpha2))
			dropdown.value = 1;
		if (Input.GetKeyDown(KeyCode.Alpha3))
			dropdown.value = 2;
		if (Input.GetKeyDown(KeyCode.Alpha4))
			dropdown.value = 3;
		if (Input.GetKeyDown(KeyCode.Alpha5))
			dropdown.value = 4;
		if (Input.GetKeyDown(KeyCode.Alpha6))
			dropdown.value = 5;
	}

	/**
	* Sets a new object in the ObjectManager
	*/
	public void SelectionChanged(int menuIndex) {
        GameObject newPrefab = availableObjects.ElementAt(menuIndex).obj;
		objectManager.SetObjectPrefab(newPrefab);

		chooseRandomObject = (menuIndex == 0) ? true : false;
	}
	
	/**
	* Returns a random object
	*/
	public GameObject GetRandomObject() {
		return GetRandomEntry(availableObjects).obj;
	}

	/**
	* Adds all object entries
	*/
	protected override void AddDropdownEntries() {
		List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
		foreach (ObjectShape entry in availableObjects) {
			optionsList.Add(new Dropdown.OptionData(entry.name));
		}
		dropdown.AddOptions(optionsList);
	}

	/**
	* Change the currently used shape to a random one if necessary
	*/
	protected virtual void RandomiseIfNeeded() {
		if (chooseRandomObject)
			objectManager.SetObjectPrefab(GetRandomObject());
	}
}
