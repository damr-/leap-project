using UnityEngine;
using UniRx;
using System.Collections.Generic;
using PXL.Utility;

namespace PXL.Interaction {

	public abstract class InteractionHandSubscriber : MonoBehaviour {

		/// <summary>
		/// The two hands in this scene
		/// </summary>
		public List<HandModel> HandModels = new List<HandModel>();

		protected List<InteractionHand> InteractionHands = new List<InteractionHand>();

		protected virtual void Start() {
			HandModels.ForEach(i => InteractionHands.Add(i.GetComponent<InteractionHand>()));

			foreach (var hand in InteractionHands) {
				hand.ObjectGrabbed.Subscribe(HandleGrabbed);
				hand.ObjectDropped.Subscribe(HandleDropped);
				hand.ObjectMoved.Subscribe(HandleMoved);
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
		/// Returns the index for accessing the arrays for the correct hand, -1 if the hand is invalid
		/// </summary>
		protected HandSide GetHandSideIfValid(Grabbable grabbable) {
			var hand = grabbable.CurrentHand;
			if (!hand.IsHandValid())
				return HandSide.None;
			return hand.GetLeapHand().IsLeft ? HandSide.Left : HandSide.Right;
		}

	}

}