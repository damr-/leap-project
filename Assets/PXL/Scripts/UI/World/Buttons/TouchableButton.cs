using System.Collections.Generic;
using System.Linq;
using Leap;
using UnityEngine;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine.UI;
using UniRx;
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
		public List<Finger.FingerType> InteractingFingerTypes = new List<Finger.FingerType>() { Finger.FingerType.TYPE_INDEX };

		/// <summary>
		/// The color for the button when a finger hovers over it
		/// </summary>
		public Color HoverColor = new Color(1f, 165 / 255f, 0f);

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
		/// The position of the finger when it starts to press the button
		/// </summary>
		private Vector3 initialFingerPos;

		/// <summary>
		/// The default position of the button
		/// </summary>
		private Vector3 defaultButtonPos;

		/// <summary>
		/// How far the finger has to move for the button to be pressed
		/// </summary>
		protected float PressDistance = 0.3f;

		protected virtual void Start() {
			Touchable.FingerEntered.Subscribe(HandleFingerEntered);
			Touchable.FingerLeft.Subscribe(HandleFingerLeft);
			defaultButtonPos = transform.localPosition;
			DefaultColor = Image.color;
		}

		protected virtual void Update() {
			if (Fingertip == null)
				return;

			var fingerDeltaPosition = Fingertip.transform.position - initialFingerPos;

			transform.localPosition =
				new Vector3(
					transform.localPosition.x,
					transform.localPosition.y,
					defaultButtonPos.z + fingerDeltaPosition.magnitude
					);

			var buttonDistance = transform.localPosition.z - defaultButtonPos.z;

			//if (transform.localPosition.z < defaultButtonPos.z) {
			//	Debug.Log("ABORT");
			//	HandleFingerLeft(new FingerInfo(FingerTip, null));
			//}

			if (buttonDistance > PressDistance) {
				Debug.LogWarning("PRESSED!!!");
				HandleFingerPressed();
			}
		}

		protected virtual void HandleFingerEntered(FingerInfo fingerInfo) {
			if (!IsReactingToNewFingers())
				return;

			if (!IsValidFingerTypeTouching(fingerInfo))
				return;

			Image.color = HoverColor;
			Fingertip = fingerInfo.Fingertip;
			initialFingerPos = Fingertip.transform.position;
		}

		protected virtual void HandleFingerLeft(FingerInfo fingerInfo) {
			Image.color = DefaultColor;
			transform.localPosition = defaultButtonPos;
		}

		protected virtual void HandleFingerPressed() {
			transform.localPosition = defaultButtonPos;
			Image.color = new Color(0, 1, 0);
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