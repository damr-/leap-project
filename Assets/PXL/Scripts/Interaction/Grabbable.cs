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
		public Vector3 PickupPosition;

		/// <summary>
		/// How long to wait after changing hands before being able to change again
		/// </summary>
		private const float ChangeHandDelay = 0.5f;

		/// <summary>
		/// The minimum grab strength necessary to pick up an object
		/// </summary>
		public static float MinGrabStrength = 0.25f;

		/// <summary>
		/// Every HandModel and its corresponding InteractionHand
		/// </summary>
		private readonly IDictionary<HandModel, InteractionHand> interactionHands =
			new Dictionary<HandModel, InteractionHand>();

		private readonly ObservableProperty<bool> isGrabbed = new ObservableProperty<bool>();

		/// <summary>
		/// Whether the object can change hands at this moment.
		/// </summary>
		private bool canChangeHands = true;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		private Touchable mTouchable;

		/// <summary>
		/// Whether this object is currently grabbed or not
		/// </summary>
		public ObservableProperty<bool> IsGrabbed {
			get { return isGrabbed; }
		}

		/// <summary>
		/// The HandModel currently grabbing the object, or at least trying to
		/// </summary>
		public HandModel CurrentHand { get; private set; }

		/// <summary>
		/// The hand interacting with this object
		/// </summary>
		public InteractionHand InteractionHand {
			get { return interactionHands.GetOrAdd(CurrentHand); }
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
			if (!isGrabbed && GrabbingHandsManager.CanHandGrab(CurrentHand) && CanHoldObject() &&
			    Touchable.CanGrabObject(CurrentHand)) {
				Grab();
			}

			if (isGrabbed && !CanHoldObject()) {
				Drop();
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
			if (isGrabbed)
				return;

			Touchable.CleanupHands(fingerInfo);
			Touchable.UpdateThumbTouches(CurrentHand);

			if (Touchable.HandFingers.Count == 0)
				CurrentHand = null;
		}

		/// <summary>
		/// Returns whether <see cref="CurrentHand" /> is valid and the grab strength is high enough
		/// </summary>
		private bool CanHoldObject() {
			return CurrentHand.IsHandValid() && CurrentHand.GetLeapHand().GrabStrength >= MinGrabStrength;
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
			isGrabbed.Value = grabbed;

			var grabbingHand = interactionHands.GetOrAdd(CurrentHand);
			if (grabbed) {
				grabbingHand.GrabObject(this);
				GrabbingHandsManager.AddHand(CurrentHand);
			}
			else {
				grabbingHand.DropObject(this);
				GrabbingHandsManager.RemoveHand(CurrentHand);
			}
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
			       Touchable.IsCertainFingerTouching(newHand, Finger.FingerType.TYPE_THUMB) &&
			       canChangeHands;
		}

	}

}