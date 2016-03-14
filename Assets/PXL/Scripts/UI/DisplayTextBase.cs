using UnityEngine;
using UnityEngine.UI;
using PXL.Utility;

namespace PXL.UI {

	[RequireComponent(typeof(Text))]
	public class DisplayTextBase : MonoBehaviour {

		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text;

		protected virtual void Start() {
			Text = this.TryGetComponent<Text>();
		}

	}

}