using PXL.Objects;
using UniRx;
using PXL.Interaction;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI {

	public class DisplayObjectInfo : DisplayBase {

		/// <summary>
		/// The observed ObjectManager
		/// </summary>
		public ObjectManager ObjectManager;

		/// <summary>
		/// Text component of a different object which displayes the position of the grabbed object
		/// </summary>
		public Text PositionText;

		/// <summary>
		/// Whether this display is for the left hand
		/// </summary>
		public bool TrackLeftHand = false;

		protected override void Start() {
			base.Start();
			ObjectManager.AssertNotNull();
			ObjectManager.ObjectSpawned.Subscribe(ObjectSpawned);
			PositionText.AssertNotNull();
		}

		private void ObjectSpawned(ObjectBehaviour objectBehaviour) {
			var grabbable = objectBehaviour.GetComponent<Grabbable>();

			if (grabbable == null)
				return;

			grabbable.IsGrabbed.Subscribe(grabbed => ObjectGrabStateChange(grabbable, grabbed));

			var moveable = objectBehaviour.GetComponent<Moveable>();

			if (moveable != null)
				moveable.MovedWhileGrabbed.Subscribe(movementInfo => ObjectMoved(grabbable, movementInfo));
		}

		/// <summary>
		/// Called when an object was grabbed. Displayes this info if it's the correct hand
		/// </summary>
		/// <param name="grabbable">The object</param>
		private void ObjectGrabStateChange(Grabbable grabbable, bool grabbed) {
			var hand = grabbable.CurrentHand;

			if (hand == null || hand.GetLeapHand().IsLeft != TrackLeftHand)
				return;

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
		private void ObjectMoved(Grabbable grabbable, MovementInfo movementInfo) {
			var hand = grabbable.CurrentHand;
			if (hand != null && hand.GetLeapHand().IsLeft == TrackLeftHand) {
				PositionText.text = "x:" + movementInfo.NewPosition.x.ToString("0.000") + "\ny: " + movementInfo.NewPosition.y.ToString("0.000") + "\nz: " + movementInfo.NewPosition.z.ToString("0.000");
			}
		}
	}

}