using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity;
using PXL.Objects;
using PXL.UI.World;
using PXL.Utility;
using UnityEngine;

namespace PXL.Interaction {

	public class ObjectDestroyLaser : MonoBehaviour {

		private LineRenderer lineRenderer;

		public bool Enabled;

		private void Start() {
			lineRenderer = GetComponentInChildren<LineRenderer>();
		}

		private void Update() {

			if (!Enabled) {
				if (lineRenderer.enabled)
					lineRenderer.enabled = false;
				return;
			}

			if(!lineRenderer.enabled)
				lineRenderer.enabled = true;

			//if (!hand.gameObject.activeInHierarchy)
			//	return;

			//var leapHand = hand.GetLeapHand();

			//if (leapHand == null || leapHand.GrabStrength > 0)
			//	return;

			//var fingers = leapHand.Fingers;

			//Finger indexFinger = null;

			//foreach (var finger in fingers.Where(finger => finger.IsExtended)) {
			//	if (finger.Type == Finger.FingerType.TYPE_INDEX)
			//		indexFinger = finger;
			//	else
			//		return;
			//}

			//if (indexFinger == null)
			//	return;

			//var fingerPos = indexFinger.TipPosition.ToVector3();
			//var fingerDir = indexFinger.Direction.ToVector3();

			//lineRenderer.enabled = true;
			//lineRenderer.SetVertexCount(2);
			//lineRenderer.SetPosition(0, fingerPos);
			//lineRenderer.SetPosition(1, fingerPos + fingerDir * 1000f);

			//var ray = new Ray(fingerPos, fingerDir);
			//RaycastHit hit;
			//if (!Physics.Raycast(ray, out hit, 1000f))
			//	return;

			//var interactiveObject = hit.transform.GetComponent<InteractiveObject>();

			//if (interactiveObject == null)
			//	return;

			//interactiveObject.Kill();
		}

	}

}