using UnityEngine;
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

			grabbable.IsGrabbed.Where(grabbed => grabbed).Subscribe(_ => ObjectGrabbed(grabbable));
			grabbable.Dropped.Subscribe(_ => ObjectDropped(grabbable));
			grabbable.MovedWhileGrabbed.Subscribe(movementInfo => ObjectMoved(grabbable, movementInfo));
		}

		/// <summary>
		/// Called when an object was grabbed. Displayes this info if it's the correct hand
		/// </summary>
		/// <param name="grabbable">The object</param>
		private void ObjectGrabbed(Grabbable grabbable) {
			var hand = grabbable.CurrentHand;
			if (hand != null && hand.GetLeapHand().IsLeft == TrackLeftHand)
				Text.text = grabbable.gameObject.name + " grabbed";
		}

		/// <summary>
		/// Called when an object is dropped from the correct hand and displays this info.
		/// </summary>
		/// <param name="grabbable">The object</param>
		private void ObjectDropped(Grabbable grabbable) {
			var hand = grabbable.CurrentHand;
			if (hand != null && hand.GetLeapHand().IsLeft == TrackLeftHand) {
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