using PXL.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PXL.Interaction {

	public class HeightAdjust : HandPoseObserver {

		/// <summary>
		/// The movement component of the camera
		/// </summary>
		public Movement.Movement Movement;

		/// <summary>
		/// Key used to skip the setup process
		/// </summary>
		public KeyCode SkipKey = KeyCode.V;

		/// <summary>
		/// Whether the vertical position of the camera is currently adjusting
		/// </summary>
		private bool isAdjusting;

		/// <summary>
		/// The object of the worldspace message object
		/// </summary>
		public GameObject MessageObject;

		/// <summary>
		/// The slider to show the calibration progress
		/// </summary>
		public Slider CalibrationSlider;

		/// <summary>
		/// The allowed difference in vertical position for a hand to be treated als correctly placed
		/// </summary>
		public float Threshold = 0.025f;

		/// <summary>
		/// How long to calibrate before accepting the current hands' poses
		/// </summary>
		public float CalibrateTime = 1f;

		/// <summary>
		/// Event for when the calibration is finished
		/// </summary>
		public UnityEvent OnCalibrationFinished;

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
			CalibrationSlider.AssertNotNull("Missing calibration slider reference");

			palm1 = DetectHandPose.HandModels[0].palm;
			palm2 = DetectHandPose.HandModels.Count > 1 ? DetectHandPose.HandModels[1].palm : null;

			CalibrationSlider.value = 0f;
		}

		private void Update() {
			if (Input.GetKeyDown(SkipKey))
				HandleCalibrationFinished();

			if (!isAdjusting)
				return;

			var distance1 = palm1.position.y - transform.position.y;
			var distance = distance1;

			if (palm2 != null) {
				var distance2 = palm2.position.y - transform.position.y;
				distance = Mathf.Abs(distance1) < Mathf.Abs(distance2) ? distance1 : distance2;
			}

			if (distance > Threshold) {
				Movement.OverwriteVelocity(Movement.Speed * Vector3.down);
				CalibrationSlider.value = 0f;
			}
			else if (distance < -Threshold) {
				Movement.OverwriteVelocity(Movement.Speed * Vector3.up);
				CalibrationSlider.value = 0f;
			}
			else {
				CalibrationSlider.value += Time.deltaTime / CalibrateTime;
				Movement.OverwriteVelocity(Vector3.zero);
				if (CalibrationSlider.value >= CalibrateTime)
					HandleCalibrationFinished();
			}
		}

		/// <summary>
		/// Called when looking at the finish setup button
		/// </summary>
		public void HandleCalibrationFinished() {
			isAdjusting = false;
			CalibrationSlider.value = 1f;
			Movement.OverwriteVelocity(Vector3.zero);
			MessageObject.SetActive(false);
			DisposeSubscriptions();
			OnCalibrationFinished.Invoke();
		}

		protected override void HandleIncorrectPose() {
			if (!isAdjusting)
				return;
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
			CalibrationSlider.value = 0f;
			Movement.OverwriteVelocity(Vector3.zero);
		}

	}

}