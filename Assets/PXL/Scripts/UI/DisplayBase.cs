using UnityEngine;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI {

	[RequireComponent(typeof(Text))]
	public class DisplayBase : MonoBehaviour {

		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text text;

		protected virtual void Start() {
			text = this.TryGetComponent<Text>();
		}

	}

}