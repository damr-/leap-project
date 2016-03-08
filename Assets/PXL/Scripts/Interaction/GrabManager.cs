using System.Collections.Generic;

namespace PXL.Interaction {

	public static class GrabManager {

		/// <summary>
		/// All the hands currently grabbing an object
		/// </summary>
		private static HashSet<HandModel> grabbingHands = new HashSet<HandModel>();

		/// <summary>
		/// Returns whether the given hand is currently grabbing an object
		/// </summary>
		/// <param name="hand">The hand to check for</param>
		/// <returns>True if the given hand is not in the set</returns>
		public static bool CanHandGrab(HandModel hand) {
			return !grabbingHands.Contains(hand);
		}

		/// <summary>
		/// Tries to set the given hand as actively grabbing and returns the result
		/// </summary>
		/// <param name="hand">The hand which will be set to grabbing</param>
		/// <returns>True if the operation was successful</returns>
		public static bool AddHand(HandModel hand) {
			return grabbingHands.Add(hand);
		}

		/// <summary>
		/// Tries to set the given hand as currently NOT grabbing and returns the result
		/// </summary>
		/// <param name="hand">The hand which doesn't grab anything anymore</param>
		/// <returns>True if the operation was successful</returns>
		public static bool RemoveHand(HandModel hand) {
			return grabbingHands.Remove(hand);
		}

		/// <summary>
		/// Returns the grabbing hands
		/// </summary>
		public static HashSet<HandModel> GetGrabbingHands() {
			return grabbingHands;
		}

	}

}