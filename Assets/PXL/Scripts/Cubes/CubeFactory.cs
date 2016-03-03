using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CubeFactory : Factory {

	/**
	* Scale of the cube
	*/
	public float scale { get; set; }

	/**
	* Color of the cube
	*/
	public Color color { get; set; }
	
	/**
	* Spawns the set prefab at position with scale
	*/
	public override GameObject Spawn() {
		GameObject newCube = base.Spawn();
		newCube.transform.localScale = new Vector3(scale, scale, scale);

		Color cubeColor = color;

		if(color == Color.white) {
			//ignore white (index 0) when choosing random color
			int index = UnityEngine.Random.Range(1, CubeColorPanel.availableColors.Count);
			cubeColor = CubeColorPanel.availableColors.ElementAt(index).Key;
		}

		newCube.GetComponent<Renderer>().material.color = cubeColor;

		return newCube;
	}
}
