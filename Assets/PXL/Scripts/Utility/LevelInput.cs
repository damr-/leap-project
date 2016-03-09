using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

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
		/// Within how many seconds the second tap has to follow
		/// </summary>
		public float MaxDoubleTapDelay = 1f;

		/// <summary>
		/// All the names of the scenes and the corresponding keys to load them
		/// </summary>
		public List<SceneInfo> SceneInfos = new List<SceneInfo> {
			new SceneInfo("dev", KeyCode.F1),
			new SceneInfo("hanoi", KeyCode.F2)
		};

		/// <summary>
		/// Last time the escape key was pressed
		/// </summary>
		private float lastTime;

		/// <summary>
		/// Whether the escape key has already been pressed once
		/// </summary>
		private bool pressedOnce;

		private void Update() {
			SceneInfos.ForEach(CheckSceneInput);

			if (Input.GetKeyDown(RestartKey)) {
				var scene = SceneManager.GetActiveScene();
				SceneManager.LoadScene(scene.name);
			}

			if (Input.GetKeyDown(QuitKey)) {
				if (!pressedOnce) {
					lastTime = Time.time;
					pressedOnce = true;
				}
				else {
					Application.Quit();
				}
			}

			if (pressedOnce && Time.time - lastTime > MaxDoubleTapDelay)
				pressedOnce = false;
		}

		private void CheckSceneInput(SceneInfo sceneInfo) {
			if (Input.GetKeyDown(sceneInfo.LoadKey)) {
				SceneManager.LoadScene(sceneInfo.SceneName);
			}
		}
	}

}