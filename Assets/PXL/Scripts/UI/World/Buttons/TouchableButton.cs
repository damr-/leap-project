using System.Collections.Generic;
using System.Linq;
using Leap;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace PXL.UI.World.Buttons {

	[RequireComponent(typeof(Touchable))]
	public abstract class TouchableButton : MonoBehaviour {

		/// <summary>
		/// The Image component of this object
		/// </summary>
		public Image Image {
			get { return mImage ?? (mImage = GetComponentInChildren<Image>()); }
		}
		private Image mImage;

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

		/// <summary>
		/// The FingerTypes which can interact with this touchable button
		/// </summary>
		public List<Finger.FingerType> InteractingFingerTypes = new List<Finger.FingerType> { Finger.FingerType.TYPE_INDEX };

		/// <summary>
		/// The color for the button when a finger hovers over it
		/// </summary>
		protected Color HoverColor = new Color(255 / 255f, 209 / 255f, 143 / 255f);

		/// <summary>
		/// The color for the button when it is pressed
		/// </summary>
		protected Color PressedColor = new Color(252 / 255f, 164 / 255f, 63 / 255f);

		/// <summary>
		/// The default color of the button
		/// </summary>
		protected Color DefaultColor;

		/// <summary>
		/// What HandSide a hand has to be of that it can press this button
		/// </summary>
		public HandSide InteractingHandSide = HandSide.Both;

		/// <summary>
		/// Whether there is a finger currently touching this level load button
		/// </summary>
		protected Fingertip Fingertip;

		/// <summary>
		/// How close the finger has to be to the button for it to be pressed
		/// </summary>
		public float PressDistance = 0.02f;

		/// <summary>
		/// If the button is currently pressed
		/// </summary>
		private bool isPressed;
		
		protected virtual void Start() {
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
			DefaultColor = Image.color;
		}

		protected virtual void Update() {
			if (Fingertip == null)
				return;
			
			var distance = Vector3.Distance(Fingertip.transform.position, transform.position);

			if (isPressed && distance > PressDistance)
				HandlePressCancelled();
			if (!isPressed && distance < PressDistance)
				HandlePressed();
		}

		/// <summary>
		/// Called when a finger enters the button's trigger
		/// </summary>
		protected virtual void HandleFingerEntered(FingerInfo fingerInfo) {
			if (!IsReactingToNewFingers())
				return;

			if (!IsValidFingerTypeTouching(fingerInfo))
				return;

			Image.color = HoverColor;
			Fingertip = fingerInfo.Fingertip;
		}

		/// <summary>
		/// Called when a finger leaves the button's trigger
		/// </summary>
		protected virtual void HandleFingerLeft(FingerInfo fingerInfo) {
			Image.color = DefaultColor;
			isPressed = false;
			Fingertip = null;
		}

		/// <summary>
		/// Called when the button is pressed
		/// </summary>
		protected virtual void HandlePressed() {
			Image.color = PressedColor;
			isPressed = true;
		}

		/// <summary>
		/// Called when the button press is interrupted
		/// </summary>
		protected virtual void HandlePressCancelled() {
			Image.color = HoverColor;
			isPressed = false;
		}

		/// <summary>
		/// Returns whether the buttons is not yet touched and can react to a new finger
		/// </summary>
		/// <returns>True if <see cref="Interaction.Fingertip"/> is null</returns>
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
		/// Returns whether the the hand of the given <see cref="Fingertip"/> is touching this button with any of the possible <see cref="InteractingFingerTypes"/>
		/// </summary>
		protected bool IsValidFingerTypeTouching(FingerInfo fingerInfo) {
			if (InteractingHandSide == HandSide.None)
				return false;

			var correctHandSide = InteractingHandSide == HandSide.Both || 
									InteractionHand.GetHandSide(fingerInfo.HandModel) == InteractingHandSide;

			var correctFingerType =
				InteractingFingerTypes.Any(
					interactingFingerType => Touchable.IsCertainFingerTouching(fingerInfo.HandModel, interactingFingerType));

			return correctHandSide && correctFingerType;
		}

	}

}