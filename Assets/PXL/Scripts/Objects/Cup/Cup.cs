using System.Collections.Generic;
using System.Linq;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Cup {

	public class Cup : MonoBehaviour {

		/// <summary>
		/// The necessary object type for objects to be interacted with
		/// </summary>
		public ObjectType ObjectType;

		/// <summary>
		/// Maximum number of objects allowed snapped inside the cup
		/// </summary>
		public int MaxHoldAmount = 20;

		/// <summary>
		/// All interactive objects inside the cup's trigger, being carried around
		/// </summary>
		private List<InteractiveObject> objects = new List<InteractiveObject>();

		/// <summary>
		/// The Grabbable Component of the cup
		/// </summary>
		private Grabbable Grabbable {
			get { return mGrabbable ?? (mGrabbable = GetComponentInParent<Grabbable>()); }
		}
		private Grabbable mGrabbable;

		/// <summary>
		/// Whether the cup can hold objects and reacts to trigger events
		/// </summary>
		private bool canHoldObjects = true;

		private void Start() {
			Grabbable.IsGrabbed.Subscribe(grabbed => {
				if (grabbed)
					return;
				ReleaseObjects();
			});
		}

		private void Update() {
			Extensions.PurgeIfNecessary(ref objects);

			var rotX = transform.rotation.eulerAngles.x;
			var rotZ = transform.rotation.eulerAngles.z;

			if ((rotX > 90 && rotX < 270) || (rotZ > 90 && rotZ < 270)) {
				if (!canHoldObjects)
					return;
				//Debug.LogWarning("Too steep! Releasing objects!");
				ReleaseObjects();
				canHoldObjects = false;
				Debug.DrawLine(Vector3.zero, Vector3.forward + new Vector3(0.05f, 0f, 0f), Color.red, 5f);
			}
			else if (!canHoldObjects) {
				//Debug.LogWarning("I can pickup objects again!");
				Debug.DrawLine(Vector3.zero, Vector3.forward, Color.green, 5f);
				canHoldObjects = true;
			}
		}

		private void ReleaseObjects() {
			foreach (var interactiveObject in objects) {
				ReleaseObject(interactiveObject);
			}
			objects = new List<InteractiveObject>();
		}

		private void ReleaseObject(InteractiveObject interactiveObject) {
			interactiveObject.transform.parent = null;
			var objectRigidbody = interactiveObject.GetComponent<Rigidbody>();
			objectRigidbody.useGravity = true;
			objectRigidbody.isKinematic = false;
			objectRigidbody.constraints = RigidbodyConstraints.None;
			interactiveObject.GetComponents<Collider>().Where(c => !c.isTrigger).ToList().ForEach(c => c.enabled = true);
		}

		private void OnTriggerEnter(Collider other) {
			if (!canHoldObjects || objects.Count > MaxHoldAmount)
				return;

			var interactiveObject = other.GetComponent<InteractiveObject>();

			if (interactiveObject == null || (ObjectType != ObjectType.All && interactiveObject.ObjectType != ObjectType) || objects.Contains(interactiveObject))
				return;

			interactiveObject.transform.SetParent(transform, true);
			var objectRigidbody = interactiveObject.GetComponent<Rigidbody>();
			objectRigidbody.useGravity = false;
			objectRigidbody.isKinematic = true;
			objectRigidbody.velocity = Vector3.zero;
			objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
			interactiveObject.GetComponents<Collider>().Where(c => !c.isTrigger).ToList().ForEach(c => c.enabled = false);
			interactiveObject.transform.rotation = Quaternion.identity;

			var index = objects.Count;
			var y = -0.4f + 0.2f * (index / 4);

			var x = 0f;
			var z = 0f;

			switch (index % 4) {
				case 0:
					x = 0.1f;
					z = 0.1f;
					break;
				case 1:
					x = -0.1f;
					z = 0.1f;
					break;
				case 2:
					x = 0.1f;
					z = -0.1f;
					break;
				case 3:
					x = -0.1f;
					z = -0.1f;
					break;
			}

			interactiveObject.transform.localPosition = new Vector3(x, y, z);

			objects.Add(interactiveObject);
		}
		
		private void OnTriggerExit(Collider other) {
			var interactiveObject = other.GetComponent<InteractiveObject>();

			if (interactiveObject == null)
				return;

			if (!objects.Contains(interactiveObject))
				return;

			ReleaseObject(interactiveObject);
		}

	}

}