using PXL.Interaction;
using PXL.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PXL.Objects.WhackAMole {

	public class Mole : MonoBehaviour {

		private enum MoleState {
			Animating = 0,
			ChangingPosition = 1
		}

		public ObjectType InteractiveObjectType = ObjectType.Weapon;

		private Animation animComponent;

		private float changePositionDelay = 1f;

		private const float MinChangePositionDelay = 0.1f;

		private const float MaxChangePositionDelay = 3f;

		private float changePositionStartTime;

		private float animationStartTime;

		private MoleState state = MoleState.ChangingPosition;

		private WhackAMoleManager whackAMoleManager;

		private Transform currentSpawnPoint;

		private Collider MoleCollider {
			get { return mTrigger ?? (mTrigger = this.TryGetComponent<Collider>()); }
		}
		private Collider mTrigger;

		public void Setup(WhackAMoleManager manager) {
			whackAMoleManager = manager;
			animComponent = GetComponent<Animation>();
			UpdateChangePositionDelay();
		}

		private void Update() {
			if (state == MoleState.ChangingPosition && Time.time - changePositionStartTime > changePositionDelay) {
				animComponent.Stop();

				currentSpawnPoint = whackAMoleManager.GetRandomFreeSpawnPoint();
				if (currentSpawnPoint == null) {
					this.Kill();
					return;
				}

				transform.parent = currentSpawnPoint;

				animComponent.Play();

				animationStartTime = Time.time;
				state = MoleState.Animating;
			}

			if (state == MoleState.Animating && Time.time - animationStartTime > animComponent.clip.length) {
				whackAMoleManager.FreeSpawnPoint(currentSpawnPoint);
				UpdateChangePositionDelay();
				changePositionStartTime = Time.time;
				state = MoleState.ChangingPosition;
			}
		}

		public void EnableTrigger() {
			MoleCollider.enabled = true;
		}

		public void DisableTrigger() {
			MoleCollider.enabled = false;
		}

		private void OnTriggerEnter(Collider other) {
			var i = other.GetComponent<InteractiveObject>();

			if (i == null || (InteractiveObjectType != ObjectType.All && i.ObjectType != InteractiveObjectType))
				return;

			var g = other.GetComponentInParent<Grabbable>();

			if (g == null || g.IsGrabbed == false)
				return;

			whackAMoleManager.CreateNewMole();
			this.Kill();
		}

		private void UpdateChangePositionDelay() {
			changePositionDelay = Random.Range(MinChangePositionDelay, MaxChangePositionDelay);
		}

		private void OnDisable() {
			if (currentSpawnPoint != null)
				whackAMoleManager.FreeSpawnPoint(currentSpawnPoint);
		}

	}

}