using PXL.Interaction;
using PXL.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PXL.Objects.WhackAMole {

	/// <summary>
	/// This script provides the functionality for an object ot behave like a mole in Whack A Mole!
	/// It moves to a free spawnpoint and plays the animation.
	/// After a random delay, the process starts again.
	/// If an object of a certain type enters the trigger of the mole, it kills itself and tells the
	/// WhackAMoleMAnager that it's been removed.
	/// </summary>
	public class Mole : MonoBehaviour {

		/// <summary>
		/// The possible states of the mole
		/// </summary>
		private enum MoleState {
			Animating = 0,
			ChangingPosition = 1
		}

		/// <summary>
		/// The object type which is needed to destroy the mole
		/// </summary>
		public ObjectType InteractiveObjectType = ObjectType.Weapon;

		/// <summary>
		/// The animation component of this object
		/// </summary>
		private Animation animComponent;

		/// <summary>
		/// The delay between changing positions
		/// </summary>
		private float changePositionDelay = 1f;

		/// <summary>
		/// The minimum possible position change delay
		/// </summary>
		private const float MinChangePositionDelay = 0.1f;

		/// <summary>
		/// The maximum possible position change delay
		/// </summary>
		private const float MaxChangePositionDelay = 3f;

		/// <summary>
		/// When the change of position has started
		/// </summary>
		private float changePositionStartTime;

		/// <summary>
		/// When playing the animation has started
		/// </summary>
		private float animationStartTime;

		/// <summary>
		/// The current state of the mole
		/// </summary>
		private MoleState state = MoleState.ChangingPosition;

		/// <summary>
		/// The WhackAMoleManager which created this mole
		/// </summary>
		private WhackAMoleManager whackAMoleManager;

		/// <summary>
		/// The Transform of the spawnpoint currently in use
		/// </summary>
		private Transform currentSpawnPoint;

		/// <summary>
		/// The collider of this mole
		/// </summary>
		private Collider MoleCollider {
			get { return mTrigger ?? (mTrigger = this.TryGetComponent<Collider>()); }
		}
		private Collider mTrigger;

		/// <summary>
		/// Sets the manager and retreives the animation component.
		/// Then calculates a new change position delay.
		/// </summary>
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