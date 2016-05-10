using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(PatternSpawner))]
	[ExecuteInEditMode]
	public class PatternSpawnerVisualizer : MonoBehaviour {
	
		/// <summary>
		/// The <see cref="GameObject" /> used for showing a spot in the preview which might be added due to the random column and
		/// row count
		/// </summary>
		public GameObject PossiblyRandomPreviewGameObject;

		/// <summary>
		/// The spawned possibly random preview objects
		/// </summary>
		public List<Transform> PossiblyRandomPreviewObjects = new List<Transform>();

		/// <summary>
		/// The <see cref="GameObject" /> used for previewing the pattern
		/// </summary>
		public GameObject PreviewGameObject;

		/// <summary>
		/// The spawned preview objects
		/// </summary>
		public List<Transform> PreviewObjects = new List<Transform>();

		/// <summary>
		/// The <see cref="GameObject" /> used for showing a spot in the preview which will have an object assigned randomly.
		/// </summary>
		public GameObject RandomPreviewGameObject;

		/// <summary>
		/// The spawned random preview objects
		/// </summary>
		public List<Transform> RandomPreviewObjects = new List<Transform>();

		/// <summary>
		/// The <see cref="PatternSpawner" /> component of this object
		/// </summary>
		private PatternSpawner PatternSpawner {
			get { return mPatternSpawner ?? (mPatternSpawner = this.TryGetComponent<PatternSpawner>()); }
		}
		private PatternSpawner mPatternSpawner;

#if UNITY_EDITOR

		/// <summary>
		/// Disposable for manually updating the preview
		/// </summary>
		private IDisposable timeDisposable = Disposable.Empty;

		/// <summary>
		/// The instance ID of this visualizer to allow duplication
		/// </summary>
		private int id = -1;

		/// <summary>
		/// Add the state change callback and start updating the previews if not playing
		/// </summary>
		private void OnEnable() {
			if (id != GetInstanceID()) {
				PreviewObjects = new List<Transform>();
				RandomPreviewObjects = new List<Transform>();
				PossiblyRandomPreviewObjects = new List<Transform>();

				id = GetInstanceID();
			}

			EditorApplication.playmodeStateChanged += StateChange;
			RemoveAllPreviewObjects();

			if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
				StartUpdating();
		}

		/// <summary>
		/// Handles the change of application states.
		/// </summary>
		private void StateChange() {
			timeDisposable.Dispose();
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
			timeDisposable = Observable.Interval(TimeSpan.FromSeconds(0.01f)).Subscribe(_ => UpdatePreview());
		}

		/// <summary>
		/// Saves the current list of preview objects, disposes the timer and removes the callback handle.
		/// </summary>
		private void OnDisable() {
			timeDisposable.Dispose();
			EditorApplication.playmodeStateChanged -= StateChange;
		}

		/// <summary>
		/// Removes all preview objects and other Transforms inside PreviewContainer
		/// </summary>
		private void RemoveAllPreviewObjects() {
			RemovePreviewObjects();
			RemoveRandomPreviewObjects();
			transform.RemoveAllChildTransforms();
		}

		/// <summary>
		/// Removes all objects from <see cref="PreviewObjects" />
		/// </summary>
		private void RemovePreviewObjects() {
			Extensions.PurgeIfNecessary(ref PreviewObjects);
			while (PreviewObjects.Count > 0)
				PreviewObjects.DestroyElement(0);
		}

		/// <summary>
		/// Removes all objects from <see cref="RandomPreviewObjects" /> and <see cref="PossiblyRandomPreviewObjects" />
		/// </summary>
		private void RemoveRandomPreviewObjects() {
			Extensions.PurgeIfNecessary(ref RandomPreviewObjects);
			while (RandomPreviewObjects.Count > 0)
				RandomPreviewObjects.DestroyElement(0);

			Extensions.PurgeIfNecessary(ref PossiblyRandomPreviewObjects);
			while (PossiblyRandomPreviewObjects.Count > 0)
				PossiblyRandomPreviewObjects.DestroyElement(0);
		}

		/// <summary>
		/// Updates the Editor preview by making sure that there's the correct amount of preview objects
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

				var randomCounter = 0;
				var possiblyRandomCounter = 0;

				for (var row = 0; row < maxRowCount; row++) {
					for (var column = 0; column < maxColumnCount; column++) {
						var possiblyRandomField = (PatternSpawner.RandomizeColumnCount && column >= PatternSpawner.MinRandomColumnCount) ||
												  (PatternSpawner.RandomizeRowCount && row >= PatternSpawner.MinRandomRowCount);
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
		/// Updates the size of the given list of objects of type <see cref="prefab" /> to the desired amount.
		/// </summary>
		private void UpdatePreviewObjectsAmount(int desiredAmount, GameObject prefab, List<Transform> existingPreviewObjects) {
			while (existingPreviewObjects.Count < desiredAmount) {
				var newObject = (GameObject)Instantiate(prefab, transform.position, Quaternion.identity);
				newObject.transform.SetParent(transform, true);

				var newObjectName = newObject.gameObject.name;
				var index = newObjectName.IndexOf("(");
				if (index != -1)
					newObject.name = newObjectName.Remove(index);

				existingPreviewObjects.Add(newObject.transform);
			}
			while (existingPreviewObjects.Count > desiredAmount)
				existingPreviewObjects.DestroyElement(0);
		}

		/// <summary>
		/// Removes all preview objects so that they will be re-created
		/// </summary>
		public void Refresh() {
			RemoveAllPreviewObjects();
		}
#endif

	}

}