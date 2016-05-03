using System.Linq;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.Interaction {

	public class HeightAdjust : HandPoseObserver {

		public Movement.Movement Movement;

		private bool canAdjust;

		public GameObject MessageObject;

		public Collider FinishedButtonCollider;

		public Image FinishedButtonBlockedImage;

		private const float Epsilon = 0.0025f;

		private Transform palm1, palm2;

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
			if (!canAdjust)
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
			canAdjust = false;
			Movement.OverwriteVelocity(Vector3.zero);
			MessageObject.SetActive(false);
			DisposeSubscriptions();
		}

		protected override void HandleIncorrectPose() {
			if (!canAdjust)
				return;
			SetButtonInteractable(false);
			StopAdjusting();
		}

		protected override void HandleCorrectPose() {
			if (canAdjust)
				return;
			canAdjust = true;
		}

		private void StopAdjusting() {
			canAdjust = false;
			SetButtonInteractable(false);
			Movement.OverwriteVelocity(Vector3.zero);
		}

		private void SetButtonInteractable(bool interactable) {
			FinishedButtonCollider.enabled = interactable;
			FinishedButtonBlockedImage.enabled = !interactable;
		}

	}

}