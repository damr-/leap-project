using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Areas {

	/// <summary>
	/// This script describes an area which usually destroyes the objects which enter it.
	/// It's made abstract so that more specific areas can define when and how exactly the objects are destroyed.
	/// It also keeps track of the current amount of destroyed objects and provides functionality 
	/// to win the game/add win points as soon as a certain amount has been destroyed.
	/// It can also recude the current win points of the gamemode by <see cref="PunishAmount"/>, 
	/// if the wrong object type is destroyed by this area.
	/// </summary>
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
		public IObservable<Unit> GoalReached { get { return goalReachedSubject; } }
		private readonly ISubject<Unit> goalReachedSubject = new Subject<Unit>();

		/// <summary>
		/// Invoked when an object with the correct type enters
		/// </summary>
		public IObservable<InteractiveObject> ValidObject { get { return validObjectSubject; } }
		private readonly ISubject<InteractiveObject> validObjectSubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Invoked when an object with the wrong type enters
		/// </summary>
		public IObservable<InteractiveObject> InvalidObject { get { return invalidObjectSubject; } }
		private readonly ISubject<InteractiveObject> invalidObjectSubject = new Subject<InteractiveObject>();

		/// <summary>
		/// Invoked when an object is about to be destroyed by this area
		/// </summary>
		public IObservable<InteractiveObject> ObjectDestroyed { get { return ObjectDestroyedSubject; } }

		protected readonly ISubject<InteractiveObject> ObjectDestroyedSubject = new Subject<InteractiveObject>();

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
				GameState.SetGameWon(true);
			}
			else {
				GameState.AddPoints(WorthWinPoints);
			}
			SetAreaActive(false);
			goalReachedSubject.OnNext(Unit.Default);
		}

		protected override void HandleValidObjectType(InteractiveObject interactiveObject) {
			validObjectSubject.OnNext(interactiveObject);
		}
		
		protected override void HandleInvalidObjectType(InteractiveObject interactiveObject) {
			invalidObjectSubject.OnNext(interactiveObject);
		}
	}

}
