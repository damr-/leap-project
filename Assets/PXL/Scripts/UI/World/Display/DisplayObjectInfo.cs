using PXL.Interaction;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI.World.Display {

	public class DisplayObjectInfo : InteractionHandSubscriber {

		/// <summary>
		/// Text component which displays the grab state of the object
		/// </summary>
		public Text GrabbedStateText;

		/// <summary>
		/// Text component of a different object which displayes the position of the grabbed object
		/// </summary>
		public Text PositionText;

		/// <summary>
		/// Text which displayes the state of the Throwable
		/// </summary>
		public Text ThrowStateText;

		/// <summary>
		/// The <see cref="Throwable"/> currently being held by the observed hand
		/// </summary>
		private Throwable throwableObject;

		protected override void Start() {
			base.Start();

			GrabbedStateText.AssertNotNull("GrabStateText reference is missing!");
			PositionText.AssertNotNull("PositionText reference missing!");
			ThrowStateText.AssertNotNull("ThrowStateText reference missing!");
		}

		protected override void HandleGrabbed(Grabbable grabbable) {
			throwableObject = grabbable.GetComponent<Throwable>();
			GrabbedStateText.text = grabbable.gameObject.name + " grabbed";
		}

		protected override void HandleDropped(Grabbable grabbable) {
			PositionText.text = "";
			throwableObject = null;
			GrabbedStateText.text = grabbable.gameObject.name + " dropped";
		}

		protected override void HandleMoved(MovementInfo movementInfo) {
			PositionText.text = "x:" + movementInfo.NewPosition.x.ToString("0.000") +
								"\ny: " + movementInfo.NewPosition.y.ToString("0.000") +
								"\nz: " + movementInfo.NewPosition.z.ToString("0.000");
		}

		protected virtual void Update() {
			if (throwableObject == null) {
				if (ThrowStateText.text != "")
					ThrowStateText.text = "";
				return;
			}
			var dir = throwableObject.GetMotionDirection();
			ThrowStateText.text = "drop delay: " + (throwableObject.Grabbable.EnableDropDelay ? "Yes" : "No") + "\n" +
									dir.x.ToString("0.00") + ", " + dir.y.ToString("0.00") + ", " + dir.z.ToString("0.00") + "\n" +
									dir.magnitude.ToString("0.00") + "\n";
		}
	}

}