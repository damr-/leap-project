using System.Collections.Generic;
using UnityEngine;
using PXL.Utility;
using System.Linq;
using UniRx;

namespace PXL.Interaction {

	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class Grabbable : MonoBehaviour {
		/// <summary>
		/// Whether this object is currently grabbed or not
		/// </summary>
		public ObservableProperty<bool> IsGrabbed { get { return isGrabbed; } }
		private ObservableProperty<bool> isGrabbed = new ObservableProperty<bool>();

		/// <summary>
		/// Observable for when the object is dropped
		/// </summary>
		public IObservable<Unit> Dropped { get { return droppedSubject; } }
		private ISubject<Unit> droppedSubject = new Subject<Unit>();

		/// <summary>
		/// Observable for when the object is moved while grabbed
		/// </summary>
		public IObservable<Vector3> Moved { get { return movedSubject; } }
		private ISubject<Vector3> movedSubject = new Subject<Vector3>();

		/// <summary>
		/// The minimum grab strength necessary to pick up an object
		/// </summary>
		private const float minGrabStrength = 0.25f;

		/// <summary>
		/// The minimum fingers necessary to pick up an object
		/// </summary>
		private const int minFingerCount = 3;

		/// <summary>
		/// The HandModel currently grabbing the object, or at least trying to
		/// </summary>
		public HandModel currentHand { get; private set; }

		/// <summary>
		/// Whether the thumb is actively touching the object
		/// </summary>
		private bool thumbTouches = false;

		/// <summary>
		/// All the fingers of all hands currently overlapping this object
		/// </summary>
		private IDictionary<HandModel, HashSet<Fingertip>> handFingers = new Dictionary<HandModel, HashSet<Fingertip>>();

		/// <summary>
		/// The rigidbody component of this object
		/// </summary>
		new private Rigidbody rigidbody {
			get {
				if (m_rigidbody == null)
					m_rigidbody = this.TryGetComponent<Rigidbody>();
				return m_rigidbody;
			}
		}
		private Rigidbody m_rigidbody;

		/// <summary>
		/// The Transform this object tracks when grabbed
		/// </summary>
		private Transform trackedTarget;

		/// <summary>
		/// Position of the object in the last frame
		/// </summary>
		private Vector3 lastPosition;

		/// <summary>
		/// How far an object has to move to throw the <see cref="movedSubject"/>
		/// </summary>
		private float moveThresHold = 0.1f;

		/// <summary>
		/// Whether the object can change hands
		/// </summary>
		private bool canChangeHands = true;

		/// <summary>
		/// The last time the object changed hands
		/// </summary>
		private float lastChangeTime;

		/// <summary>
		/// Whether the offset of the object should be kept (The offset it has to the average finger position when being picked up)
		/// </summary>
		public static bool keepObjectOffset = false;
		
		/// <summary>
		/// The offset of the object when being picked up
		/// </summary>
		private Vector3 offset;

		/// <summary>
		/// How long to wait after changing hands before being able to change again
		/// </summary>
		private float changeHandDelay = 0.25f;
		
		private void Update() {
			if (IsGrabPossible()) {
				if (!isGrabbed) {
					Grab();
				}
				else {
					TrackTarget();
				}
			}
			else if (isGrabbed) {
				Drop();
			}

			if (!canChangeHands && Time.time - lastChangeTime > changeHandDelay) {
				canChangeHands = true;
			}
		}

		/// <summary>
		/// Returns whether it is possible to grab this object with the <see cref="currentHand"/>
		/// </summary>
		/// <returns></returns>
		private bool IsGrabPossible() {
			return currentHand != null &&
					currentHand.isActiveAndEnabled &&
					handFingers.ContainsKey(currentHand) &&
					handFingers[currentHand].Count > minFingerCount &&
					thumbTouches &&
					currentHand.GetLeapHand().GrabStrength >= minGrabStrength;
		}

		/// <summary>
		/// Calls <see cref="SetGrabbed(bool)"/> to disable physics and sets the tracked target
		/// </summary>
		private void Grab() {
			SetGrabbed(true);
			GrabbingHandsManager.AddHand(currentHand);
			trackedTarget = currentHand.palm;
			if(keepObjectOffset)
				offset = transform.position - CalculateAverageFingerPosition();
        }

		/// <summary>
		/// Removes the target and hand. Calls <see cref="SetGrabbed(bool)"/> to re-enable physics
		/// </summary>
		private void Drop() {
			SetGrabbed(false);
			droppedSubject.OnNext(Unit.Default);
			GrabbingHandsManager.RemoveHand(currentHand);
			trackedTarget = null;
			currentHand = null;
		}

		/// <summary>
		/// Disables Physics and sets the flag
		/// </summary>
		/// <param name="grabbed"></param>
		private void SetGrabbed(bool grabbed) {
			rigidbody.useGravity = !grabbed;
			rigidbody.isKinematic = grabbed;
			isGrabbed.Value = grabbed;
		}

		/// <summary>
		/// Updates the position and rotation of the object
		/// </summary>
		private void TrackTarget() {
			if (Vector3.Distance(transform.position, lastPosition) > moveThresHold) {
				movedSubject.OnNext(transform.position);
				lastPosition = transform.position;
			}
			transform.position = CalculateAverageFingerPosition();
			transform.rotation = trackedTarget.rotation;
			if (keepObjectOffset)
				transform.position += offset.magnitude * 0.5f * trackedTarget.up * -1;
        }

		/// <summary>
		/// Returns the average position of all fingers that touch the object
		/// </summary>
		private Vector3 CalculateAverageFingerPosition() {
			Vector3 result = Vector3.zero;
			var fingers = handFingers[currentHand];
			foreach (Fingertip tip in fingers) {
				result += tip.transform.position;
			}
			return result / fingers.Count;
		}

		/// <summary>
		/// Checks whether the thumb of the <see cref="currentHand"/> is actively touching the object and sets the corresponding flag
		/// </summary>
		private void UpdateThumbTouches() {
			if (currentHand == null || !handFingers.ContainsKey(currentHand)) {
				thumbTouches = false;
				return;
			}
			thumbTouches = handFingers[currentHand].Any(ft => ft.GetComponentInParent<RigidFinger>().GetLeapFinger().Type == Leap.Finger.FingerType.TYPE_THUMB);
		}

		/// <summary>
		/// Drops the object when it is disabled
		/// </summary>
		private void OnDisable() {
			if (isGrabbed)
				Drop();
		}

		/// <summary>
		/// Called when a fingertip entered the trigger of an object
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void FingerEntered(Fingertip fingertip) {
			HandModel hand = fingertip.handModel;

			if (handFingers.ContainsKey(hand)) {
				handFingers[hand].Add(fingertip);
			}
			else {
				handFingers.Add(hand, new HashSet<Fingertip>() { fingertip });
			}

			if (GrabbingHandsManager.CanHandGrab(hand)) {
				// set the current hand if
				// 1. it is the first hand touching the object
				if (currentHand == null) {
					currentHand = hand;
				}
				// 2. the new hand has everything that is required to take it away from the other (currently holding) hand
				else if (CanChangeHands(hand)) {
					Drop();
					currentHand = hand;
					Grab();
					canChangeHands = false;
					lastChangeTime = Time.time;
				}
			}

			UpdateThumbTouches();
		}

		/// <summary>
		/// Returns whether the object can change hands
		/// </summary>
		/// <returns></returns>
		private bool CanChangeHands(HandModel newHand) {
			return currentHand != newHand && handFingers[newHand].Count > minFingerCount && IsCertainFingerTouching(newHand, Leap.Finger.FingerType.TYPE_THUMB) && canChangeHands;
		}

		/// <summary>
		/// Returns whether a certain fingertype of the given hand touches the object.
		/// </summary>
		/// <param name="hand"></param>
		/// <returns>False if hand is null or the given finger is not touching the object</returns>
		private bool IsCertainFingerTouching(HandModel hand, Leap.Finger.FingerType fingerType) {
			if (hand == null)
				return false;
			return handFingers[hand].Any(ft => ft.GetComponentInParent<RigidFinger>().GetLeapFinger().Type == fingerType);
		}

		/// <summary>
		/// Called when a fingertip left the trigger of an object
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void FingerLeft(Fingertip fingertip) {
			HandModel hand = fingertip.handModel;

			if (handFingers.ContainsKey(hand)) {
				handFingers[hand].Remove(fingertip);

				if (handFingers[hand].Count == 0)
					handFingers.Remove(hand);
			}

			UpdateThumbTouches();
		}
	}

}