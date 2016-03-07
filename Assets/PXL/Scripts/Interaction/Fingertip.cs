using PXL.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace PXL.Interaction {

	public class Fingertip : MonoBehaviour {
		/// <summary>
		/// The type of tag this fingertip interacts with
		/// </summary>
		private Tags.TagType targetTagType = Tags.TagType.OBJECT;

		/// <summary>
		/// The actual tag as string
		/// </summary>
		private string targetTag;

		/// <summary>
		/// The hand this finger tip's finger belongs to
		/// </summary>
		public HandModel handModel { get; private set; }

		public static IDictionary<Collider, Grabbable> grabbableObjects = new Dictionary<Collider, Grabbable>();

		private void Awake() {
			targetTag = Tags.GetTagString(targetTagType);
		}

		private void Start() {
			handModel = GetComponentInParent<HandModel>();
		}

		private void OnTriggerEnter(Collider other) {
			GameObject otherObject = other.gameObject;
			if (otherObject.CompareTag(targetTag)) {
				var grabbable = grabbableObjects.GetOrAdd(other);
				grabbable.FingerEntered(this);
			}
		}

		private void OnTriggerExit(Collider other) {
			GameObject otherObject = other.gameObject;
			if (otherObject.CompareTag(targetTag)) {
				var grabbable = grabbableObjects.GetOrAdd(other);
				grabbable.FingerLeft(this);
			}
		}

	}

}