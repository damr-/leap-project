using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using PXL.Gamemodes;

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
		
		private void Update() {
			SceneInfos.ForEach(CheckSceneInput);

			if (Input.GetKeyDown(RestartKey)) {
				var scene = SceneManager.GetActiveScene();
				SceneManager.LoadScene(scene.name);
			}

			if (Input.GetKeyDown(QuitKey)) {
				Application.Quit();
			}
		}

		private void CheckSceneInput(SceneInfo sceneInfo) {
			if (Input.GetKeyDown(sceneInfo.LoadKey)) {
				SceneManager.LoadScene(sceneInfo.SceneName);
			}
		}
	}

}