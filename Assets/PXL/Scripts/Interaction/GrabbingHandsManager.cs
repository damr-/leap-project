using System.Collections.Generic;
using Leap.Unity;

namespace PXL.Interaction {

	public static class GrabbingHandsManager {

		/// <summary>
		/// All the hands currently grabbing an object
		/// </summary>
		private static readonly HashSet<HandModel> GrabbingHands = new HashSet<HandModel>();

		/// <summary>
		/// Returns whether the given hand is currently grabbing an object
		/// </summary>
		/// <param name="hand">The hand to check for</param>
		/// <returns>True if the given hand is not in the set</returns>
		public static bool CanHandGrab(HandModel hand) {
			return !GrabbingHands.Contains(hand);
		}

		/// <summary>
		/// Tries to set the given hand as actively grabbing and returns the result
		/// </summary>
		/// <param name="hand">The hand which will be set to grabbing</param>
		/// <returns>True if the operation was successful</returns>
		public static bool AddHand(HandModel hand) {
			return GrabbingHands.Add(hand);
		}

		/// <summary>
		/// Tries to set the given hand as currently NOT grabbing and returns the result
		/// </summary>
		/// <param name="hand">The hand which doesn't grab anything anymore</param>
		/// <returns>True if the operation was successful</returns>
		public static bool RemoveHand(HandModel hand) {
			return GrabbingHands.Remove(hand);
		}

		/// <summary>
		/// Returns the grabbing hands
		/// </summary>
		public static HashSet<HandModel> GetGrabbingHands() {
			return GrabbingHands;
		}

	}

}