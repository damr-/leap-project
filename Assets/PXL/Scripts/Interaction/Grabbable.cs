using System;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	[RequireComponent(typeof(Touchable))]
	public class Grabbable : MonoBehaviour {

		/// <summary>
		/// The position where the object was grabbed
		/// </summary>
		public Vector3 PickupPosition { get; set; }

		/// <summary>
		/// The minimum fingers necessary to pick up the object
		/// </summary>
		public static int MinFingerCount = 2;

		/// <summary>
		/// How long to wait after changing hands before being able to change again
		/// </summary>
		private const float ChangeHandDelay = 0.5f;

		/// <summary>
		/// Every HandModel and its corresponding InteractionHand
		/// </summary>
		public static IDictionary<HandModel, InteractionHand> InteractionHands = new Dictionary<HandModel, InteractionHand>();

		/// <summary>
		/// Whether the object can change hands at this moment.
		/// </summary>
		private bool canChangeHands = true;

		/// <summary>
		/// Whether this object is currently grabbed or not
		/// </summary>
		public ObservableProperty<bool> IsGrabbed = new ObservableProperty<bool>();

		/// <summary>
		/// The HandModel currently grabbing the object, or at least trying to
		/// </summary>
		public HandModel CurrentHand { get; private set; }

		/// <summary>
		/// Whether this object can be grabbed
		/// </summary>
		public bool CanBeGrabbed { get; set; }

		/// <summary>
		/// The hand interacting with this object
		/// </summary>
		public InteractionHand InteractionHand {
			get { return InteractionHands.GetOrAdd(CurrentHand); }
		}

		/// <summary>
		/// Returns whether this <see cref="Grabbable" /> is currently stationary and not grabbed
		/// </summary>
		public bool IsStationary(float epsilon = 0.001f) {
			return !IsGrabbed && Touchable.Rigidbody.velocity.Equal(Vector3.zero, epsilon);
		}

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}
		private Touchable mTouchable;

		/// <summary>
		/// Whether the delay on dropping an object is enabled
		/// </summary>
		public bool EnableDropDelay { get; set; }

		/// <summary>
		/// For how many seconds the grab strength has been 0 while the object is grabbed
		/// </summary>
		private float noGrabTime;

		/// <summary>
		/// For how many seconds the grab strength can be 0 while grabbed before the object is dropped
		/// </summary>
		private const float MaxNoGrabTime = 0.15f;

		/// <summary>
		/// Disposable to setting the parent null interval
		/// </summary>
		private IDisposable unparentDisposable = Disposable.Empty;

		private void Start() {
			CanBeGrabbed = true;
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
		}

		private void Update() {
			if (!CanBeGrabbed)
				return;

			if (!IsGrabbed) {
				if (CurrentHand.IsHandValid() && GrabbingHandsManager.CanHandGrab(CurrentHand) &&
					Touchable.AreEnoughFingersAndIsThumbTouching(CurrentHand, MinFingerCount)) {
					Grab();
				}
				return;
			}

			if (!CurrentHand.IsHandValid()) {
				Drop();
			}
			else if (!(CurrentHand.GetLeapHand().GrabStrength > 0f)) {
				if (!EnableDropDelay) {
					Drop();
				}
				else {
					noGrabTime += Time.deltaTime;
					if (noGrabTime > MaxNoGrabTime) {
						noGrabTime = 0f;
						Drop();
					}
				}
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
				canChangeHands = false;
				Observable.Timer(TimeSpan.FromSeconds(ChangeHandDelay)).Subscribe(_ => { canChangeHands = true; });
			}

			Touchable.UpdateThumbTouches(CurrentHand);
		}

		/// <summary>
		/// Called when a finger leaves the object
		/// </summary>
		private void HandleFingerLeft(FingerInfo fingerInfo) {
			if (IsGrabbed)
				return;

			Touchable.CleanupHands(fingerInfo);
			Touchable.UpdateThumbTouches(CurrentHand);

			if (Touchable.HandFingers.Count == 0)
				CurrentHand = null;
		}

		/// <summary>
		/// Sets up everything for the object to be grabbed and moved around
		/// </summary>
		private void Grab() {
			PickupPosition = transform.position;
			SetGrabbed(true);
		}

		/// <summary>
		/// Sets up everything for the object to be dropped again.
		/// </summary>
		private void Drop() {
			SetGrabbed(false);
			CurrentHand = null;
		}

		/// <summary>
		/// Disables physics and sets the observable flag.
		/// Updates the GrabbindHandsManager and the Grabbing hand of this object.
		/// </summary>
		private void SetGrabbed(bool grabbed) {
			Touchable.Rigidbody.useGravity = !grabbed;
			Touchable.Rigidbody.isKinematic = grabbed;

			unparentDisposable.Dispose();

			if (grabbed) {
				transform.SetParent(CurrentHand.palm, true);
			}
			else {
				unparentDisposable = Observable.Interval(TimeSpan.FromSeconds(0.01f)).Subscribe(_ => {
					if (transform.parent == null) {
						unparentDisposable.Dispose();
						return;
					}
					transform.SetParent(null, true);
				});
			}

			var grabbingHand = InteractionHands.GetOrAdd(CurrentHand);
			if (grabbed) {
				grabbingHand.GrabObject(this);
				GrabbingHandsManager.AddHand(CurrentHand);
			}
			else {
				grabbingHand.DropObject(this);
				GrabbingHandsManager.RemoveHand(CurrentHand);
			}

			IsGrabbed.Value = grabbed;
		}

		/// <summary>
		/// Returns whether the object can change hands
		/// </summary>
		private bool CanChangeHands(HandModel newHand) {
			return CurrentHand != newHand &&
				   Touchable.HandFingers[newHand].Count > MinFingerCount &&
				   Touchable.IsCertainFingerTouching(newHand, Finger.FingerType.TYPE_THUMB) &&
				   canChangeHands;
		}

		/// <summary>
		/// Drops the object when it is disabled
		/// </summary>
		private void OnDisable() {
			if (IsGrabbed)
				Drop();
			CurrentHand = null;
		}

	}

}