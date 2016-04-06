using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

namespace PXL.Interaction {

	public class GrabbingHandsManager : MonoBehaviour {

		/// <summary>
		/// All the hands currently grabbing an object
		/// </summary>
		private static HashSet<HandModel> _grabbingHands;
		
		/// <summary>
		/// Returns whether the given hand is currently grabbing an object
		/// </summary>
		/// <param name="hand">The hand to check for</param>
		/// <returns>True if the given hand is not in the set</returns>
		public static bool CanHandGrab(HandModel hand) {
			return !_grabbingHands.Contains(hand);
		}

		/// <summary>
		/// Tries to set the given hand as actively grabbing and returns the result
		/// </summary>
		/// <param name="hand">The hand which will be set to grabbing</param>
		/// <returns>True if the operation was successful</returns>
		public static bool AddHand(HandModel hand) {
			return _grabbingHands.Add(hand);
		}

		/// <summary>
		/// Tries to set the given hand as currently NOT grabbing and returns the result
		/// </summary>
		/// <param name="hand">The hand which doesn't grab anything anymore</param>
		/// <returns>True if the operation was successful</returns>
		public static bool RemoveHand(HandModel hand) {
			return _grabbingHands.Remove(hand);
		}

		/// <summary>
		/// Returns the grabbing hands
		/// </summary>
		public static HashSet<HandModel> GetGrabbingHands() {
			return _grabbingHands;
		}
		
		private void Awake() {
			_grabbingHands = new HashSet<HandModel>();
		}

	}

}