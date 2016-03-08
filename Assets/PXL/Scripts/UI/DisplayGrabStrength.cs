namespace PXL.UI {
	
	public class DisplayGrabStrength : DisplayBase {

		/// <summary>
		/// The observed RigidHand
		/// </summary>
		public RigidHand hand;

		private void Update() {
			if (hand.GetLeapHand() != null && hand.isActiveAndEnabled)
				text.text = hand.GetLeapHand().GrabStrength.ToString("0.0000");
			else
				text.text = "Inactive";
		}

	}

}