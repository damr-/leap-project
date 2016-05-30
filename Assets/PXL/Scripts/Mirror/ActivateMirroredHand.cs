using PXL.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PXL.Mirror {

	/// <summary>
	/// This script activates the given mirrored hand as soon as this object is activated
	/// </summary>
	[RequireComponent(typeof(ImprovedCapsuleHand))]
	public class ActivateMirroredHand : MonoBehaviour {

		/// <summary>
		/// The target mirrored hand to be activated
		/// </summary>
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