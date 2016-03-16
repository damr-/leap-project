using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Areas {

	public abstract class DestroyArea : ObjectArea {

		/// <summary>
		/// How many objects have to be destroyed to win. 
		/// -1 for infinite
		/// </summary>
		public int WinDestroyAmount = 1;

		/// <summary>
		/// How many win points this objective adds to the win condition. -1 to instantly win
		/// </summary>
		public int WorthWinPoints = 1;

		/// <summary>
		/// If wrong types are ignored or still destroyed
		/// </summary>
		[SerializeField]
		protected bool DestroyWrongTypes = true;

		/// <summary>
		/// If wrong types are punished by subtracting <see cref="PunishAmount"/> points
		/// </summary>
		[SerializeField]
		protected bool PunishWrongTypes;

		/// <summary>
		/// The amount of points subtracted when the object has the wrong type
		/// </summary>
		[SerializeField]
		protected int PunishAmount = 1;

		/// <summary>
		/// The current amount of objects that have been destroyed by this area
		/// </summary>
		public ObservableProperty<int> CurrentDestroyAmount = new ObservableProperty<int>(0);

		/// <summary>
		/// Invoked when <see cref="CurrentDestroyAmount"/> reaches <see cref="WinDestroyAmount"/>
		/// </summary>
		public IObservable<Unit> GoalReached { get { return GoalReachedSubject; } }
		protected readonly ISubject<Unit> GoalReachedSubject = new Subject<Unit>();

		/// <summary>
		/// Invoked when an object with the correct type enters
		/// </summary>
		public IObservable<ObjectBehaviour> ValidObject { get { return validObjectSubject; } }
		private readonly ISubject<ObjectBehaviour> validObjectSubject = new Subject<ObjectBehaviour>();

		/// <summary>
		/// Invoked when an object with the wrong type enters
		/// </summary>
		public IObservable<ObjectBehaviour> InvalidObject { get { return invalidObjectSubject; } }
		private readonly ISubject<ObjectBehaviour> invalidObjectSubject = new Subject<ObjectBehaviour>();

		/// <summary>
		/// Reset <see cref="CurrentDestroyAmount"/>
		/// </summary>
		protected override void Awake() {
			base.Awake();
			CurrentDestroyAmount.Value = 0;
		}

		/// <summary>
		/// Called when the necessary amount of objects has been destroyed
		/// If <see cref="WorthWinPoints"/> is -1, ends the game immediately
		/// </summary>
		protected virtual void HandleGameWon() {
			if (WorthWinPoints == -1) {
				GameMode.SetGameOver(true);
			}
			else {
				GameMode.AddPoints(WorthWinPoints);
			}
			SetAreaActive(false);
			GoalReachedSubject.OnNext(Unit.Default);
		}

		protected override void HandleValidObjectType(ObjectBehaviour objectBehaviour) {
			validObjectSubject.OnNext(objectBehaviour);
		}
		
		protected override void HandleInvalidObjectType(ObjectBehaviour objectBehaviour) {
			invalidObjectSubject.OnNext(objectBehaviour);
		}
	}

}
