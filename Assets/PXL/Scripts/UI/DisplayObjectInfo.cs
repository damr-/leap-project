using UniRx;
using PXL.Interaction;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI {

	public class DisplayObjectInfo : InteractionHandSubscriber {

		/// <summary>
		/// Text component which displays the grab state of the object
		/// </summary>
		public Text GrabbedStateText;

		/// <summary>
		/// Text component of a different object which displayes the position of the grabbed object
		/// </summary>
		public Text PositionText;

		protected override void Start() {
			base.Start();

			GrabbedStateText.AssertNotNull("GrabStateText reference is missing");
			PositionText.AssertNotNull("PositionText reference missing");
		}

		protected override void HandleGrabbed(Grabbable grabbable) {
			GrabbedStateText.text = grabbable.gameObject.name + " grabbed";
		}

		protected override void HandleDropped(Grabbable grabbable) {
			GrabbedStateText.text = grabbable.gameObject.name + " dropped";
			PositionText.text = "";
		}

		protected override void HandleMoved(MovementInfo movementInfo) {
			PositionText.text = "x:" + movementInfo.NewPosition.x.ToString("0.000") +
								"\ny: " + movementInfo.NewPosition.y.ToString("0.000") +
								"\nz: " + movementInfo.NewPosition.z.ToString("0.000");
		}
	}

}