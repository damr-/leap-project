using System.Linq;
using PXL.Utility;
using UnityEngine;

namespace  PXL.UI.World {

	public class SetupLevelChangePanel : MonoBehaviour {

		public GameObject LevelButtonPrefab;

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
