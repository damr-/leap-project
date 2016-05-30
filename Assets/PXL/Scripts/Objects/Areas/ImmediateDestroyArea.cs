using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	/// <summary>
	/// This script describes <see cref="DestroyArea"/> which immediately destroyes an object 
	/// which enters the area.
	/// </summary>
	public class ImmediateDestroyArea : DestroyArea {
		
		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
			base.HandleValidObjectType(interactiveObject);

			CurrentDestroyAmount.Value++;
			if (CurrentDestroyAmount.Value == WinDestroyAmount)
				HandleGameWon();
			ObjectDestroyedSubject.OnNext(interactiveObject);
			interactiveObject.Kill();
        }

		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {
			base.HandleInvalidObjectType(interactiveObject);

			if (DestroyWrongTypes) {
				ObjectDestroyedSubject.OnNext(interactiveObject);
				interactiveObject.Kill();
			}
			if (PunishWrongTypes)
				CurrentDestroyAmount.Value = Mathf.Clamp(CurrentDestroyAmount.Value - PunishAmount, 0, CurrentDestroyAmount.Value);
		}

	}

}
