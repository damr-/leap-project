using System.Collections.Generic;
using UnityEngine;
using PXL.Utility;
using UnityEngine.UI;
using System.Linq;

namespace PXL.Interaction {

	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class Grabbable : MonoBehaviour {
		/// <summary>
		/// Whether this Grabbable is currently grabbed or not
		/// </summary>
		public bool isGrabbed { get; set; }

		/// <summary>
		/// The minimum grab strength necessary to pick up an object
		/// </summary>
		private const float minGrabStrength = 0.25f;

		/// <summary>
		/// The minimum fingers necessary to pick up an object
		/// </summary>
		private const int minFingerCount = 3;

		/// <summary>
		/// Current hand in use
		/// </summary>
		private HandModel currentHand;

		/// <summary>
		/// All the fingertips currently within this object's trigger
		/// </summary>
		private HashSet<Fingertip> fingertips = new HashSet<Fingertip>();

		/// <summary>
		/// The rigidbody component of this object
		/// </summary>
		new private Rigidbody rigidbody;

		/// <summary>
		/// The Transform this object tracks when grabbed
		/// </summary>
		private Transform trackedTarget;

		/// <summary>
		/// All the delta positions from the last <see cref="keepDeltaPositionsFrames"/> frames
		/// </summary>
		//private List<Vector3> deltaPositions = new List<Vector3>();

		/// <summary>
		/// The position of the object last frame
		/// </summary>
		//private Vector3 lastPosition;

		/// <summary>
		/// Facetor which gets multiplied with the resulting motion for the rigidbody
		/// </summary>
		//private float motionStrength = 10f;

		/// <summary>
		/// How many frames of deltaPositions should be kept in store
		/// </summary>
		//private float keepDeltaPositionsFrames = 30;
		
		/// <summary>
		/// Whether the thumb is actively touching the object
		/// </summary>
		private bool thumbTouches = false;

		private void Start() {
			rigidbody = this.TryGetComponent<Rigidbody>();
		}

		private void Update() {
			if (currentHand != null && currentHand.isActiveAndEnabled && fingertips.Count > minFingerCount && thumbTouches && currentHand.GetLeapHand().GrabStrength >= minGrabStrength) {
				if (!isGrabbed) {
					TryGrab();
				}
				else {
					TrackTarget();
					//CalculateMotion();
				}
			}
			else {
				TryDrop();
			}
		}

		/// <summary>
		/// If <see cref="isGrabbed"/> is false, calls <see cref="Grab"/>
		/// </summary>
		private void TryGrab() {
			if (!isGrabbed)
				Grab();
		}

		/// <summary>
		/// Calls <see cref="SetGrabbed(bool)"/> to disable physics and sets the tracked target
		/// </summary>
		private void Grab() {
			SetGrabbed(true);
			//deltaPositions.Clear();
			trackedTarget = currentHand.palm;
			//Debug.Log("Grabbed! " + Time.time.ToString("0.0"));
		}

		/// <summary>
		/// If <see cref="isGrabbed"/>, call <see cref="Drop"/>
		/// </summary>
		public void TryDrop() {
			if (isGrabbed)
				Drop();
		}

		/// <summary>
		/// Removes the target and hand. Calls <see cref="SetGrabbed(bool)"/> to re-enable physics
		/// </summary>
		public void Drop() {
			SetGrabbed(false);
			trackedTarget = null;
			currentHand = null;
			//if (deltaPositions.Count > 0) {
			//	rigidbody.velocity = (transform.position - deltaPositions.ElementAt(0)) * motionStrength;
			//}
			//Debug.Log("Dropped! " + Time.time.ToString("0.0"));
		}

		/// <summary>
		/// Disables Physics and sets the flag
		/// </summary>
		/// <param name="grabbed"></param>
		private void SetGrabbed(bool grabbed) {
			rigidbody.useGravity = !grabbed;
			rigidbody.isKinematic = grabbed;
			isGrabbed = grabbed;
		}

		/// <summary>
		/// Updates the position and rotation of the object
		/// </summary>
		private void TrackTarget() {
			transform.position = CalculateAverageFingerPosition();
			transform.rotation = trackedTarget.rotation;
		}

		/// <summary>
		/// 
		/// </summary>
		//private void CalculateMotion() {			
			//deltaPositions.Add(transform.position - lastPosition);
			//if (deltaPositions.Count > keepDeltaPositionsFrames)
			//	deltaPositions.RemoveAt(0);
			//lastPosition = transform.position;
		//}

		/// <summary>
		/// Returns the average position of all fingers that touch the object
		/// </summary>
		private Vector3 CalculateAverageFingerPosition() {
			Vector3 result = Vector3.zero;
			foreach (Fingertip tip in fingertips) {
				result += tip.transform.position;
			}
			return result / fingertips.Count;
		}

		/// <summary>
		/// Checks whether the thumb is actively touching the object and sets the corresponding flag
		/// </summary>
		private void UpdateThumbTouches() {
			thumbTouches = fingertips.Any(ft => ft.GetComponentInParent<RigidFinger>().GetLeapFinger().Type == Leap.Finger.FingerType.TYPE_THUMB);
        }

		/// <summary>
		/// Drops the object when it's disabled
		/// </summary>
		private void OnDisable() {
			TryDrop();
		}

		/// <summary>
		/// Called when a fingertip entered the trigger of an object
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void FingerEntered(Fingertip fingertip) {
			if (currentHand != null && currentHand != fingertip.handModel)
				return;
			fingertips.Add(fingertip);
			UpdateThumbTouches();
            if (currentHand == null) {
				currentHand = fingertip.handModel;
			}
		}

		/// <summary>
		/// Called when a fingertip left the trigger of an object
		/// </summary>
		/// <param name="fingertip">Particular fingertip</param>
		public void FingerLeft(Fingertip fingertip) {
			fingertips.Remove(fingertip);
			UpdateThumbTouches();
			if (fingertips.Count == 0)
				currentHand = null;
		}
	}

}