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
		public int MaxHoldAmount = 100;

		/// <summary>
		/// The maximum angle allowed before releasing all objects inside the cup
		/// </summary>
		public int MaxTiltAngle = 100;

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

		/// <summary>
		/// Sets up the subscription to release all objects as soon as the cup is dropped
		/// </summary>
		private void Start() {
			Grabbable.IsGrabbed.Subscribe(grabbed => {
				if (grabbed)
					return;
				ReleaseObjects();
			});
		}

		/// <summary>
		/// Checks if the cup is tilted too much so that the objects should be released
		/// </summary>
		private void Update() {
			Extensions.PurgeIfNecessary(ref objects);

			var rotX = transform.rotation.eulerAngles.x;
			var rotZ = transform.rotation.eulerAngles.z;

			if ((rotX > MaxTiltAngle && rotX < 360 - MaxTiltAngle) || (rotZ > MaxTiltAngle && rotZ < 360 - MaxTiltAngle)) {
				if (!canHoldObjects)
					return;
				ReleaseObjects();
				canHoldObjects = false;
			}
			else if (!canHoldObjects) {
				canHoldObjects = true;
			}
		}

		/// <summary>
		/// Releases all objects currently held by the cup
		/// </summary>
		private void ReleaseObjects() {
			foreach (var interactiveObject in objects) {
				SetObjectPickupState(interactiveObject, false);
			}
			objects = new List<InteractiveObject>();
		}

		/// <summary>
		/// Returns the position for the next object which will be added to the grid
		/// </summary>
		private Vector3 GetObjectPosition() {
			var index = objects.Count;
			var y = -0.4f + 0.1f * (index / 13);

			switch (index % 13) {
				case 0:
					return new Vector3(0.1f, y, 0.1f);
				case 1:
					return new Vector3(0f, y, 0.1f);
				case 2:
					return new Vector3(-0.1f, y, 0.1f);
				case 3:
					return new Vector3(0.1f, y, 0f);
				case 4:
					return new Vector3(0f, y, 0f);
				case 5:
					return new Vector3(-0.1f, y, 0f);
				case 6:
					return new Vector3(0.1f, y, -0.1f);
				case 7:
					return new Vector3(0f, y, -0.1f);
				case 8:
					return new Vector3(-0.1f, y, -0.1f);
				case 9:
					return new Vector3(0.2f, y, 0f);
				case 10:
					return new Vector3(-0.2f, y, 0f);
				case 11:
					return new Vector3(0f, y, 0.2f);
				default:
					return new Vector3(0f, y, -0.2f);
			}
		}

		/// <summary>
		/// Sets properties of the given <see cref="InteractiveObject"/> to be either pickedup by the cup or not
		/// </summary>
		private void SetObjectPickupState(InteractiveObject interactiveObject, bool pickedUp) {
			interactiveObject.transform.SetParent(pickedUp ? transform : null, true);
			interactiveObject.transform.rotation = Quaternion.identity;

			interactiveObject.GetComponents<Collider>().Where(c => !c.isTrigger).ToList().ForEach(c => c.enabled = !pickedUp);

			var objectRigidbody = interactiveObject.GetComponent<Rigidbody>();

			if (objectRigidbody == null)
				return;

			objectRigidbody.useGravity = !pickedUp;
			objectRigidbody.isKinematic = pickedUp;
			objectRigidbody.constraints = pickedUp ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
			objectRigidbody.velocity = Vector3.zero;
		}

		/// <summary>
		/// Called when the <see cref="Collider"/> 'other' enters thie object's Trigger
		/// </summary>
		private void OnTriggerEnter(Collider other) {
			if (!canHoldObjects || objects.Count > MaxHoldAmount)
				return;

			var interactiveObject = other.GetComponent<InteractiveObject>();

			if (interactiveObject == null || (ObjectType != ObjectType.All && interactiveObject.ObjectType != ObjectType) || objects.Contains(interactiveObject))
				return;

			SetObjectPickupState(interactiveObject, true);

			var pos = GetObjectPosition();
			interactiveObject.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

			objects.Add(interactiveObject);
		}

		/// <summary>
		/// Called when the <see cref="Collider"/> 'other' leaves thie object's Trigger
		/// </summary>
		private void OnTriggerExit(Collider other) {
			var interactiveObject = other.GetComponent<InteractiveObject>();

			if (interactiveObject == null)
				return;

			if (!objects.Contains(interactiveObject))
				return;

			SetObjectPickupState(interactiveObject, false);
		}

	}

}