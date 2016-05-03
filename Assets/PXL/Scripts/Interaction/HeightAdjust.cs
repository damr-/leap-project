using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.Interaction {

	public class HeightAdjust : HandPoseObserver {

		/// <summary>
		/// The movement component of the camera
		/// </summary>
		public Movement.Movement Movement;

		/// <summary>
		/// Whether the vertical position of the camera is currently adjusting
		/// </summary>
		private bool isAdjusting;

		/// <summary>
		/// The object of the worldspace message object
		/// </summary>
		public GameObject MessageObject;

		/// <summary>
		/// The collider of the finish button
		/// </summary>
		public Collider FinishedButtonCollider;

		/// <summary>
		/// The image showing that the button is disabled
		/// </summary>
		public Image FinishedButtonBlockedImage;

		/// <summary>
		/// The allowed difference in vertical position for a hand to be treated als correctly placed
		/// </summary>
		private const float Epsilon = 0.0025f;

		/// <summary>
		/// The transform component of the first hand's palm
		/// </summary>
		private Transform palm1;

		/// <summary>
		/// The transform component of the second hand's palm
		/// </summary>
		private Transform palm2;

		protected override void Start() {
			base.Start();

			MessageObject.AssertNotNull("Missing message object reference!");
			FinishedButtonCollider.AssertNotNull("Missing finish button collider reference");
			FinishedButtonBlockedImage.AssertNotNull("Missing finish button blocked image reference");

			if (DetectHandPose.HandModels.Count != 2)
				throw new MissingReferenceException("Not exactly 2 hands given!");

			palm1 = DetectHandPose.HandModels[0].palm;
			palm2 = DetectHandPose.HandModels[1].palm;
		}

		private void Update() {
			if (!isAdjusting)
				return;

			var diff = palm1.position.y - palm2.position.y;

			if (diff > 0.5f) {
				StopAdjusting();
				return;
			}

			var distance1 = palm1.position.y - transform.position.y;
			var distance2 = palm2.position.y - transform.position.y;

			var distance = Mathf.Abs(distance1) < Mathf.Abs(distance2) ? distance1 : distance2;

			SetButtonInteractable(false);
			if (distance > Epsilon)
				Movement.OverwriteVelocity(Movement.Speed * Vector3.down);
			else if (distance < -Epsilon)
				Movement.OverwriteVelocity(Movement.Speed * Vector3.up);
			else {
				SetButtonInteractable(true);
				Movement.OverwriteVelocity(Vector3.zero);
			}
		}

		/// <summary>
		/// Called when looking at the finish setup button
		/// </summary>
		public void HandleCalibrationFinished() {
			isAdjusting = false;
			Movement.OverwriteVelocity(Vector3.zero);
			MessageObject.SetActive(false);
			DisposeSubscriptions();
		}

		protected override void HandleIncorrectPose() {
			if (!isAdjusting)
				return;
			SetButtonInteractable(false);
			StopAdjusting();
		}
		
		protected override void HandleCorrectPose() {
			if (isAdjusting)
				return;
			isAdjusting = true;
		}

		/// <summary>
		/// Stops adjusting and disables the button
		/// </summary>
		private void StopAdjusting() {
			isAdjusting = false;
			SetButtonInteractable(false);
			Movement.OverwriteVelocity(Vector3.zero);
		}

		/// <summary>
		/// Sets the enabled state of the button's collider and image according to the given value
		/// </summary>
		/// <param name="interactable">Whether the button is interactable</param>
		private void SetButtonInteractable(bool interactable) {
			FinishedButtonCollider.enabled = interactable;
			FinishedButtonBlockedImage.enabled = !interactable;
		}

	}

}