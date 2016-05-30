using System.Collections.Generic;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.WhackAMole {

	/// <summary>
	/// This script manages the spawning of moles and handles the occupation of spawnpoints
	/// as well as providing a free spawnpoint for a mole to move to.
	/// </summary>
	public class WhackAMoleManager : MonoBehaviour {

		/// <summary>
		/// The object prefab for the mole
		/// </summary>
		public GameObject MolePrefab;

		/// <summary>
		/// The amount of moles which will be spawned at the beginning
		/// </summary>
		[Range(1, 10)]
		public int MoleCount = 1;

		/// <summary>
		/// The possible spawnpoints for the moles
		/// </summary>
		public List<Transform> SpawnPoints = new List<Transform>();

		/// <summary>
		/// The spawnpoints which are currently occupied by moles
		/// </summary>
		private readonly List<Transform> occupiedSpawnPoints = new List<Transform>();

		/// <summary>
		/// Subscriptions to the moles' death observables for spawning a new one
		/// </summary>
		private readonly CompositeDisposable moleDisposables = new CompositeDisposable();

		private void Start() {
			MolePrefab.AssertNotNull("Missing mole prefab!");
			if (SpawnPoints.Count == 0)
				throw new MissingReferenceException("Missing spawnpoints!");

			for (var i = 0; i < MoleCount; i++)
				CreateNewMole();
		}
		
		public void CreateNewMole() {
			var mole = SimplePool.Spawn(MolePrefab, Vector3.up * 10f, Quaternion.identity);
			mole.GetComponent<Mole>().Setup(this);
		}

		/// <summary>
		/// Returns the transform of a random, unoccupied spawnpoint
		/// </summary>
		public Transform GetRandomFreeSpawnPoint() {
			Transform s;
			var counter = 0;
			do {
				s = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
				if (++counter > SpawnPoints.Count)
					return null;
			} while (occupiedSpawnPoints.Contains(s));

			occupiedSpawnPoints.Add(s);
			return s;
		}

		/// <summary>
		/// Tell the manager that the given spawnpoint prefab is unoccupied again 
		/// </summary>
		public void FreeSpawnPoint(Transform spawnPoint) {
			occupiedSpawnPoints.Remove(spawnPoint);
		}

		/// <summary>
		/// Dispose the subscriptions to the moles' death observables
		/// </summary>
		private void OnDisable() {
			moleDisposables.Dispose();
		}

	}

}