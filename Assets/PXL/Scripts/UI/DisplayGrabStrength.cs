using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {
	
	[RequireComponent(typeof(Text))]
	public class DisplayGrabStrength : MonoBehaviour {

		public RigidHand hand;

		private Text text;

		private void Start() {
			text = this.TryGetComponent<Text>();
        }

		private void Update() {
			if (hand.GetLeapHand() != null && hand.isActiveAndEnabled)
				text.text = hand.GetLeapHand().GrabStrength.ToString("0.0000");
			else
				text.text = "Inactive";
		}

	}

}