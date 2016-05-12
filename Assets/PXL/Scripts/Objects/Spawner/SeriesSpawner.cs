using System;
using System.Collections.Generic;
using UniRx;

namespace PXL.Objects.Spawner {

	public class SeriesSpawner : ObjectSpawner {

		/// <summary>
		/// The scales of the spawned objects
		/// </summary>
		public List<float> Scales = new List<float>();

		/// <summary>
		/// The new scale which will be added to the list via the editor
		/// </summary>
		public float NewScale { get; set; }

		/// <summary>
		/// How many objects of the series are spawned per second
		/// </summary>
		public float SeriesSpawnFrequency = 2f;

		/// <summary>
		/// Disposable for spawning the series
		/// </summary>
		private IDisposable seriesSpawnDisposable = Disposable.Empty;

		public override void SpawnObject() {
			var counter = 0;

			if (!seriesSpawnDisposable.Equals(Disposable.Empty))
				return;

			seriesSpawnDisposable = Observable.Interval(TimeSpan.FromSeconds(1 / SeriesSpawnFrequency)).Subscribe(_ => {
				if (counter >= Scales.Count) {
					seriesSpawnDisposable.Dispose();
					seriesSpawnDisposable = Disposable.Empty;
					return;
				}

				SetObjectScale(Scales[counter]);
				base.SpawnObject();
				counter++;
			});
		}

		public void AddNewScale() {
			Scales.Add(NewScale);
		}

		protected override void OnDisable() {
			base.OnDisable();
			seriesSpawnDisposable.Dispose();
		}

	}

}