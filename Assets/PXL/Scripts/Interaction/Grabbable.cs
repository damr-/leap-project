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
		new private Rigidbody rigidbody;

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

		private void Start() {
			rigidbody = this.TryGetComponent<Rigidbody>();
		}

		private void Update() {
			if (IsGrabPossible()) {
				if (!isGrabbed) {
					TryGrab();
				}
				else {
					TrackTarget();
				}
			}
			else {
				TryDrop();
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
		/// If <see cref="isGrabbed"/> is false, calls <see cref="Grab"/>
		/// </summary>
		private void TryGrab() {
			if (!isGrabbed)
				Grab();
		}

		/// <summary>
		/// Calls <see cref="SetGrabbed(bool)"/> to disable physics and sets the tracked target
		/// </summary>
		private void Grab() {
			SetGrabbed(true);
			UpdateTrackedTarget();

			//Debug.Log("Grabbed! " + Time.time.ToString("0.0"));
		}

		/// <summary>
		/// If <see cref="isGrabbed"/>, call <see cref="Drop"/>
		/// </summary>
		public void TryDrop() {
			if (isGrabbed)
				Drop();
		}

		/// <summary>
		/// Removes the target and hand. Calls <see cref="SetGrabbed(bool)"/> to re-enable physics
		/// </summary>
		public void Drop() {
			SetGrabbed(false);
			droppedSubject.OnNext(Unit.Default);
			trackedTarget = null;
			currentHand = null;
			//Debug.Log("Dropped! " + Time.time.ToString("0.0"));
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
		}

		/// <summary>
		/// Sets the <see cref="trackedTarget"/> to the palm of the <see cref="currentHand"/>
		/// </summary>
		private void UpdateTrackedTarget() {
			if (currentHand != null)
				trackedTarget = currentHand.palm;
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
		/// Returns whether the thumb of the given hand touches the object.
		/// </summary>
		/// <param name="hand"></param>
		/// <returns>False if hand is null or it's thumb is not touching the object</returns>
		private bool IsThumbTouching(HandModel hand) {
			if (hand == null)
				return false;
			return handFingers[hand].Any(ft => ft.GetComponentInParent<RigidFinger>().GetLeapFinger().Type == Leap.Finger.FingerType.TYPE_THUMB);
		}

		/// <summary>
		/// Drops the object when it is disabled
		/// </summary>
		private void OnDisable() {
			TryDrop();
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

			// set the current hand if
			// 1. it is the first hand touching the object
			// 2. the new hand has enough fingers and the thumb touching the object to take it away from the other (currently holding) hand			
			if (currentHand == null || handFingers[hand].Count > minFingerCount && IsThumbTouching(hand)) {
				currentHand = hand;
				UpdateTrackedTarget();
			}

			UpdateThumbTouches();
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