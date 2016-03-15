using System.Collections.Generic;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	[RequireComponent(typeof(Collider))]
	public abstract class TargetArea : MonoBehaviour {

		/// <summary>
		/// What tag an object has to have to be recognised
		/// </summary>
		public Tags.TagType TargetTagType;

		/// <summary>
		/// The actual tag as string
		/// </summary>
		protected string TargetTag;

		/// <summary>
		/// The Collider of this area
		/// </summary>
		protected Collider AreaCollider {
			get {
				return mAreaCollider ?? (mAreaCollider = this.TryGetComponent<Collider>());
			}
		}
		private Collider mAreaCollider;

		/// <summary>
		/// All valid objects that are inside the area
		/// </summary>
		protected HashSet<GameObject> Objects = new HashSet<GameObject>();

		protected virtual void Awake() {
			TargetTag = Tags.GetTagString(TargetTagType);
		}

		/// <summary>
		/// Called when a Collider enters the Trigger
		/// </summary>
		protected virtual void OnTriggerEnter(Collider other) {
			HandleTriggerEntered(other);
		}

		/// <summary>
		/// Called when a Collider exits the Trigger
		/// </summary>
		protected virtual void OnTriggerExit(Collider other) {
			HandleTriggerExit(other);
		}

		/// <summary>
		/// Called when an object exits the trigger
		/// </summary>
		protected virtual void HandleTriggerExit(Collider other) {
			Objects.Remove(other.gameObject);
		}

		/// <summary>
		/// Called when any object enters the trigger
		/// </summary>
		protected virtual void HandleTriggerEntered(Collider other) {
			if (!other.gameObject.CompareTag(TargetTag))
				return;
			Objects.Add(other.gameObject);
			HandleValidOther(other);
		}

		/// <summary>
		/// Called if the other object, which entered the trigger, has the correct tag
		/// </summary>
		protected abstract void HandleValidOther(Collider other);

	}

}