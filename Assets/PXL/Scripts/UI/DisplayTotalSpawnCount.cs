using UniRx;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class DisplayTotalSpawnCount : MonoBehaviour {

		/// <summary>
		/// The Text component which will display the info
		/// </summary>
		private Text DisplayText {
			get { return mDisplayText ?? (mDisplayText = this.TryGetComponent<Text>()); }
		}
		private Text mDisplayText;

		/// <summary>
		/// The referenced spawner
		/// </summary>
		public ObjectSpawner Spawner;

		private void Start() {
			Spawner.AssertNotNull("Spawner reference missing");
			
			Spawner.TotalDespawnCount.Subscribe(UpdateText);
			UpdateText(0);
		}

		/// <summary>
		/// Called when the total amount of objects spawned by the spawner changes
		/// </summary>
		private void UpdateText(int despawnCount) {
			DisplayText.text = (Spawner.TotalSpawnLimit - despawnCount) + "/" + Spawner.TotalSpawnLimit;
		}

	}

}
