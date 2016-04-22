using Leap.Unity;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

	public class DisplayTextGrabStrength : MonoBehaviour {

		/// <summary>
		/// The observed RigidHand
		/// </summary>
		public RigidHand Hand;
		
		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		private void Update() {
			if (Hand.GetLeapHand() != null && Hand.isActiveAndEnabled)
				Text.text = Hand.GetLeapHand().GrabStrength.ToString("0.0000");
			else
				Text.text = "Inactive";
		}

	}

}