using PXL.Objects;
using PXL.Utility;
using UnityEngine;

namespace PXL.Health {

	public class DealCollisionDamage : MonoBehaviour {

		/// <summary>
		/// The <see cref="Tags.TagType"/> the other object has to have for it to be interacted with
		/// </summary>
		public Tags.TagType TargetTag;

		/// <summary>
		/// The <see cref="ObjectType"/> the other object has to have for it to be interacted with
		/// </summary>
		public ObjectType ObjectType;

		private string targetTag;

		private void Start() {
			targetTag = Tags.GetTagString(TargetTag);
		}

		private void OnCollisionEnter(Collision collision) {
			if (!collision.gameObject.CompareTag(targetTag))
				return;

			var interactiveObject = collision.transform.GetComponent<InteractiveObject>();
			if (interactiveObject == null)
				return;

			if (ObjectType != ObjectType.All && ObjectType != interactiveObject.ObjectType)
				return;

			collision.transform.Kill();
		}

	}

}