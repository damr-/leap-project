using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	/// <summary>
	/// The base class for areas which reacts to objects with a certain tag and stores 
	/// all objects which have the correct tag and are currently inside the area.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public abstract class TargetArea : MonoBehaviour {

		/// <summary>
		/// What tag an object needsto be recognised by this area
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
			get { return mAreaCollider ?? (mAreaCollider = this.TryGetComponent<Collider>()); }
		}
		private Collider mAreaCollider;

		/// <summary>
		/// All objects that are inside the area with their overlapping colliders
		/// </summary>
		protected IDictionary<GameObject, HashSet<Collider>> Objects = new Dictionary<GameObject, HashSet<Collider>>();

		/// <summary>
		/// Whether this area is active
		/// </summary>
		protected bool AreaActive = true;

		protected virtual void Awake() {
			TargetTag = Tags.GetTagString(TargetTagType);
			SetAreaActive(true);
		}

		protected virtual void Update() {
			if (GameState.GameOver || !AreaActive)
				return;

			if (Objects.Any(o => o.Key == null || !o.Key.activeInHierarchy || o.Value.Count == 0))
				Objects =
					Objects.Where(e => e.Key != null && e.Key.activeInHierarchy && e.Value.Count > 0)
						.Distinct()
						.ToDictionary(e => e.Key, e => e.Value);
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
			var otherObject = other.gameObject;

			if (!Objects.ContainsKey(otherObject))
				return;

			Objects[otherObject].Remove(other);

			if (Objects[otherObject].Count == 0)
				Objects.Remove(otherObject);
		}

		/// <summary>
		/// Called when any object enters the trigger
		/// </summary>
		protected virtual void HandleTriggerEntered(Collider other) {
			if (other.isTrigger || !HasCorrectTag(other.gameObject))
				return;

			if (Objects.ContainsKey(other.gameObject))
				Objects[other.gameObject].Add(other);
			else
				Objects.Add(new KeyValuePair<GameObject, HashSet<Collider>>(other.gameObject, new HashSet<Collider> { other }));

			HandleValidOther(other);
		}

		/// <summary>
		/// Called if the other object, which entered the trigger, has the correct tag
		/// </summary>
		protected abstract void HandleValidOther(Collider other);

		/// <summary>
		/// Returns whether the other object has the correct tag
		/// </summary>
		protected bool HasCorrectTag(GameObject other) {
			return other.CompareTag(TargetTag);
		}

		/// <summary>
		/// Enables the trigger and sets the flag
		/// </summary>
		/// <param name="active">Whether this area is active</param>
		protected virtual void SetAreaActive(bool active) {
			AreaCollider.enabled = active;
			AreaActive = active;
		}
	}

}