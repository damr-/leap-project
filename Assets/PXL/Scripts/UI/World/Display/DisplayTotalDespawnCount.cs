using UniRx;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

	/// <summary>
	/// This script displays the TotalDespawnCount of the referenced spawner compared to it's TotalSpawnLimit.
	/// 
	/// The shown data is updated whenever the TotalDespawnCount changes.
	/// </summary>
	public class DisplayTotalDespawnCount : MonoBehaviour {

		/// <summary>
		/// The referenced spawner
		/// </summary>
		public ObjectSpawner Spawner;

		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		protected virtual void Start() {
			Spawner.AssertNotNull("Spawner reference missing");

			Spawner.TotalDespawnCount.Subscribe(UpdateText);
			UpdateText(0);
		}

		/// <summary>
		/// Called when the total amount of objects spawned by the spawner changes
		/// </summary>
		private void UpdateText(int despawnCount) {
			Text.text = Spawner.TotalSpawnLimit - despawnCount + "/" + Spawner.TotalSpawnLimit;
		}

	}

}