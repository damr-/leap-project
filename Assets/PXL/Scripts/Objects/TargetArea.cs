using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public abstract class TargetArea : MonoBehaviour {
	
		/// <summary>
		/// What tag an object has to have to be recognised
		/// </summary>
		public Tags.TagType targetTagType;
		
		/// <summary>
		/// The actual tag as string
		/// </summary>
		protected string targetTag;
		
		protected virtual void Awake() {
			targetTag = Tags.getTagString(targetTagType);
		}

		protected virtual void OnTriggerEnter(Collider other) {
			HandleTriggerEntered(other);
		}

		protected virtual void HandleTriggerEntered(Collider other) {
			if (other.gameObject.CompareTag(targetTag)) {
				HandleValidOther(other);
			}
		}
		
		/// <summary>
		/// Called if the other object has the correct tag
		/// </summary>
		protected abstract void HandleValidOther(Collider other);
	}

}