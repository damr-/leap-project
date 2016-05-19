using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.WhackAMole {

	public class WhackAMoleManager : MonoBehaviour {

		public GameObject MolePrefab;

		[Range(1, 10)]
		public int MoleCount = 1;

		public List<Transform> SpawnPoints = new List<Transform>();

		private List<Transform> occupiedSpawnPoints = new List<Transform>();

		private void Start() {
			MolePrefab.AssertNotNull("Missing mole prefab!");
			if (SpawnPoints.Count == 0)
				throw new MissingReferenceException("Missing spawnpoints!");

			for (var i = 0; i < MoleCount; i++)
				CreateNewMole();
		}

		private void CreateNewMole() {
			var mole = SimplePool.Spawn(MolePrefab, Vector3.down * 10, Quaternion.identity);
			mole.GetComponent<Mole>().Setup(this);
		}

		public void MoleDespawned() {
			CreateNewMole();
		}

		public Transform GetRandomFreeSpawnPoint() {
			Transform s;
			var counter = 0;
			do {
				s = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
				if (++counter > SpawnPoints.Count) {
					return null;
				}
			} while (occupiedSpawnPoints.Contains(s));

			occupiedSpawnPoints.Add(s);
			return s;
		}

		public void MoleLeftSpawnPoint(Transform spawnPoint) {
			occupiedSpawnPoints.Remove(spawnPoint);
		}

	}

}