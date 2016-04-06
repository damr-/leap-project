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
		public List<SceneInfo> SceneInfos = new List<SceneInfo> {
			new SceneInfo("dev", KeyCode.F1),
			new SceneInfo("hanoi", KeyCode.F2)
		};

		private void Update() {
			if (isLoading)
				return;

			foreach (var item in SceneInfos.Select((value, i) => new { i, value })) {
				if (!Input.GetKeyDown(item.value.LoadKey))
					continue;

				StartCoroutine(CallLoad(item.i));
			}

			if (Input.GetKeyDown(RestartKey)) {
				var currentLevel = SceneManager.GetActiveScene().buildIndex;
				StartCoroutine(CallLoad(currentLevel));
			}

			if (Input.GetKeyDown(QuitKey)) {
				Application.Quit();
			}
		}

		/// <summary>
		/// Starts to asynchronously load the level with the given index
		/// </summary>
		private IEnumerator CallLoad(int loadLevel) {
			isLoading = true;
			var async = SceneManager.LoadSceneAsync(loadLevel);
			yield return async;
		}

		[Serializable]
		public class SceneInfo {
			public KeyCode LoadKey;
			public string SceneName;

			public SceneInfo(string sceneName, KeyCode loadKey) {
				SceneName = sceneName;
				LoadKey = loadKey;
			}
		}

	}

}