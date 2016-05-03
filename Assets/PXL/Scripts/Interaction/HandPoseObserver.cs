using PXL.Interaction.Detection;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	public abstract class HandPoseObserver : MonoBehaviour {

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
		protected bool CorrectOrientation;

		/// <summary>
		/// Whether the pose is correct
		/// </summary>
		protected bool CorrectPose;

		/// <summary>
		/// All the subscriptions to (in)correct pose observables
		/// </summary>
		protected CompositeDisposable Disposables = new CompositeDisposable();

		protected virtual void OnEnable() {
			Disposables = new CompositeDisposable();
			Start();
		}

		protected virtual void Start() {
			DetectPalmOrientation.CorrectPose.Subscribe(_ => UpdateFlag(ref CorrectOrientation, true)).AddTo(Disposables);
			DetectPalmOrientation.IncorrectPose.Subscribe(_ => UpdateFlag(ref CorrectOrientation, false)).AddTo(Disposables);

			DetectHandPose.CorrectPose.Subscribe(_ => UpdateFlag(ref CorrectPose, true)).AddTo(Disposables);
			DetectHandPose.IncorrectPose.Subscribe(_ => UpdateFlag(ref CorrectPose, false)).AddTo(Disposables);
		}

		/// <summary>
		/// Sets the value of <see cref="flag"/> to the <see cref="newValue"/>.
		/// If the <see cref="newValue"/> is true, calls <see cref="HandleCorrectPose"/>.
		/// Otherwise calls <see cref="HandleIncorrectPose"/>.
		/// </summary>
		/// <param name="flag">The flag zu update</param>
		/// <param name="newValue">The new value for the flag</param>
		private void UpdateFlag(ref bool flag, bool newValue) {
			flag = newValue;
			if (newValue)
				HandleCorrectFlag();
			else
				HandleIncorrectPose();
		}

		private void HandleCorrectFlag() {
			if (CorrectOrientation && CorrectPose)
				HandleCorrectPose();
		}
		
		protected abstract void HandleCorrectPose();

		protected abstract void HandleIncorrectPose();

		protected virtual void OnDisable() {
			DisposeSubscriptions();
		}

		protected void DisposeSubscriptions() {
			Disposables.Dispose();
		}

	}

}