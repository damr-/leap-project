using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UniRx;

namespace PXL.Interaction {

	public class FingerInfo {
		public Fingertip Fingertip;
		public HandModel HandModel;

		public FingerInfo(Fingertip fingertip, HandModel handModel) {
			Fingertip = fingertip;
			HandModel = handModel;
		}
	}

	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class Touchable : MonoBehaviour {
		/// <summary>
		/// The minimum fingers necessary to pick up an object
		/// </summary>
		public static int MinFingerCount = 3;

		/// <summary>
		/// Whether the thumb is actively touching the object
		/// </summary>
		private bool thumbTouches;

		/// <summary>
		/// All the fingers of all hands currently overlapping this object
		/// </summary>
		public readonly IDictionary<HandModel, HashSet<Fingertip>> HandFingers = new Dictionary<HandModel, HashSet<Fingertip>>();

		/// <summary>
		/// Observable for when a fingertip enters
		/// </summary>
		public IObservable<FingerInfo> FingerEntered { get { return fingerEnteredSubject; } }
		private readonly ISubject<FingerInfo> fingerEnteredSubject = new Subject<FingerInfo>();

		/// <summary>
		/// Observable for when a fingertip enters
		/// </summary>
		public IObservable<FingerInfo> FingerLeft { get { return fingerLeftSubject; } }
		private readonly ISubject<FingerInfo> fingerLeftSubject = new Subject<FingerInfo>();

		/// <summary>
		/// Called when a fingertip entered the trigger of an object
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void AddFinger(Fingertip fingertip) {
			var hand = fingertip.HandModel;
			
			if (HandFingers.ContainsKey(hand)) {
				HandFingers[hand].Add(fingertip);
			}
			else {
				HandFingers.Add(hand, new HashSet<Fingertip>() { fingertip });
			}

			fingerEnteredSubject.OnNext(new FingerInfo(fingertip, hand));
		}

		/// <summary>
		/// Called when a fingertip left the trigger of an object. 
		/// Just invokes <see cref="fingerLeftSubject"/> because <see cref="Grabbable"/> needs to check their conditions first.
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void RemoveFinger(Fingertip fingertip) {
			fingerLeftSubject.OnNext(new FingerInfo(fingertip, fingertip.HandModel));
		}

		/// <summary>
		/// Removes the fingertip if it's still registered and also removes the hand if all of it's fingers are gone
		/// </summary>
		/// <param name="fingerInfo"></param>
		public void CleanHandsDictionary(FingerInfo fingerInfo) {
			var fingertip = fingerInfo.Fingertip;
			var hand = fingerInfo.HandModel;

			if (HandFingers.ContainsKey(hand)) {
				HandFingers[hand].Remove(fingertip);

				if (HandFingers[hand].Count == 0) {
					HandFingers.Remove(hand);
				}
			}
		}

		/// <summary>
		/// Check if the thumb of the given hand is touching the object and updates the corresponding flag
		/// </summary>
		public void UpdateThumbTouches(HandModel hand) {
			thumbTouches = IsCertainFingerTouching(hand, Leap.Finger.FingerType.TYPE_THUMB);
		}

		/// <summary>
		/// Returns whether a certain fingertype of the given hand touches the object.
		/// </summary>
		/// <param name="hand"></param>
		/// <param name="fingerType">The type of the finger to check for</param>
		/// <returns>False if hand is null or the given finger is not touching the object</returns>
		public bool IsCertainFingerTouching(HandModel hand, Leap.Finger.FingerType fingerType) {
			if (hand == null || !HandFingers.ContainsKey(hand))
				return false;
			foreach (var ft in HandFingers[hand]) {
				var rigidFinger = ft.GetComponentInParent<RigidFinger>();

				if (rigidFinger == null)
					continue;

				var leapFinger = rigidFinger.GetLeapFinger();

				if (leapFinger != null && leapFinger.Type == fingerType)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns whether there are enough fingers touching the object and the thumb is one of them
		/// </summary>
		public bool CanGrabObject(HandModel hand) {
			return HandFingers.ContainsKey(hand) &&
				   HandFingers[hand].Count > MinFingerCount &&
				   thumbTouches;
		}

		/// <summary>
		/// Returns the average position of all fingers that touch the object
		/// </summary>
		public Vector3 GetAverageFingerPosition(HandModel hand) {
			var result = Vector3.zero;
			var fingers = HandFingers[hand];
			result = fingers.Aggregate(result, (current, tip) => current + tip.transform.position);
			return result / fingers.Count;
		}
	}

}