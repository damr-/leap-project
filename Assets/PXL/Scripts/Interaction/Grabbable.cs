using PXL.Gamemodes;
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
		/// The HandModel currently grabbing the object, or at least trying to
		/// </summary>
		public HandModel CurrentHand { get; private set; }

		/// <summary>
		/// The minimum grab strength necessary to pick up an object
		/// </summary>
		private const float MinGrabStrength = 0.25f;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}
		private Touchable mTouchable;

		/// <summary>
		/// How long to wait after changing hands before being able to change again
		/// </summary>
		private const float ChangeHandDelay = 0.5f;

		/// <summary>
		/// The last time the object changed hands
		/// </summary>
		private float lastChangeTime;

		/// <summary>
		/// Whether the object can change hands at this moment.
		/// </summary>
		private bool canChangeHands;

		/// <summary>
		/// Sets up the subscriptions
		/// </summary>
		private void Start() {
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
		}

		/// <summary>
		/// Updates the state of the object
		/// </summary>
		private void Update() {
			if (!isGrabbed && CanHoldObject() && Touchable.CanGrabObject(CurrentHand)) {
				Grab();
			}

			if (isGrabbed && !CanHoldObject()) {
				Drop();
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
			if (!GrabbingHandsManager.CanHandGrab(hand)) {
				Touchable.UpdateThumbTouches(CurrentHand);
				return;
			}

			//set the current hand if
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

			Touchable.UpdateThumbTouches(CurrentHand);
		}

		/// <summary>
		/// Called when a finger leaves the object
		/// </summary>
		private void HandleFingerLeft(FingerInfo fingerInfo) {
			if (isGrabbed)
				return;

			Touchable.CleanupHands(fingerInfo);
			Touchable.UpdateThumbTouches(CurrentHand);

			if (Touchable.HandFingers.Count == 0)
				CurrentHand = null;
		}

		/// <summary>
		/// Returns whether <see cref="CurrentHand"/> is valid and the grab strength is high enough
		/// </summary>
		private bool CanHoldObject() {
			return CurrentHand.IsHandValid() && CurrentHand.GetLeapHand().GrabStrength >= MinGrabStrength;
		}

		/// <summary>
		/// Sets up everything for the object to be grabbed and moved around
		/// Notifies <see cref="GrabbingHandsManager"/> that <see cref="CurrentHand"/> is now grabbing an object.
		/// </summary>
		private void Grab() {
			SetGrabbed(true);
			GrabbingHandsManager.AddHand(CurrentHand);
		}

		/// <summary>
		/// Unsets <see cref="CurrentHand"/> and calls <see cref="SetGrabbed(bool)"/> to re-enable physics
		/// Notifies <see cref="GrabbingHandsManager"/> that <see cref="CurrentHand"/> is no longer grabbing an object.
		/// </summary>
		private void Drop() {
			SetGrabbed(false);
			GrabbingHandsManager.RemoveHand(CurrentHand);
			CurrentHand = null;
		}

		/// <summary>
		/// Disables physics and sets the observable flag
		/// </summary>
		private void SetGrabbed(bool grabbed) {
			Touchable.Rigidbody.useGravity = !grabbed;
			Touchable.Rigidbody.isKinematic = grabbed;
			isGrabbed.Value = grabbed;
		}

		/// <summary>
		/// Drops the object when it is disabled
		/// </summary>
		private void OnDisable() {
			if (isGrabbed)
				Drop();
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