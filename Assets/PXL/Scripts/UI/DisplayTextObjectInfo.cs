using UniRx;
using PXL.Interaction;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI {

	public class DisplayTextObjectInfo : DisplayTextBase {

		/// <summary>
		/// All InteractionHands in this scene
		/// </summary>
		public HandModel HandModel;

		/// <summary>
		/// The InteractionHand components of the HandModels
		/// </summary>
		protected InteractionHand InteractionHand;

		/// <summary>
		/// Text component of a different object which displayes the position of the grabbed object
		/// </summary>
		public Text PositionText;

		protected override void Start() {
			base.Start();

			InteractionHand = HandModel.GetComponent<InteractionHand>();

			InteractionHand.ObjectGrabbed.Subscribe(grabbable => ObjectGrabStateChange(grabbable, true));
			InteractionHand.ObjectDropped.Subscribe(grabbable => ObjectGrabStateChange(grabbable, false));
			InteractionHand.ObjectMoved.Subscribe(ObjectMoved);

			PositionText.AssertNotNull();
		}

		/// <summary>
		/// Called when an object was grabbed. Displayes this info if it's the correct hand
		/// </summary>
		/// <param name="grabbable">The object</param>
		private void ObjectGrabStateChange(Grabbable grabbable, bool grabbed) {
			if (grabbed) {
				Text.text = grabbable.gameObject.name + " grabbed";
			}
			else {
				Text.text = grabbable.gameObject.name + " dropped";
				PositionText.text = "";
			}
		}

		/// <summary>
		/// Called when an object, grabbed by the correct hand, is moved
		/// </summary>
		private void ObjectMoved(MovementInfo movementInfo) {
			PositionText.text = "x:" + movementInfo.NewPosition.x.ToString("0.000") + 
								"\ny: " + movementInfo.NewPosition.y.ToString("0.000") + 
								"\nz: " + movementInfo.NewPosition.z.ToString("0.000");
		}
	}

}