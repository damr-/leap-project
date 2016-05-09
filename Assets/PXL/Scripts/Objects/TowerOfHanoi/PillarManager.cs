﻿using System.Collections.Generic;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.TowerOfHanoi {

	public class PillarManager : MonoBehaviour {

		/// <summary>
		/// The pillars in the scene, from left to right
		/// </summary>
		public List<Grabbable> Pillars = new List<Grabbable>();

		/// <summary>
		/// The default positions of the three pillars
		/// </summary>
		private readonly List<Vector3> defaultPillarPositions = new List<Vector3> {
			new Vector3(-0.2203f, Y, Z),
			new Vector3(0.041f, Y, Z),
			new Vector3(0.297f, Y, Z)
		};

		/// <summary>
		/// The default y position of the pillars
		/// </summary>
		private const float Y = -0.179f;

		/// <summary>
		/// The default z position of the pillars
		/// </summary>
		private const float Z = 0.056f;

		/// <summary>
		/// Disposables for the pillars' grabbed state
		/// </summary>
		private readonly CompositeDisposable pillarsCompositeDisposable = new CompositeDisposable();

		/// <summary>
		/// Checks if there's exactly three pillars and loads their positions
		/// </summary>
		private void Start() {
			if (Pillars.Count != 3)
				throw new MissingReferenceException("Not exactly 3 pillars given!");

			foreach (var p in Pillars) {
				p.AssertNotNull("Missing pillar reference!");
				p.IsGrabbed.Subscribe(grabbed => {
					if (grabbed)
						return;
					SavePillarPositions();
				});
			}

			LoadPillarPositions();
		}

		/// <summary>
		/// Loads the positions for the pillars from the PlayerPrefs and sets them
		/// </summary>
		private void LoadPillarPositions() {
			for (var pillarIndex = 0; pillarIndex < Pillars.Count; pillarIndex++) {
				var pillarPos = defaultPillarPositions[pillarIndex];

				var prefValue = PlayerPrefs.GetString(GetPrefKeyString(pillarIndex), "");

				if (prefValue != "")
					pillarPos = GetPrefValueAsVector(prefValue);

				Pillars[pillarIndex].transform.position = pillarPos;
			}
		}

		/// <summary>
		/// Saves the current positions of the pillars to the PlayerPrefs
		/// </summary>
		private void SavePillarPositions() {
			for (var pillarIndex = 0; pillarIndex < Pillars.Count; pillarIndex++) {
				PlayerPrefs.SetString(
					GetPrefKeyString(pillarIndex),
					GetPrefValueString(Pillars[pillarIndex].transform.position)
					);
			}
		}

		/// <summary>
		/// Resets the positions of the pillars to their default ones
		/// </summary>
		public void ResetPillarPositions() {
			for (var pillarIndex = 0; pillarIndex < Pillars.Count; pillarIndex++) {
				PlayerPrefs.DeleteKey(GetPrefKeyString(pillarIndex));
				Pillars[pillarIndex].transform.position = defaultPillarPositions[pillarIndex];
			}
		}

		/// <summary>
		/// Returns the string for the PlayerPrefs for the pillar with the given index
		/// </summary>
		private string GetPrefKeyString(int pillarIndex) {
			return "P" + pillarIndex;
		}

		/// <summary>
		/// Returns the string for the PlayerPrefs value storing the given vector
		/// </summary>
		private string GetPrefValueString(Vector3 pillarPos) {
			return pillarPos.x + "|" + pillarPos.y + "|" + pillarPos.z;
		}

		/// <summary>
		/// Returns the Vector which is stored in the given PlayerPrefs value string
		/// </summary>
		private Vector3 GetPrefValueAsVector(string prefValue) {
			var splitValue = prefValue.Split('|');
			return new Vector3(
				float.Parse(splitValue[0]),
				float.Parse(splitValue[1]),
				float.Parse(splitValue[2])
			);
		}

		/// <summary>
		/// Disposes the composite disposable
		/// </summary>
		private void OnDisable() {
			pillarsCompositeDisposable.Dispose();
		}

	}

}