using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;

namespace PXL.Utility {

	public class LevelInput : MonoBehaviour {

		[Serializable]
		public class SceneInfo {
			public string SceneName;
			public KeyCode LoadKey;

			public SceneInfo(string sceneName, KeyCode loadKey) {
				SceneName = sceneName;
				LoadKey = loadKey;
			}
		}

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

		private static int _currentLevel;

		private bool isLoading;

		private void Update() {
			if (SceneManager.GetActiveScene().buildIndex != _currentLevel) {
				isLoading = false;
				_currentLevel = SceneManager.GetActiveScene().buildIndex;
			}

			if (isLoading)
				return;

			foreach (var item in SceneInfos.Select((value, i) => new { i, value })) {
				if (!Input.GetKeyDown(item.value.LoadKey)) 
					continue;

				_currentLevel = SceneManager.GetActiveScene().buildIndex;
				isLoading = true;
				StartCoroutine(CallLoad(item.i));
			}

			if (Input.GetKeyDown(RestartKey)) {
				var scene = SceneManager.GetActiveScene();
				SceneManager.LoadScene(scene.name);
			}

			if (Input.GetKeyDown(QuitKey)) {
				Application.Quit();
			}
		}

		private static IEnumerator CallLoad(int loadLevel) {
			var async = SceneManager.LoadSceneAsync(loadLevel);
			yield return async;
		}

	}

}