using System.Linq;
using PXL.UI.World.Buttons;
using PXL.Utility;
using UnityEngine;

namespace PXL.UI.World {

	/// <summary>
	/// A script which sets up the level change panel by spawning a button for each available level and setting up the references.
	/// </summary>
	public class SetupLevelChangePanel : MonoBehaviour {

		/// <summary>
		/// The prefab used for the button
		/// </summary>
		public GameObject LevelButtonPrefab;

		/// <summary>
		/// Refernce to the <see cref="LevelInput"/> in this scene
		/// </summary>
		private LevelInput levelInput;

		private void Start() {
			LevelButtonPrefab.AssertNotNull("Missing Level Button Prefab!");
			levelInput = FindObjectOfType<LevelInput>();

			foreach (var item in levelInput.SceneInfos.Select((value, i) => new { i, value })) {
				var buttonTransform = ((GameObject)Instantiate(LevelButtonPrefab, Vector3.zero, Quaternion.identity)).transform;
				buttonTransform.SetParent(transform, false);

				var levelLoadButton = buttonTransform.TryGetComponent<LevelLoadButton>();
				levelLoadButton.LevelIndex = item.value.Index;
				levelLoadButton.ButtonText.text = item.value.PrettyName;
				levelLoadButton.Button.onClick.AddListener(() => levelInput.StartLoadLevel(levelLoadButton.LevelIndex));
			}

		}

	}

}