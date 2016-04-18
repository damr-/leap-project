using System.Collections.Generic;
using System.Linq;
using Leap;
using UnityEngine;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine.UI;
using UniRx;

namespace PXL.UI.World.Buttons {

	[RequireComponent(typeof(Touchable))]
	public abstract class TouchableButton : MonoBehaviour {
	
		/// <summary>
		/// The Button component of this object
		/// </summary>
		public Button Button {
			get { return mButton ?? (mButton = GetComponentInChildren<Button>()); }
		}

		private Button mButton;

		/// <summary>
		/// The Touchable component of this object
		/// </summary>
		protected Touchable Touchable {
			get { return mTouchable ?? (mTouchable = this.TryGetComponent<Touchable>()); }
		}

		private Touchable mTouchable;

		public List<Finger.FingerType> InteractingFingerTypes = new List<Finger.FingerType>() { Finger.FingerType.TYPE_INDEX };

		/// <summary>
		/// Whether there is a finger currently touching this level load button
		/// </summary>
		protected Fingertip Fingertip;

		protected virtual void Start() {
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
		}

		protected abstract void HandleFingerEntered(FingerInfo fingerInfo);

		protected abstract void HandleFingerLeft(FingerInfo fingerInfo);

		/// <summary>
		/// Returns whether the buttons is not yet touched and can react to a new finger
		/// </summary>
		/// <returns>True if <see cref="Fingertip"/> is null</returns>
		protected bool IsReactingToNewFingers() {
			return Fingertip == null;
		}

		/// <summary>
		/// Returns whether the given finger is the currently touching one
		/// </summary>
		protected bool IsTouchingFinger(FingerInfo fingerInfo) {
			return fingerInfo.Fingertip == Fingertip;
		}

		/// <summary>
		/// Returns whether the the hand of the given <see cref="FingerInfo"/> is touching this button with any of the possible <see cref="InteractingFingerTypes"/>
		/// </summary>
		protected bool IsValidFingerTypeTouching(FingerInfo fingerInfo) {
			return InteractingFingerTypes.Any(interactingFingerType => Touchable.IsCertainFingerTouching(fingerInfo.HandModel, interactingFingerType));
		}

	}

}