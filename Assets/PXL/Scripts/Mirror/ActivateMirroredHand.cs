using UnityEngine;
using PXL.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PXL.Mirror {

	[RequireComponent(typeof(ImprovedCapsuleHand))]
	public class ActivateMirroredHand : MonoBehaviour {

		public MirroredHand MirroredHand;

		private void OnEnable() {
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
				return;
#endif

			MirroredHand.AssertNotNull();
			MirroredHand.Setup();
		}

#if UNITY_EDITOR
		private void OnValidate() {
			if (!Utility.EditorUtility.IsPlaying())
				OnEnable();
		}
#endif

	}

}