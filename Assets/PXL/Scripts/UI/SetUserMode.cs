using PXL.UI.Admin;
using UnityEngine;

namespace PXL.UI {

	public class SetUserMode : MonoBehaviour {

		/// <summary>
		/// The initial Mode of this scene
		/// </summary>
		public UserMode InitialMode;

		private void Start() {
			AdminBase.Mode = InitialMode;
		}

	}

}