using PXL.Interaction;
using System.Linq;
using PXL.Gamemodes;
using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	public class DestroyArea : ObjectArea {

		/// <summary>
		/// How many objects have to be destroyed to win
		/// </summary>
		public int WinDestroyAmount = 1;

		/// <summary>
		/// The current amount of objects that have been destroyed by this area
		/// </summary>
		protected int CurrentDestroyAmount = 0;

		protected override void Awake() {
			base.Awake();
			CurrentDestroyAmount = 0;
		}

		protected virtual void Update() {
			if (GameMode.GameWon)
				return;

			foreach (var o in Objects.Where(o => o.activeInHierarchy)) {
				var grabbable = o.GetComponent<Grabbable>();
				var rigidbody = o.GetComponent<Rigidbody>();
				if (grabbable == null || grabbable.IsGrabbed ||
					rigidbody == null || !rigidbody.velocity.Equal(Vector3.zero))
					continue;

				if (++CurrentDestroyAmount >= WinDestroyAmount) {
					AreaCollider.enabled = false;
					GameMode.SetGameOver(true);
					o.GetComponent<ObjectBehaviour>().DestroyObject();
					break;
				}

				o.GetComponent<ObjectBehaviour>().DestroyObject();

			}
		}
	}

}
