using UnityEngine;
using UniRx;

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

	public class InteractionHand : MonoBehaviour {

		/// <summary>
		/// Invoked when an object is grabbed with this hand
		/// </summary>
		public IObservable<Grabbable> ObjectGrabbed { get { return objectGrabbedSubject; } }
		private readonly ISubject<Grabbable> objectGrabbedSubject = new Subject<Grabbable>();

		/// <summary>
		/// Invoked when an object is grabbed with this hand
		/// </summary>
		public IObservable<Grabbable> ObjectDropped { get { return objectDroppedSubject; } }
		private readonly ISubject<Grabbable> objectDroppedSubject = new Subject<Grabbable>();

		/// <summary>
		/// Observable for when the object is moved while grabbed
		/// </summary>
		public IObservable<MovementInfo> ObjectMoved { get { return objectMovedSubject; } }
		private readonly ISubject<MovementInfo> objectMovedSubject = new Subject<MovementInfo>();

		public void GrabObject(Grabbable grabbable) {
			objectGrabbedSubject.OnNext(grabbable);
		}

		public void DropObject(Grabbable grabbable) {
			objectDroppedSubject.OnNext(grabbable);
		}

		public void MoveObject(MovementInfo movementInfo) {
			objectMovedSubject.OnNext(movementInfo);
		}
	}

}