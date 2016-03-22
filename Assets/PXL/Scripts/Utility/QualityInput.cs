using UnityEngine;

namespace PXL.Utility {

	public class QualityInput : MonoBehaviour {

		private string[] qualityNames;

		private readonly KeyCode[] keyCodes = {
			KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
			KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
		};

		private void Start() {
			qualityNames = QualitySettings.names;
		}

		private void Update() {
			if (!Input.GetKey(KeyCode.LeftControl))
				return;

			for (var i = 0; i < qualityNames.Length; i++) {
				if (Input.GetKeyDown(keyCodes[i]))
					QualitySettings.SetQualityLevel(i, true);
			}
		}

	}

}