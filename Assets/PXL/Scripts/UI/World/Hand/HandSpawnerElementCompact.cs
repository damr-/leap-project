using PXL.Objects.Spawner;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Hand {

	public class HandSpawnerElementCompact : MonoBehaviour {
	
		public Text SpawnerNameText;

		public Text ScaleText;

		protected ObjectSpawner ObjectSpawner;

		public void SetSpawner(ObjectSpawner objectSpawner) {
			ObjectSpawner = objectSpawner;
			SpawnerNameText.text = ObjectSpawner.gameObject.name;

			ObjectSpawner.CurrentScaleFactor.Subscribe(UpdateScaleDisplay);
			UpdateScaleDisplay(ObjectSpawner.CurrentScaleFactor.Value);
		}

		public void UpdateScaleDisplay(float newScale) {
			ScaleText.text = newScale.ToString("0.00");
		}

	}

}