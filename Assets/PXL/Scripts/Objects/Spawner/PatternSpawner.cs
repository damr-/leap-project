﻿using PXL.Gamemodes;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// This spawner spawns a pattern of objects, which can be customized in the Editor.
	/// The min/max amount of columns/rows can be changes, as well as the margin between rows and colums
	/// 
	/// Furthermore, the whole pattern can be randomized as well as the amount of rows/columns.
	/// </summary>
	public class PatternSpawner : ObjectSpawner {
	
		[System.Serializable]
		public class PatternColumn {
			public bool[] Rows = new bool[MaxPatternRows];

			public bool this[int i] {
				get { return Rows[i]; }
				set { Rows[i] = value; }
			}
		}

		/// <summary>
		/// The maximum number of columns
		/// </summary>
		public static int MaxPatternColumns = 10;

		/// <summary>
		/// The maximum number of rows
		/// </summary>
		public static int MaxPatternRows = 10;

		/// <summary>
		/// The current number of columns
		/// </summary>
		public int PatternColumns = 5;

		/// <summary>
		/// The current number of rows
		/// </summary>
		public int PatternRows = 5;

		/// <summary>
		/// The margin between rows
		/// </summary>
		public float RowMargin = 0.5f;

		/// <summary>
		/// The margin between columns
		/// </summary>
		public float ColumnMargin = 1f;

		/// <summary>
		/// Whether the fields of the pattern should be randomized
		/// </summary>
		public bool RandomizePattern;

		/// <summary>
		/// Whether the row count should be randomized
		/// </summary>
		public bool RandomizeRowCount;

		/// <summary>
		/// Whether the column count should be randomized
		/// </summary>
		public bool RandomizeColumnCount;

		/// <summary>
		/// Minimum amount of columns possible when choosing the amount randomly
		/// </summary>
		public int MinRandomColumnCount;

		/// <summary>
		/// Maximum amount of columns possible when choosing the amount randomly
		/// </summary>
		public int MaxRandomColumnCount;

		/// <summary>
		/// Minimum amount of rows possible when choosing the amount randomly
		/// </summary>
		public int MinRandomRowCount;

		/// <summary>
		/// Maximum amount of rows possible when choosing the amount randomly
		/// </summary>
		public int MaxRandomRowCount;

		/// <summary>
		/// How many points are added to <see cref="GameState.CurrentPoints"/> when all objects have been removed
		/// </summary>
		public int PatternFinishPoints = 1;

		/// <summary>
		/// The Pattern used by this spawner
		/// </summary>
		public PatternColumn[] SpawnPattern = new PatternColumn[MaxPatternColumns];

		public IObservable<Unit> PatternChanged { get { return patternChangedSubject; } }
		private readonly ISubject<Unit> patternChangedSubject = new Subject<Unit>();

		public override void SpawnObject() {
			if (!CanSpawn())
				return;

			if (RandomizeColumnCount)
				PatternColumns = Random.Range(MinRandomColumnCount, MaxRandomColumnCount + 1);
			if (RandomizeRowCount)
				PatternRows = Random.Range(MinRandomRowCount, MaxRandomRowCount + 1);
			if (RandomizePattern)
				CreateRandomPattern();

			for (var row = 0; row < PatternRows; row++) {
				for (var column = 0; column < PatternColumns; column++) {
					if (!SpawnPattern[column][row])
						continue;

					var spawnOffset = GetPositionOffset(row, column);
					SpawnObject(spawnOffset);
				}
			}
			if (SpawnedObjects.Count == 0)
				SpawnObject();
		}

		protected override void HandleObjectDespawned(InteractiveObject interactiveObject) {
			base.HandleObjectDespawned(interactiveObject);

			if (SpawnedObjects.Count == 0)
				GameState.AddPoints(PatternFinishPoints);
		}

		/// <summary>
		/// Returns the offset for an object which should be placed at (row,column)
		/// </summary>
		public Vector3 GetPositionOffset(int row, int col) {
			return new Vector3((col) * ColumnMargin, 0, -(row) * RowMargin);
		}

		/// <summary>
		/// Updates the amount of columns in the pattern and sets all fields out of bounds to false
		/// </summary>
		public void SetColumns(int newAmount) {
			for (var row = 0; row < MaxPatternRows; row++) {
				for (var column = newAmount; column < MaxPatternColumns; column++) {
					SpawnPattern[column][row] = false;
				}
			}
			PatternColumns = newAmount;
			patternChangedSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Updates the amount of rows in the pattern and sets all fields out of bounds to false
		/// </summary>
		public void SetRows(int newAmount) {
			for (var row = newAmount; row < MaxPatternRows; row++) {
				for (var column = 0; column < MaxPatternColumns; column++) {
					SpawnPattern[column][row] = false;
				}
			}
			PatternRows = newAmount;
			patternChangedSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Updates the column margin and calles the subject
		/// </summary>
		/// <param name="newMargin"></param>
		public void SetColumnMargin(float newMargin) {
			ColumnMargin = newMargin;
			patternChangedSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Updates the row margin and calles the subject
		/// </summary>
		/// <param name="newMargin"></param>
		public void SetRowMargin(float newMargin) {
			RowMargin = newMargin;
			patternChangedSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// Sets all fields unchecked
		/// </summary>
		public void ClearPattern() {
			SetPatternChecked(false);
		}

		/// <summary>
		/// Inverts all fields of the currently visible pattern
		/// </summary>
		public void InvertPattern() {
			for (var row = 0; row < PatternRows; row++) {
				for (var column = 0; column < PatternColumns; column++) {
					SpawnPattern[column][row] = !SpawnPattern[column][row];
				}
			}
		}

		/// <summary>
		/// Sets a random checked state on all visible fields
		/// </summary>
		public void CreateRandomPattern() {
			for (var row = 0; row < PatternRows; row++) {
				for (var column = 0; column < PatternColumns; column++) {
					SpawnPattern[column][row] = Random.Range(0, 2) == 0;
				}
			}
		}

		/// <summary>
		/// Sets the given checked status on all visible fields
		/// </summary>
		private void SetPatternChecked(bool isChecked) {
			for (var row = 0; row < PatternRows; row++) {
				for (var column = 0; column < PatternColumns; column++) {
					SpawnPattern[column][row] = isChecked;
				}
			}
		}
	}
}