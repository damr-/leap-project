using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(PatternSpawner))]
	[ExecuteInEditMode]
	public class PatternSpawnerVisualizer : MonoBehaviour {

		/// <summary>
		/// The <see cref="GameObject"/> used for previewing the pattern
		/// </summary>
		public GameObject PreviewGameObject;

		/// <summary>
		/// The <see cref="GameObject"/> used for showing a spot in the preview which will have an object assigned randomly.
		/// </summary>
		public GameObject RandomPreviewGameObject;

		/// <summary>
		/// The <see cref="GameObject"/> used for showing a spot in the preview which might be added due to the random column and row count
		/// </summary>
		public GameObject PossiblyRandomPreviewGameObject;

		/// <summary>
		/// The parent for the spawned preview objects
		/// </summary>
		public Transform PreviewContainer;

		/// <summary>
		/// The <see cref="PatternSpawner"/> component of this object
		/// </summary>
		private PatternSpawner PatternSpawner {
			get {
				return mPatternSpawner ?? (mPatternSpawner = this.TryGetComponent<PatternSpawner>());
			}
		}
		private PatternSpawner mPatternSpawner;

		/// <summary>
		/// The spawned preview objects
		/// </summary>
		public List<Transform> PreviewObjects = new List<Transform>();

		/// <summary>
		/// The spawned random preview objects
		/// </summary>
		public List<Transform> RandomPreviewObjects = new List<Transform>();

		/// <summary>
		/// The spawned possibly random preview objects
		/// </summary>
		public List<Transform> PossiblyRandomPreviewObjects = new List<Transform>();

		/// <summary>
		/// Subscription for manually updating the preview
		/// </summary>
		private IDisposable timeSubscription = Disposable.Empty;

		/// <summary>
		/// Add the state change callback and start updating the previews if not playing
		/// </summary>
		private void OnEnable() {
			EditorApplication.playmodeStateChanged += StateChange;
			RemoveAllPreviewObjects();

			if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) {
				StartUpdating();
			}
		}

		/// <summary>
		/// Handles the change of application states.
		/// </summary>
		private void StateChange() {
			timeSubscription.Dispose();
			if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying) {
				RemoveAllPreviewObjects();
				return;
			}
			StartUpdating();
		}

		/// <summary>
		/// Loads the list of preview objects, if possible, and starts the manual update timer.
		/// </summary>
		private void StartUpdating() {
			timeSubscription = Observable.Interval(TimeSpan.FromSeconds(0.01f)).Subscribe(_ => UpdatePreview());
		}

		/// <summary>
		/// Saves the current list of preview objects, disposes the timer and removes the callback handle.
		/// </summary>
		private void OnDisable() {
			timeSubscription.Dispose();
			EditorApplication.playmodeStateChanged -= StateChange;
		}

		private void RemoveAllPreviewObjects() {
			RemovePreviewObjects();
			RemoveRandomPreviewObjects();
			foreach (Transform o in PreviewContainer) {
				DestroyImmediate(o.gameObject);
			}
		}

		private void RemovePreviewObjects() {
			while (PreviewObjects.Count > 0) {
				RemoveObject(PreviewObjects, 0);
			}
		}

		private void RemoveRandomPreviewObjects() {
			while (RandomPreviewObjects.Count > 0) {
				RemoveObject(RandomPreviewObjects, 0);
			}
			while (PossiblyRandomPreviewObjects.Count > 0) {
				RemoveObject(PossiblyRandomPreviewObjects, 0);
			}
		}

		/// <summary>
		/// Updates the Editor preview by makign sure that there's the correct amount of preview objects
		/// and that every object is at the correct position
		/// </summary>
		private void UpdatePreview() {

			if (PatternSpawner.RandomizePattern) {
				RemovePreviewObjects();

				var randomRowCount = PatternSpawner.RandomizeRowCount ? PatternSpawner.MinRandomRowCount : PatternSpawner.PatternRows;
				var randomColCount = PatternSpawner.RandomizeColumnCount ? PatternSpawner.MinRandomColumnCount : PatternSpawner.PatternColumns;
				var randomCount = randomRowCount * randomColCount;
				UpdatePreviewObjectsAmount(randomCount, RandomPreviewGameObject, RandomPreviewObjects);

				var maxRowCount = PatternSpawner.RandomizeRowCount ? PatternSpawner.MaxRandomRowCount : PatternSpawner.PatternRows;
				var maxColumnCount = PatternSpawner.RandomizeColumnCount ? PatternSpawner.MaxRandomColumnCount : PatternSpawner.PatternColumns;
				var possiblyRandomCount = maxRowCount * maxColumnCount - randomCount;
				UpdatePreviewObjectsAmount(possiblyRandomCount, PossiblyRandomPreviewGameObject, PossiblyRandomPreviewObjects);

				var rowCount = PatternSpawner.RandomizeRowCount ? PatternSpawner.MaxRandomRowCount : PatternSpawner.PatternRows;
				var columnCount = PatternSpawner.RandomizeColumnCount ? PatternSpawner.MaxRandomColumnCount : PatternSpawner.PatternColumns;

				var randomCounter = 0;
				var possiblyRandomCounter = 0;

				for (var row = 0; row < rowCount; row++) {
					for (var column = 0; column < columnCount; column++) {
						var possiblyRandomField = (PatternSpawner.RandomizeColumnCount && column >= PatternSpawner.MinRandomColumnCount) || (PatternSpawner.RandomizeRowCount && row >= PatternSpawner.MinRandomRowCount);
						var spawnOffset = PatternSpawner.GetPositionOffset(row, column);
						var previewTransform = possiblyRandomField ? PossiblyRandomPreviewObjects[possiblyRandomCounter++] : RandomPreviewObjects[randomCounter++];
						previewTransform.position = transform.position + spawnOffset;
					}
				}


			}
			else {
				RemoveRandomPreviewObjects();

				var activeFieldsCount = PatternSpawner.SpawnPattern.Sum(column => column.Rows.Count(row => row));
				UpdatePreviewObjectsAmount(activeFieldsCount, PreviewGameObject, PreviewObjects);
				var counter = 0;
				for (var row = 0; row < PatternSpawner.PatternRows; row++) {
					for (var column = 0; column < PatternSpawner.PatternColumns; column++) {
						if (!PatternSpawner.SpawnPattern[column][row])
							continue;

						var spawnOffset = PatternSpawner.GetPositionOffset(row, column);
						var previewObject = PreviewObjects[counter];
						previewObject.transform.position = transform.position + spawnOffset;

						counter++;
					}
				}
			}

		}

		/// <summary>
		/// Updates the size of the given list of objects of type <see cref="prefab"/> to the desired amount.
		/// </summary>
		private void UpdatePreviewObjectsAmount(int desiredAmount, GameObject prefab, List<Transform> existingPreviewObjects) {
			while (existingPreviewObjects.Count < desiredAmount) {
				var newObject = (GameObject)Instantiate(prefab, transform.position, Quaternion.identity);
				newObject.transform.SetParent(PreviewContainer, true);

				var name = newObject.gameObject.name;
				var index = name.IndexOf("(");
				if (index != -1)
					newObject.name = name.Remove(index);

				existingPreviewObjects.Add(newObject.transform);
			}
			while (existingPreviewObjects.Count > desiredAmount) {
				RemoveObject(existingPreviewObjects, 0);
			}
		}

		/// <summary>
		/// Removes the element with the given index from the given list
		/// </summary>
		private void RemoveObject(List<Transform> objects, int index) {
			if (index >= objects.Count)
				return;

			var o = objects[index];
			objects.RemoveAt(index);
			DestroyImmediate(o.gameObject);
		}

	}

}