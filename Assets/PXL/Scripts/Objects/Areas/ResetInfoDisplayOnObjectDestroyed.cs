using System;
using PXL.UI.World.Display;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Areas {

	/// <summary>
	/// This script resets the information display as soon as an object has been destroyed.
	/// </summary>
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

		/// <summary>
		/// The disposable for the goal reached subscrpition
		/// </summary>
		private IDisposable goalReachedDisposable = Disposable.Empty;

		private void Start() {
			DisplayInformation.AssertNotNull("Missing DisplayInformation!");
			goalReachedDisposable = DestroyArea.ObjectDestroyed.Subscribe(_  => HandleObjectDestroyed());
		}

		private void HandleObjectDestroyed() {
			DisplayInformation.ResetInformation();
		}

		private void OnDisable() {
			goalReachedDisposable.Dispose();
        }
	}

}