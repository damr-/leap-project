using System.Diagnostics;
using Random = UnityEngine.Random;

namespace PXL.Objects.Spawner {

	public class AutomatedObjectSpawner : ObjectSpawner {

		/// <summary>
		/// How many times per second an object will be spawned
		/// </summary>
		public float SpawnFrequency = 0.5f;

		/// <summary>
		/// Whether <see cref="SpawnFrequency"/> should be applied or a random one 
		/// between <see cref="MinSpawnFrequency"/> and <see cref="MaxSpawnFrequency"/> should be chosen every time.
		/// </summary>
		public bool ChooseRandomly;

		/// <summary>
		/// The maximum possible spawn frequency, if choosing randomly
		/// </summary>
		public float MaxSpawnFrequency = 2f;

		/// <summary>
		/// The minimum possible spawn frequency, if choosing randomly
		/// </summary>
		public float MinSpawnFrequency = 0.2f;

		/// <summary>
		/// The currently used frequency to spawn objects
		/// </summary>
		protected float CurrentSpawnFrequency;

		/// <summary>
		/// Last time an object was spawned
		/// </summary>
		private float lastSpawnTime;

		protected override void Start() {
			base.Start();
			SetRandomSpawnFrequencyIfNecessary();
		}

		protected override void Update() {
			base.Update();

			if (UnityEngine.Time.time - lastSpawnTime <= 1 / CurrentSpawnFrequency)
				return;

			SpawnObject();
			SetRandomSpawnFrequencyIfNecessary();
			lastSpawnTime = UnityEngine.Time.time;
		}

		/// <summary>
		/// Checks if a random SpawnFrequency should be calculated and if yes, does so.
		/// If not, sets the frequency to the default one.
		/// </summary>
		protected virtual void SetRandomSpawnFrequencyIfNecessary() {
			if (ChooseRandomly)
				SetRandomSpawnFrequency();
			else if (CurrentSpawnFrequency != SpawnFrequency)
				CurrentSpawnFrequency = SpawnFrequency;
		}

		/// <summary>
		/// Sets <see cref="CurrentSpawnFrequency"/> to a random one between <see cref="MinSpawnFrequency"/> and <see cref="MaxSpawnFrequency"/>
		/// </summary>
		protected virtual void SetRandomSpawnFrequency() {
			CurrentSpawnFrequency = Random.Range(MinSpawnFrequency, MaxSpawnFrequency);
		}
	}

}