using PXL.UI.Admin;
using UnityEngine;

namespace PXL.UI {

	/// <summary>
	/// This script needs to be present in every scene to set the initially active user mode
	/// </summary>
	public class SetInitialUserMode : MonoBehaviour {

		/// <summary>
		/// The initial Mode of this scene
		/// </summary>
		public UserMode InitialMode;

		private void Start() {
			AdminBase.Mode.Value = InitialMode;
		}

	}

}