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
	public class PatternVisualizer : MonoBehaviour {

		/// <summary>
		/// The <see cref="GameObject"/> used for previewing the pattern
		/// </summary>
		public GameObject PreviewGameObject;

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
		/// All spawned preview Objects
		/// </summary>
		public List<Transform> PreviewObjects = new List<Transform>();

		/// <summary>
		/// Static Dictionary with all PatternVisualizers and their Lists of preview objects to save them across game states
		/// </summary>
		public static IDictionary<PatternVisualizer, List<Transform>> VisualizerPreviews = new Dictionary<PatternVisualizer, List<Transform>>(); 

		private IDisposable timeSubscription = Disposable.Empty;

		private void OnEnable() {
			if (Application.isEditor && Application.isPlaying) {
				PreviewContainer.gameObject.SetActive(false);
				return;
			}

			if (!VisualizerPreviews.ContainsKey(this)) {
				VisualizerPreviews.Add(this, PreviewObjects);
			}
			else {
				PreviewObjects = VisualizerPreviews[this];
			}

			PreviewContainer.gameObject.SetActive(true);
			timeSubscription.Dispose();
			timeSubscription = Observable.Interval(TimeSpan.FromSeconds(0.01f)).Subscribe(_ => UpdatePreview());
		}

		private void OnDisable() {
			VisualizerPreviews[this] = PreviewObjects;
			timeSubscription.Dispose();
		}

		/// <summary>
		/// Updates the Editor preview by makign sure that there's the correct amount of preview objects
		/// and that every object is at the correct position
		/// </summary>
		private void UpdatePreview() {
			var activeFieldsCount = PatternSpawner.SpawnPattern.Sum(column => column.Rows.Count(row => row));

			while (PreviewObjects.Count < activeFieldsCount) {
				var previewObject = (GameObject)Instantiate(PreviewGameObject, transform.position, Quaternion.identity);
				previewObject.transform.SetParent(PreviewContainer, true);
				PreviewObjects.Add(previewObject.transform);
			}
			while (PreviewObjects.Count > activeFieldsCount) {
				RemovePreviewObject(0);
			}

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

		public void Refresh() {
			while (PreviewObjects.Count > 0) {
				RemovePreviewObject(0);
			}
			foreach (Transform o in PreviewContainer) {
				DestroyImmediate(o.gameObject);
			}
			UpdatePreview();
			VisualizerPreviews[this] = PreviewObjects;
		}

		/// <summary>
		/// Removes the preview object from the scene with the given list's index
		/// </summary>
		private void RemovePreviewObject(int index) {
			var o = PreviewObjects[index];
			PreviewObjects.RemoveAt(index);
			DestroyImmediate(o.gameObject);
		}

	}

}