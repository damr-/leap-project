using PXL.Interaction;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Areas {

	public class RemoveIncorrectlyStackedObjects : MonoBehaviour {

		/// <summary>
		/// The StackArea of this object
		/// </summary>
		private StackArea StackArea {
			get { return mStackArea ?? (mStackArea = this.TryGetComponent<StackArea>()); }
		}

		private StackArea mStackArea;

		private void Start() {
			StackArea.AreaStatus.Subscribe(status => {
				if (status != StackArea.Status.StackedIcorrectly)
					return;

				var o = StackArea.IncorrectObject;
				//o.Kill();
				//o.GetComponent<Rigidbody>().AddForce(Vector3.up * 50f, ForceMode.Impulse);
				o.transform.position = o.GetComponent<Grabbable>().PickupPosition;
			});
		}

	}

}