using UnityEngine;
using PXL.Utility;

namespace PXL.Mirror {

	[RequireComponent(typeof(ImprovedCapsuleHand))]
	public class ActivateMirroredHand : MonoBehaviour {

		public MirroredHand MirroredHand;

		private void OnEnable() {
			MirroredHand.AssertNotNull();
			MirroredHand.Setup();
		}

	}

}