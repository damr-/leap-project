using Leap.Unity;
using UnityEngine;

namespace PXL.Mirror {

	public class ToggleMirroredSide : MonoBehaviour {

		public KeyCode SideSwapKey = KeyCode.M;

		private HandPool handPool;

		public RigidHand LeftHand;

		public RigidHand RightHand;

		private static readonly string[] Groups = {
			"L_Graphics_Hands",
			"R_Graphics_Hands"
		};

		private int currentGroupIndex;

		private Collider[] leftHandColliders;

		private Collider[] rightHandColliders;

		private void Start() {
			handPool = FindObjectOfType<HandPool>();

			leftHandColliders = LeftHand.GetComponentsInChildren<Collider>();
			rightHandColliders = RightHand.GetComponentsInChildren<Collider>();
		}

		private void Update() {
			if (!Input.GetKeyDown(SideSwapKey))
				return;

			handPool.DisableGroup(Groups[currentGroupIndex]);
			currentGroupIndex = ++currentGroupIndex % Groups.Length;
			handPool.EnableGroup(Groups[currentGroupIndex]);

			foreach (var c in leftHandColliders)
				c.enabled = currentGroupIndex == 0;
			foreach (var c in rightHandColliders)
				c.enabled = currentGroupIndex == 1;

		}

	}

}