using UniRx;
using PXL.Objects.Spawner;
using PXL.Utility;

namespace PXL.UI.World.Display {

	public class DisplayTotalSpawnCount : DisplayTextBase {

		/// <summary>
		/// The referenced spawner
		/// </summary>
		public ObjectSpawner Spawner;

		protected override void Start() {
			base.Start();

			Spawner.AssertNotNull("Spawner reference missing");

			Spawner.TotalDespawnCount.Subscribe(UpdateText);
			UpdateText(0);
		}

		/// <summary>
		/// Called when the total amount of objects spawned by the spawner changes
		/// </summary>
		private void UpdateText(int despawnCount) {
			Text.text = (Spawner.TotalSpawnLimit - despawnCount) + "/" + Spawner.TotalSpawnLimit;
		}

	}

}
