using System.Collections.Generic;
using Leap.Unity;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	/// <summary>
	/// This class works as an abstract base for every script which needs to observe certain <see cref="InteractionHand"/> objects for their interactions (grab, drop, move).
	/// </summary>
	public abstract class InteractionHandSubscriber : MonoBehaviour {

		/// <summary>
		/// The referenced hands in the scene
		/// </summary>
		public List<HandModel> HandModels = new List<HandModel>();

		/// <summary>
		/// The observed interaction hands
		/// </summary>
		protected List<InteractionHand> InteractionHands = new List<InteractionHand>();

		/// <summary>
		/// All the individual subscriptions to the hands' interacitons
		/// </summary>
		private readonly CompositeDisposable subscriptions = new CompositeDisposable();

		protected virtual void Start() {
			HandModels.ForEach(i => {
				i.AssertNotNull("HandModel is missing!");
				InteractionHands.Add(i.GetComponent<InteractionHand>());
			});

			foreach (var hand in InteractionHands) {
				hand.ObjectGrabbed.Subscribe(HandleGrabbed).AddTo(subscriptions);
				hand.ObjectDropped.Subscribe(HandleDropped).AddTo(subscriptions);
				hand.ObjectMoved.Subscribe(HandleMoved).AddTo(subscriptions);
			}
		}

		/// <summary>
		/// Called when the grabbable is grabbed
		/// </summary>
		/// <param name="grabbable">The grabbable object</param>
		protected abstract void HandleGrabbed(Grabbable grabbable);

		/// <summary>
		/// Called when the grabbable is dropped
		/// </summary>
		/// <param name="grabbable">The grabbable object</param>
		protected abstract void HandleDropped(Grabbable grabbable);

		/// <summary>
		/// Called when the grabbable is moved
		/// </summary>
		protected abstract void HandleMoved(MovementInfo movementInfo);

		/// <summary>
		/// Returns the HandSide of the given grabbable's hand
		/// </summary>
		protected HandSide GetHandSideIfValid(Grabbable grabbable) {
			return InteractionHand.GetHandSide(grabbable.CurrentHand);
		}

		private void OnDisable() {
			subscriptions.Dispose();
		}

	}

}