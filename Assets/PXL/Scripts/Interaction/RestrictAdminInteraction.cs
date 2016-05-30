using System;
using UniRx;
using PXL.UI.Admin;
using PXL.Utility;
using UnityEngine;

namespace PXL.Interaction {

	/// <summary>
	/// This class makes an object only interactable when the admin mode is enabled.
	/// Otherwise, it will be static and not influenced by physics, pushing, grabbing and similar.
	/// </summary>
	public class RestrictAdminInteraction : MonoBehaviour {

		/// <summary>
		/// The Rigidbody component of this object
		/// </summary>
		private Rigidbody rbody;

		/// <summary>
		/// The Moveable component of this object
		/// </summary>
		private Moveable moveable;

		/// <summary>
		/// The Grabbable component of this object
		/// </summary>
		private Grabbable grabbable;

		/// <summary>
		/// The Touchable componnet of this object
		/// </summary>
		private Touchable touchable;

		/// <summary>
		/// The constraints for the Rigidbody in admin mode
		/// </summary>
		private const RigidbodyConstraints AdminConstraints =
			RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

		/// <summary>
		/// Disposable for the mode change observable property
		/// </summary>
		private IDisposable modeChangeDisposable = Disposable.Empty;

		private void Start() {
			rbody = this.TryGetComponent<Rigidbody>();
			moveable = this.TryGetComponent<Moveable>();
			grabbable = this.TryGetComponent<Grabbable>();
			touchable = this.TryGetComponent<Touchable>();

			modeChangeDisposable = AdminBase.Mode.Subscribe(HandleModeChanged);
			HandleModeChanged(AdminBase.Mode.Value);
		}

		private void HandleModeChanged(UserMode newMode) {
			moveable.enabled = AdminBase.IsAdmin;
			grabbable.enabled = AdminBase.IsAdmin;
			touchable.enabled = AdminBase.IsAdmin;

			rbody.constraints = AdminBase.IsAdmin ? AdminConstraints : RigidbodyConstraints.FreezeAll;
		}

		private void OnDisable() {
			modeChangeDisposable.Dispose();
		}

	}

}