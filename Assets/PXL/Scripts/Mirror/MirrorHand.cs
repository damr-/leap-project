using System.Linq;
using Leap.Unity;
using UnityEngine;

namespace PXL.Mirror {

	public class MirrorHand : MonoBehaviour {

		public CapsuleHand LeftCapsuleHand;

		public HandModel LeftHandModel;

		public CapsuleHand RightCapsuleHand;

		public HandModel RightHandModel;

		private void LateUpdate() {
			if (!LeftHandModel.gameObject.activeInHierarchy || !RightHandModel.gameObject.activeInHierarchy)
				return;

			foreach (var item in LeftHandModel.fingers.Select((value, index) => new { index, value })) {
				Mirror(item.value.gameObject, RightHandModel.fingers[item.index].gameObject);
			}
		}

		private void Mirror(GameObject origin, GameObject target) {
			var originPosition = origin.transform.position;
			var mirrorPos = transform.position;

			var diffX = Mathf.Abs(mirrorPos.x - originPosition.x);

			var newPos = new Vector3(mirrorPos.x + diffX, originPosition.y, originPosition.z);

			target.transform.position = newPos;
		}

		private bool TrySetObjectEnabled(GameObject go, bool newEnabledState) {
			if (go.activeInHierarchy == newEnabledState)
				return false;

			go.SetActive(newEnabledState);
			return true;
		}

	}

}