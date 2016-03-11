using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	[RequireComponent(typeof(Grabbable))]
	public class Moveable : MonoBehaviour {

		/// <summary>
		/// Observable for when the object is moved while grabbed
		/// </summary>
		public IObservable<MovementInfo> MovedWhileGrabbed { get { return movedWhileGrabbedSubject; } }
		private readonly ISubject<MovementInfo> movedWhileGrabbedSubject = new Subject<MovementInfo>();

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Grabbable Grabbable {
			get { return mGrabbable ?? (mGrabbable = this.TryGetComponent<Grabbable>()); }
		}
		private Grabbable mGrabbable;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}
		private Touchable mTouchable;

		/// <summary>
		/// The Transform this object tracks when grabbed
		/// </summary>
		private Transform trackedTarget;

		/// <summary>
		/// Position of the object in the last frame
		/// </summary>
		private Vector3 lastPosition;

		/// <summary>
		/// How far an object has to move to invoke <see cref="movedWhileGrabbedSubject"/>
		/// </summary>
		private const float MoveThresHold = 0.1f;

		/// <summary>
		/// The offset of the object's position when being picked up
		/// </summary>
		private Vector3 posOffset;

		/// <summary>
		/// The offset of the object's rotation when begin picked up
		/// </summary>
		private Quaternion rotOffset;
		
		/// <summary>
		/// Sets up subscriptions
		/// </summary>
		private void Start() {
			Grabbable.IsGrabbed.Subscribe(HandleGrabStateChange);
		}

		/// <summary>
		/// Updates the state, position and rotation of the object, if grabbed
		/// </summary>
		private void Update() {
			if (!Grabbable.IsGrabbed)
				return;

			transform.position = CalculateObjectPosition(0.5f);
			transform.rotation = trackedTarget.rotation * rotOffset;

			CheckMovement(movedWhileGrabbedSubject);
		}

		/// <summary>
		/// Called when the grabbed state of the object changes
		/// </summary>
		/// <param name="grabbed"></param>
		private void HandleGrabStateChange(bool grabbed) {
			if (grabbed) {
				trackedTarget = Grabbable.CurrentHand.palm;
				posOffset = transform.position - Touchable.GetAverageFingerPosition(Grabbable.CurrentHand);
				rotOffset = Quaternion.Inverse(transform.rotation) * trackedTarget.rotation;
			}
			else {
				trackedTarget = null;
			}
		}

		/// <summary>
		/// Checks whether the object has been moved beyond a certain threshold and if yes, emits the subject
		/// </summary>
		private void CheckMovement(ISubject<MovementInfo> subject) {
			if (!(Vector3.Distance(transform.position, lastPosition) > MoveThresHold))
				return;
			subject.OnNext(new MovementInfo(Vector3.Distance(transform.position, lastPosition), transform.position));
			lastPosition = transform.position;
		}

		/// <summary>
		/// Returns the position of the object whilst being held
		/// </summary>
		/// <param name="offsetPercent">How many percent of the original offset should be kept when holding it</param>
		private Vector3 CalculateObjectPosition(float offsetPercent = 1f) {
			return Touchable.GetAverageFingerPosition(Grabbable.CurrentHand) + posOffset.magnitude * trackedTarget.up * -1 * offsetPercent;
		}

		/// <summary>
		/// Remove the tracked target when the object is destroyed
		/// </summary>
		private void OnDisable() {
			trackedTarget = null;
		}

	}

}