using System;
using PXL.UI.World;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Areas {

	[RequireComponent(typeof(DestroyArea))]
	public class ResetInfoDisplayOnObjectDestroyed : MonoBehaviour {

		/// <summary>
		/// The target DisplayInformation
		/// </summary>
		public DisplayInformation DisplayInformation;

		/// <summary>
		/// The DestroyArea component of this object
		/// </summary>
		private DestroyArea DestroyArea {
			get { return mDestroyArea ?? (mDestroyArea = this.TryGetComponent<DestroyArea>()); }
		}
		private DestroyArea mDestroyArea;

		private IDisposable goalReachedSubscription = Disposable.Empty;

		private void Start() {
			DisplayInformation.AssertNotNull("Missing DisplayInformation!");
			goalReachedSubscription = DestroyArea.ObjectDestroyed.Subscribe(_  => HandleObjectDestroyed());
		}

		private void HandleObjectDestroyed() {
			DisplayInformation.ResetInformation();
		}

		private void OnDisable() {
			goalReachedSubscription.Dispose();
        }
	}

}