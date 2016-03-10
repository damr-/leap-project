using UnityEngine;
using PXL.Utility;
using UniRx;

namespace PXL.Interaction {

	public class MovementInfo {
		public float Delta;
		public Vector3 NewPosition;

		public MovementInfo(float delta, Vector3 newPosition) {
			Delta = delta;
			NewPosition = newPosition;
		}
	}

	[RequireComponent(typeof(Touchable))]
	public class Grabbable : MonoBehaviour {

		/// <summary>
		/// Whether this object is currently grabbed or not
		/// </summary>
		public ObservableProperty<bool> IsGrabbed { get { return isGrabbed; } }
		private readonly ObservableProperty<bool> isGrabbed = new ObservableProperty<bool>();

		/// <summary>
		/// Observable for when the object is dropped
		/// </summary>
		public IObservable<Unit> Dropped { get { return droppedSubject; } }
		private readonly ISubject<Unit> droppedSubject = new Subject<Unit>();

		/// <summary>
		/// Observable for when the object is moved while grabbed
		/// </summary>
		public IObservable<MovementInfo> MovedWhileGrabbed { get { return movedWhileGrabbedSubject; } }
		private readonly ISubject<MovementInfo> movedWhileGrabbedSubject = new Subject<MovementInfo>();

		/// <summary>
		/// The minimum grab strength necessary to pick up an object
		/// </summary>
		private const float MinGrabStrength = 0.25f;

		/// <summary>
		/// The HandModel currently grabbing the object, or at least trying to
		/// </summary>
		public HandModel CurrentHand { get; private set; }

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}
		private Touchable mTouchable;

		/// <summary>
		/// The Rigidbody component of this object
		/// </summary>
		private Rigidbody Rigidbody {
			get { return mRigidbody ?? (mRigidbody = this.TryGetComponent<Rigidbody>()); }
		}
		private Rigidbody mRigidbody;

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
		/// The last time the object changed hands
		/// </summary>
		private float lastChangeTime;

		/// <summary>
		/// The offset of the object's position when being picked up
		/// </summary>
		private Vector3 posOffset;

		/// <summary>
		/// The offset of the object's rotation when begin picked up
		/// </summary>
		private Quaternion rotOffset;

		/// <summary>
		/// How long to wait after changing hands before being able to change again
		/// </summary>
		private const float ChangeHandDelay = 0.25f;

		/// <summary>
		/// Whether the object can change hands at this moment.
		/// </summary>
		private bool canChangeHands;

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Start() {
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
		}

		private void Update() {
			if (!isGrabbed && CanHoldObject() && Touchable.CanGrabObject(CurrentHand)) {
				Grab();
			}

			if (isGrabbed) {
				if (CanHoldObject()) {
					transform.position = CalculateObjectPosition(0.5f);
					transform.rotation = trackedTarget.rotation * rotOffset;

					CheckMovement(movedWhileGrabbedSubject);
				}
				else {
					Drop();
				}
			}

			if (!canChangeHands && Time.time - lastChangeTime > ChangeHandDelay) {
				canChangeHands = true;
			}
		}

		/// <summary>
		/// Called when a finger enters the object
		/// </summary>
		private void HandleFingerEntered(FingerInfo fingerInfo) {
			var hand = fingerInfo.HandModel;
			if (GrabbingHandsManager.CanHandGrab(hand)) {
				// set the current hand if
				// 1. it is the first hand touching the object
				if (CurrentHand == null) {
					CurrentHand = hand;
				}
				// 2. the new hand has everything that is required to take the object away from the other (currently holding) hand
				else if (CanChangeHands(hand)) {
					Drop();
					CurrentHand = hand;
					Grab();
					lastChangeTime = Time.time;
					canChangeHands = false;
				}
			}
			Touchable.UpdateThumbTouches(CurrentHand);
		}

		/// <summary>
		/// Called when a finger leaves the object
		/// </summary>
		private void HandleFingerLeft(FingerInfo fingerInfo) {
			if (isGrabbed)
				return;
				
			Touchable.CleanHandsDictionary(fingerInfo);
			Touchable.UpdateThumbTouches(CurrentHand);

			if (Touchable.HandFingers.Count == 0)
				CurrentHand = null;
		}

		/// <summary>
		/// Returns whether <see cref="CurrentHand"/> is active and the grab strength is high enough
		/// </summary>
		private bool CanHoldObject() {
			return CurrentHand.IsHandValid() && CurrentHand.GetLeapHand().GrabStrength >= MinGrabStrength;
		}

		/// <summary>
		/// Calls <see cref="SetGrabbed(bool)"/> to disable physics and sets the tracked target
		/// </summary>
		private void Grab() {
			SetGrabbed(true);
			GrabbingHandsManager.AddHand(CurrentHand);
			trackedTarget = CurrentHand.palm;
			posOffset = transform.position - Touchable.GetAverageFingerPosition(CurrentHand);
			rotOffset = Quaternion.Inverse(transform.rotation) * trackedTarget.rotation;
		}

		/// <summary>
		/// Removes the target and hand. Calls <see cref="SetGrabbed(bool)"/> to re-enable physics
		/// </summary>
		private void Drop() {
			SetGrabbed(false);
			droppedSubject.OnNext(Unit.Default);
			GrabbingHandsManager.RemoveHand(CurrentHand);
			trackedTarget = null;
			CurrentHand = null;
		}

		/// <summary>
		/// Disables Physics and sets the flag
		/// </summary>
		/// <param name="grabbed"></param>
		private void SetGrabbed(bool grabbed) {
			Rigidbody.useGravity = !grabbed;
			Rigidbody.isKinematic = grabbed;
			isGrabbed.Value = grabbed;
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
			return Touchable.GetAverageFingerPosition(CurrentHand) + posOffset.magnitude * trackedTarget.up * -1 * offsetPercent;
		}

		/// <summary>
		/// Drops the object when it is disabled
		/// </summary>
		private void OnDisable() {
			if (isGrabbed)
				Drop();
			trackedTarget = null;
			CurrentHand = null;
		}

		/// <summary>
		/// Returns whether the object can change hands
		/// </summary>
		/// <returns></returns>
		private bool CanChangeHands(HandModel newHand) {
			return CurrentHand != newHand &&
					Touchable.HandFingers[newHand].Count > Touchable.MinFingerCount &&
					Touchable.IsCertainFingerTouching(newHand, Leap.Finger.FingerType.TYPE_THUMB) &&
					canChangeHands;
		}
	}

}