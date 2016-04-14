using System.Collections.Generic;
using Leap.Unity;
using PXL.Objects;
using PXL.Utility;
using UnityEngine;

namespace PXL.Interaction {

	public class ObjectDestroyLaser : MonoBehaviour {

		public List<Material> AvailableMaterials = new List<Material>();

		public bool Enabled;

		private LineRenderer lineRenderer;

		private HandModel correspondingHand;

		private void Start() {
			correspondingHand = GetComponentInParent<HandModel>();
			correspondingHand.AssertNotNull("ObjectDestroyLaser must be child of a hand!");
			lineRenderer = GetComponentInChildren<LineRenderer>();
			ApplyRandomMaterial();
		}

		public void ToggleLaser() {
			Enabled = !Enabled;
		}

		public void EnableLaser(bool newEnabledState) {
			Enabled = newEnabledState;
		}

		/// <summary>
		/// Applies a random material from <see cref="AvailableMaterials"/> to the <see cref="lineRenderer"/>
		/// </summary>
		private void ApplyRandomMaterial() {
			var index = Random.Range(0, AvailableMaterials.Count);
			lineRenderer.material = AvailableMaterials[index];
		}

		private void Update() {
			if (!Enabled) {
				if (lineRenderer.enabled)
					lineRenderer.enabled = false;
				return;
			}

			if (!lineRenderer.enabled) {
				ApplyRandomMaterial();
				lineRenderer.enabled = true;
			}

			if (!correspondingHand.isActiveAndEnabled)
				return;

			var leapHand = correspondingHand.GetLeapHand();

			if (leapHand == null)
				return;

			var direction = leapHand.Direction.ToVector3().normalized;

			lineRenderer.enabled = true;
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, transform.position + direction * 100f);

			var ray = new Ray(transform.position, direction);
			RaycastHit hit;
			if (!Physics.Raycast(ray, out hit, 100f))
				return;

			var interactiveObject = hit.transform.GetComponent<InteractiveObject>();

			if (interactiveObject == null)
				return;

			interactiveObject.Kill();
		}

	}

}