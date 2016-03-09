using PXL.Utility;
using UnityEngine;

namespace PXL.Objects {

	public abstract class TargetArea : MonoBehaviour {
	
		/// <summary>
		/// What tag an object has to have to be recognised
		/// </summary>
		public Tags.TagType TargetTagType;
		
		/// <summary>
		/// The actual tag as string
		/// </summary>
		protected string TargetTag;
		
		protected virtual void Awake() {
			TargetTag = Tags.GetTagString(TargetTagType);
		}

		protected virtual void OnTriggerEnter(Collider other) {
			HandleTriggerEntered(other);
		}

		protected virtual void HandleTriggerEntered(Collider other) {
			if (other.gameObject.CompareTag(TargetTag)) {
				HandleValidOther(other);
			}
		}
		
		/// <summary>
		/// Called if the other object has the correct tag
		/// </summary>
		protected abstract void HandleValidOther(Collider other);
	}

}