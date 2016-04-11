using PXL.Gamemodes;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Areas {

	[RequireComponent(typeof(StackArea))]
	public class WinOnStackedCorrectly : MonoBehaviour {

		/// <summary>
		/// Whether the game will be won immediately
		/// </summary>
		public bool WinImmediately;

		/// <summary>
		/// The points added to the GameMode when stacked correctly
		/// </summary>
		public int WinPoints = 1;

		/// <summary>
		/// The StackArea of this object
		/// </summary>
		private StackArea StackArea {
			get {
				return mStackArea ?? (mStackArea = this.TryGetComponent<StackArea>());
			}
		}
		private StackArea mStackArea;

		private void Start() {
			StackArea.AreaStatus.Subscribe(status => {
				if (status != StackArea.Status.GameWon)
					return;

				if (WinImmediately) {
					GameMode.SetGameWon(true);
				}
				else {
					GameMode.AddPoints(WinPoints);
				}
			});
		}

	}

}