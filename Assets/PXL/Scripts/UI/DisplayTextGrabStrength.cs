using Leap.Unity;

namespace PXL.UI {
	
	public class DisplayTextGrabStrength : DisplayTextBase {

		/// <summary>
		/// The observed RigidHand
		/// </summary>
		public RigidHand Hand;

		private void Update() {
			if (Hand.GetLeapHand() != null && Hand.isActiveAndEnabled)
				Text.text = Hand.GetLeapHand().GrabStrength.ToString("0.0000");
			else
				Text.text = "Inactive";
		}

	}

}