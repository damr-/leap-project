using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PXL.Utility {

	public class LevelInput : MonoBehaviour {

		/// <summary>
		/// Whether a level is currently loading
		/// </summary>
		private bool isLoading;

		/// <summary>
		/// The key used for quitting the application
		/// </summary>
		public KeyCode QuitKey = KeyCode.Escape;

		/// <summary>
		/// The key used for restarting the level
		/// </summary>
		public KeyCode RestartKey = KeyCode.Space;

		/// <summary>
		/// All the names of the scenes and the corresponding keys to load them
		/// </summary>
		public List<SceneInfo> SceneInfos = new List<SceneInfo>();

		private void Update() {
			if (isLoading)
				return;

			foreach (var item in SceneInfos.Select((value, i) => new { i, value })) {
				if (!Input.GetKeyDown(item.value.LoadKey))
					continue;

				StartLoadLevel(item.i);
			}

			if (Input.GetKeyDown(RestartKey)) {
				StartReloadLevel();
			}

			if (Input.GetKeyDown(QuitKey)) {
				Application.Quit();
			}
		}

		/// <summary>
		/// Starts to load the level with the given index asynchronously
		/// </summary>
		public void StartLoadLevel(int levelIndex) {
			if (isLoading)
				return;

			StartCoroutine(CallLoad(levelIndex));
		}

		/// <summary>
		/// Starts to reload the current level asynchronously
		/// </summary>
		public void StartReloadLevel() {
			var currentLevel = SceneManager.GetActiveScene().buildIndex;
			StartLoadLevel(currentLevel);
		}

		/// <summary>
		/// Starts to asynchronously load the level with the given index
		/// </summary>
		private IEnumerator CallLoad(int levelIndex) {
			isLoading = true;
			SceneInfo.Counter = 0;
			var async = SceneManager.LoadSceneAsync(levelIndex);
			yield return async;
		}

		[Serializable]
		public class SceneInfo {
			public static int Counter;

			public KeyCode LoadKey;
			public string SceneName;
			public string PrettyName;

			public int Index {
				get {
					if (mIndex == 0)
						mIndex = Counter++;
					return mIndex;
				}
			}

			private int mIndex;
		}

	}

}