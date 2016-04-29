using System;
using System.Collections.Generic;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Interaction {

	[RequireComponent(typeof(Grabbable))]
	public class DeactivateGrabbableUnderneathOther : MonoBehaviour {

		/// <summary>
		/// The Grabbable Component of this object
		/// </summary>
		protected Grabbable Grabbable {
			get { return mGrabbable ?? (mGrabbable = this.TryGetComponent<Grabbable>()); }
		}
		private Grabbable mGrabbable;

		/// <summary>
		/// The Rigidbody Component of this object
		/// </summary>
		protected Rigidbody Rigidbody {
			get { return mRigidbody ?? (mRigidbody = this.TryGetComponent<Rigidbody>()); }
		}
		private Rigidbody mRigidbody;

		/// <summary>
		/// How many times per second the state of the grabbable should be updated
		/// </summary>
		private const float UpdateFrequency = 5f;

		/// <summary>
		/// The disposable for the update interval
		/// </summary>
		private IDisposable updateDisposable = Disposable.Empty;

		/// <summary>
		/// The tag for the objects which are not allowed above this object
		/// </summary>
		private string objectTag = "";

		/// <summary>
		/// Wether the ray should be drawn as debug ray
		/// </summary>
		public bool DrawDebugRay;

		/// <summary>
		/// Each through raycasts obtained collider and their corresponding grabbable component
		/// </summary>
		private readonly IDictionary<Collider, Grabbable> colliderGrabbables = new Dictionary<Collider, Grabbable>();

		/// <summary>
		/// The offsets of the cast rays
		/// </summary>
		private readonly List<Vector3> rayOffsets = new List<Vector3>() {
			new Vector3(0.03f, 0f, 0.03f),
			new Vector3(-0.03f, 0f, 0.03f),
			new Vector3(0.03f, 0f, -0.03f),
			new Vector3(-0.03f, 0f, -0.03f)
		};

		private void OnEnable() {
			if (Grabbable != null)
				Start();
		}

		private void Start() {
			objectTag = Tags.GetTagString(Tags.TagType.Object);
			updateDisposable.Dispose();
			updateDisposable = Observable.Interval(TimeSpan.FromSeconds(1 / UpdateFrequency)).Subscribe(_ => UpdateState());
			TrySetGrabbableState(true);
		}

		private void UpdateState() {
			if (Grabbable.IsGrabbed || !Grabbable.IsStationary(0.002f))
				return;

			foreach (var rayOffset in rayOffsets) {
				var ray = new Ray(transform.position + rayOffset, Vector3.up);
				if (DrawDebugRay)
					Debug.DrawRay(ray.origin, ray.direction.normalized * 0.2f, Color.red, 1 / UpdateFrequency);
				CastRay(ray);
			}
		}

		/// <summary>
		/// Casts the given ray and checks if it hits and object.
		/// According to the state of the hit object it determines, whether this object will be interactable or not
		/// </summary>
		private void CastRay(Ray ray) {
			RaycastHit hit;

			if (!Physics.Raycast(ray, out hit, 0.2f)) {
				TrySetGrabbableState(true);
				return;
			}

			var obj = hit.collider.gameObject;
			if (obj == gameObject)
				return;

			var grabbable = colliderGrabbables.GetOrAdd(hit.collider, true);
			if (grabbable == null)
				return;

			if (obj.CompareTag(objectTag) && grabbable.IsStationary(0.005f))
				TrySetGrabbableState(false);
			else
				TrySetGrabbableState(true);
		}

		/// <summary>
		/// Sets the state of this object to the given grabbable state, if it's not the same
		/// </summary>
		private void TrySetGrabbableState(bool newGrabbableState) {
			if (Grabbable.CanBeGrabbed == !newGrabbableState)
				Grabbable.CanBeGrabbed = newGrabbableState;

			Rigidbody.isKinematic = !newGrabbableState;
			Rigidbody.constraints = newGrabbableState ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
			Rigidbody.mass = newGrabbableState ? 0.25f : 10000f;
			Rigidbody.drag = newGrabbableState ? 0f : 10000f;
		}

		/// <summary>
		/// Disposes the update disposable
		/// </summary>
		private void OnDisable() {
			TrySetGrabbableState(true);
			updateDisposable.Dispose();
		}

	}

}