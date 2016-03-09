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
		/// The offset of the object when being picked up
		/// </summary>
		private Vector3 offset;
		
		/// <summary>
		/// How long to wait after changing hands before being able to change again
		/// </summary>
		private float changeHandDelay = 0.25f;

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Update() {
			if (IsGrabPossible() && !isGrabbed) {
				Grab();
			}
			else if (isGrabbed) {
				if (CanHoldObject()) {
					TrackTarget();
					CheckMovement();
				}
				else{
					Drop();
				}
			}

			if (!canChangeHands && Time.time - lastChangeTime > changeHandDelay) {
				canChangeHands = true;
			}
		}

		/// <summary>
		/// Returns whether it is possible to grab this object.
		/// Checks if the hand can hold the object and whether there are enough fingers, and the thumb, touching it.
		/// </summary>
		private bool IsGrabPossible() {
			return CanHoldObject() &&
					handFingers.ContainsKey(currentHand) &&
					handFingers[currentHand].Count > minFingerCount &&
					thumbTouches;
		}

		/// <summary>
		/// Returns whether <see cref="currentHand"/> is active and the grab strength is high enough
		/// </summary>
		private bool CanHoldObject() {
			return IsHandActive() && currentHand.GetLeapHand().GrabStrength >= minGrabStrength;
		}

		/// <summary>
		/// Returns whether the current hand is valid and active
		/// </summary>
		private bool IsHandActive() {
			return currentHand != null && currentHand.isActiveAndEnabled;
		}

		/// <summary>
		/// Calls <see cref="SetGrabbed(bool)"/> to disable physics and sets the tracked target
		/// </summary>
		private void Grab() {
			SetGrabbed(true);
			GrabbingHandsManager.AddHand(currentHand);
			trackedTarget = currentHand.palm;
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

			//make it child of the palm to avoid the need to track it
			//problem: rescaling
			//transform.SetParent((grabbed ? currentHand.palm : null), true);
		}

		/// <summary>
		/// Checks whether the object has been moved beyond a certain threshold and if yes, emits the subject
		/// </summary>
		private void CheckMovement() {
			if (Vector3.Distance(transform.position, lastPosition) > moveThresHold) {
				movedSubject.OnNext(transform.position);
				lastPosition = transform.position;
			}
		}

		/// <summary>
		/// Updates the position and rotation of the object
		/// </summary>
		private void TrackTarget() {
			transform.position = CalculateObjectPosition(0.5f);
			transform.rotation = trackedTarget.rotation;
		}

		/// <summary>
		/// Returns the position of the object whilst being held
		/// </summary>
		/// <param name="offsetPercent">How many percent of the original offset should be kept when holding it</param>
		private Vector3 CalculateObjectPosition(float offsetPercent = 1f) {
			return CalculateAverageFingerPosition() + offset.magnitude * trackedTarget.up * -1 * offsetPercent;
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
			thumbTouches = IsCertainFingerTouching(currentHand, Leap.Finger.FingerType.TYPE_THUMB);
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
				// 2. the new hand has everything that is required to take the object away from the other (currently holding) hand
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
			if (hand == null || !handFingers.ContainsKey(hand))
				return false;
			return handFingers[hand].Any(ft => ft.GetComponentInParent<RigidFinger>().GetLeapFinger().Type == fingerType);
		}

		/// <summary>
		/// Called when a fingertip left the trigger of an object
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void FingerLeft(Fingertip fingertip) {
			if (isGrabbed)
				return;

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