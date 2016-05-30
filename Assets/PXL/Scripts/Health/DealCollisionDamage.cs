using PXL.Objects;
using PXL.Utility;
using UnityEngine;

namespace PXL.Health {

	/// <summary>
	/// This script makes the object deal <see cref="Damage"/> points to or immediately kill other ones, when they collide.
	/// The other objects need to have <see cref="InteractiveObject"/> and <see cref="Health"/> components.
	/// </summary>
	public class DealCollisionDamage : MonoBehaviour {

		/// <summary>
		/// The <see cref="Tags.TagType"/> the other object has to have for it to be interacted with
		/// </summary>
		public Tags.TagType TargetTag;

		/// <summary>
		/// The <see cref="ObjectType"/> the other object has to have for it to be interacted with
		/// </summary>
		public ObjectType ObjectType;

		/// <summary>
		/// Whether the other object should be instantly killed
		/// </summary>
		public bool InstantKill;

		/// <summary>
		/// The amount of damage that will be inflicted, if <see cref="InstantKill"/> is set to 'false'
		/// </summary>
		public float Damage = 1f;

		/// <summary>
		/// The target tag as an actual string
		/// </summary>
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

			var health = collision.transform.GetComponent<Health>();

			if (health == null)
				return;

			if (InstantKill)
				health.Kill();
			else 
				health.ApplyDamage(Damage);
		}

	}

}