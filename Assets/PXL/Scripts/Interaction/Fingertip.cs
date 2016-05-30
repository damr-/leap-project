using PXL.Utility;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

namespace PXL.Interaction {

	/// <summary>
	/// Class which provides an object the functionality with interacting with another object.
	/// The other object needs a <see cref="Touchable"/> as well as the <see cref="Tags.TagType.Object"/> tag to be interacted with
	/// </summary>
	public class Fingertip : MonoBehaviour {

		/// <summary>
		/// The type of tag this fingertip interacts with
		/// </summary>
		public Tags.TagType TargetTagType = Tags.TagType.Object;

		/// <summary>
		/// The actual tag as string
		/// </summary>
		private string targetTag;

		/// <summary>
		/// The hand this finger tip belongs to
		/// </summary>
		public HandModel HandModel { get; private set; }

		/// <summary>
		/// The Touchable this Fingertip is currently touching
		/// </summary>
		public Touchable Touchable;

		/// <summary>
		/// All known Colliders of grabbable Objects and their Grabbable component
		/// </summary>
		public static IDictionary<Collider, Touchable> Touchables = new Dictionary<Collider, Touchable>();

		private void Awake() {
			targetTag = Tags.GetTagString(TargetTagType);
		}

		private void Start() {
			HandModel = GetComponentInParent<HandModel>();
		}

		private void OnTriggerEnter(Collider other) {
			var otherObject = other.gameObject;
			if (!otherObject.CompareTag(targetTag))
				return;
			var touchable = Touchables.GetOrAdd(other);
			touchable.AddFinger(this);
			Touchable = touchable;
		}

		private void OnTriggerExit(Collider other) {
			var otherObject = other.gameObject;
			if (!otherObject.CompareTag(targetTag))
				return;
			var touchable = Touchables.GetOrAdd(other);
			touchable.RemoveFinger(this);
			Touchable = null;
		}

		private void OnDisable() {
			if (Touchable == null)
				return;
			Touchable.RemoveFinger(this);
			Touchable = null;
		}

	}

}