using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public abstract class AdminDropdownUI : AdminUIBase {

	/**
	* The dropdown component of the child UI element
	*/
	protected Dropdown dropdown;
	
	protected override void Start() {
		base.Start();

		dropdown = GetComponentInChildren<Dropdown>();
		dropdown.ClearOptions();

		AddDropdownEntries();
	}

	protected abstract void AddDropdownEntries();

	/**
	* Returns a random entry of the dropdown list
	*/
	protected virtual T GetRandomEntry<T>(T[] entries) where T : struct{
		int index = Random.Range(1, entries.Length);
		return entries.ElementAt(index);
	}
}
