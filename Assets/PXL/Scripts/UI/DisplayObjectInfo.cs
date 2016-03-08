using UnityEngine;
using PXL.Objects;
using System.Linq;
using UniRx;
using PXL.Interaction;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI {

	[RequireComponent(typeof(Text))]
	public class DisplayObjectInfo : MonoBehaviour {

		/// <summary>
		/// The observed ObjectManager
		/// </summary>
		public ObjectManager objectManager;

		/// <summary>
		/// Text component of a different object which displayes the position of the grabbed object
		/// </summary>
		public Text positionText;

		/// <summary>
		/// Whether this display is for the left hand
		/// </summary>
		public bool trackLeftHand = false;

		/// <summary>
		/// The Text component of this GameObject
		/// </summary>
		private Text text;

		private void Start() {
			text = this.TryGetComponent<Text>();
			objectManager.ObjectSpawned.Subscribe(ObjectSpawned);

			if (positionText == null)
				throw new MissingReferenceException("No positionText assigned!");
		}

		private void ObjectSpawned(ObjectBehaviour objectBehaviour) {
			Grabbable grabbable = objectBehaviour.GetComponent<Grabbable>();

			if (grabbable == null)
				return;

			grabbable.IsGrabbed.Where(grabbed => grabbed).Subscribe(_ => ObjectGrabbed(grabbable));
			grabbable.Dropped.Subscribe(_ => ObjectDropped(grabbable));
			grabbable.Moved.Subscribe(pos => ObjectMoved(grabbable, pos));
		}

		/// <summary>
		/// Called when an object was grabbed. Displayes this info if it's the correct hand
		/// </summary>
		/// <param name="grabbable">The object</param>
		private void ObjectGrabbed(Grabbable grabbable) {
			HandModel hand = grabbable.currentHand;
			if (hand != null && hand.GetLeapHand().IsLeft == trackLeftHand)
				text.text = grabbable.gameObject.name + " grabbed";
		}

		/// <summary>
		/// Called when an object is dropped from the correct hand and displays this info.
		/// </summary>
		/// <param name="grabbable">The object</param>
		private void ObjectDropped(Grabbable grabbable) {
			HandModel hand = grabbable.currentHand;
			if (hand != null && hand.GetLeapHand().IsLeft == trackLeftHand) {
				text.text = grabbable.gameObject.name + " dropped";
				positionText.text = "";
            }
		}

		/// <summary>
		/// Called when an object, grabbed by the correct hand, is moved
		/// </summary>
		/// <param name="grabbable">The object</param>
		/// <param name="newPosition">The new position of the object</param>
		private void ObjectMoved(Grabbable grabbable, Vector3 newPosition) {
			HandModel hand = grabbable.currentHand;
			if (hand != null && hand.GetLeapHand().IsLeft == trackLeftHand) {
				positionText.text = "x:" + newPosition.x.ToString("0.000") + "\ny: " + newPosition.y.ToString("0.000") + "\nz: " + newPosition.z.ToString("0.000");
			}
		}
	}

}