using UnityEngine;

namespace PXL.Objects.Areas {

	public class ImmediateDestroyArea : DestroyArea {
		
		protected override void HandleValidObjectType(ObjectBehaviour objectBehaviour) {
			CurrentDestroyAmount.Value++;
            if (CurrentDestroyAmount.Value == WinDestroyAmount) {
				HandleGameWon();
			}
			objectBehaviour.DestroyObject();
		}

		protected override void HandleInvalidObjectType(ObjectBehaviour objectBehaviour) {
			if (DestroyWrongTypes) {
				objectBehaviour.DestroyObject();
			}
			if (PunishWrongTypes) {
				CurrentDestroyAmount.Value = Mathf.Clamp(CurrentDestroyAmount.Value - PunishAmount, 0, CurrentDestroyAmount.Value);
			}
		}

	}

}
