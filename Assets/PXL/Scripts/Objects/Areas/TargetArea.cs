using System.Collections.Generic;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

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
		/// All valid objects that are inside the area
		/// </summary>
		protected HashSet<GameObject> Objects = new HashSet<GameObject>();

		protected virtual void Awake() {
			TargetTag = Tags.GetTagString(TargetTagType);
		}

		protected virtual void OnTriggerEnter(Collider other) {
			HandleTriggerEntered(other);
		}

		protected virtual void OnTriggerExit(Collider other) {
			if (Objects.Contains(other.gameObject)) {
				Objects.Remove(other.gameObject);
			}
		}

		protected virtual void HandleTriggerEntered(Collider other) {
			if (!other.gameObject.CompareTag(TargetTag))
				return;
			Objects.Add(other.gameObject);
			HandleValidOther(other);
		}

		/// <summary>
		/// Called if the other object has the correct tag
		/// </summary>
		protected abstract void HandleValidOther(Collider other);
	}

}