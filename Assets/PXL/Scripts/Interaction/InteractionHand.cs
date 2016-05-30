using Leap.Unity;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	public enum HandSide {
		None = 0,
		Left = 1,
		Right = 2,
		Both = 3
	}

	public enum InteractionType {
		Touch = 0,
		EndTouch = 1,
		Grab = 2,
		Drop = 3,
		Move = 4
	}

	/// <summary>
	/// This class provides observables to a hand object which are invoked whenever the hand interacts with an object.
	/// It can also give information about the hand side of the hand if possible.
	/// </summary>
	public class InteractionHand : MonoBehaviour {

		/// <summary>
		/// Subject when the hand drops and object
		/// </summary>
		private readonly ISubject<Grabbable> objectDroppedSubject = new Subject<Grabbable>();

		/// <summary>
		/// Subject when the hand grabs an object
		/// </summary>
		private readonly ISubject<Grabbable> objectGrabbedSubject = new Subject<Grabbable>();

		/// <summary>
		/// Subject when the hand moves a grabbed object
		/// </summary>
		private readonly ISubject<MovementInfo> objectMovedSubject = new Subject<MovementInfo>();

		/// <summary>
		/// Invoked when an object is grabbed with this hand
		/// </summary>
		public IObservable<Grabbable> ObjectGrabbed {
			get { return objectGrabbedSubject; }
		}

		/// <summary>
		/// Invoked when an object is grabbed with this hand
		/// </summary>
		public IObservable<Grabbable> ObjectDropped {
			get { return objectDroppedSubject; }
		}

		/// <summary>
		/// Observable for when the object is moved while grabbed
		/// </summary>
		public IObservable<MovementInfo> ObjectMoved {
			get { return objectMovedSubject; }
		}

		public void GrabObject(Grabbable grabbable) {
			objectGrabbedSubject.OnNext(grabbable);
		}

		public void DropObject(Grabbable grabbable) {
			objectDroppedSubject.OnNext(grabbable);
		}

		public void MoveObject(MovementInfo movementInfo) {
			objectMovedSubject.OnNext(movementInfo);
		}

		public static HandSide GetHandSide(HandModel hand) {
			if (!hand.IsHandValid())
				return HandSide.None;
			return hand.GetLeapHand().IsLeft ? HandSide.Left : HandSide.Right;
		}

	}

}