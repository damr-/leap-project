using UnityEngine;
using UniRx;
using System.Collections.Generic;
using PXL.Interaction.Detection;
using PXL.Utility;

namespace PXL.Interaction {

	public class ToggleGameObjectOnPose : MonoBehaviour {

		/// <summary>
		/// The PalmOrientationDetector to observe
		/// </summary>
		public DetectPalmOrientation DetectPalmOrientation;

		/// <summary>
		/// The HandPoseDetector to observe
		/// </summary>
		public DetectHandPose DetectHandPose;

		/// <summary>
		/// Whether the orientation is currect
		/// </summary>
		private bool correctOrientation;

		/// <summary>
		/// Whether the pose is correct
		/// </summary>
		private bool correctPose;

		/// <summary>
		/// Whether the target objects are currently enabled
		/// </summary>
		private bool objectsEnabled;

		/// <summary>
		/// The Objects which will be de/activated
		/// </summary>
		public List<GameObject> GameObjects = new List<GameObject>();

		private CompositeDisposable compositeDisposable = new CompositeDisposable();

		private void OnEnable() {
			compositeDisposable = new CompositeDisposable();
			objectsEnabled = false;
			Start();
		}

		private void Start() {
			GameObjects.ForEach(g => g.AssertNotNull("GameObject reference missing!"));

			DetectPalmOrientation.CorrectPose.Subscribe(_ => UpdateFlag(ref correctOrientation, true)).AddTo(compositeDisposable);
			DetectPalmOrientation.IncorrectPose.Subscribe(_ => UpdateFlag(ref correctOrientation, false)).AddTo(compositeDisposable);

			DetectHandPose.CorrectPose.Subscribe(_ => UpdateFlag(ref correctPose, true)).AddTo(compositeDisposable);
			DetectHandPose.IncorrectPose.Subscribe(_ => UpdateFlag(ref correctPose, false)).AddTo(compositeDisposable);
		}

		private void UpdateFlag(ref bool flag, bool value) {
			flag = value;
			if (value)
				EnableObjectsIfPossible();
			else
				EnableObjects(false);
		}

		private void EnableObjectsIfPossible() {
			if (correctOrientation && correctPose)
				EnableObjects(true);
		}

		private void EnableObjects(bool newEnabledState) {
			if (objectsEnabled == newEnabledState)
				return;

			Debug.Log("Objects enabled: " + (newEnabledState ? "yes" : "no"));

			objectsEnabled = newEnabledState;
			GameObjects.ForEach(c => c.SetActive(newEnabledState));
		}

		private void OnDisable() {
			compositeDisposable.Dispose();
		}

	}

}