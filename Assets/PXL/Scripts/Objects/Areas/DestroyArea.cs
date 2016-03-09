using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Areas {

	public class DestroyArea : ObjectArea {
		protected override void HandleValidObjectType(ObjectBehaviour objectBehaviour) {
			objectBehaviour.DestroyObject();
		}
	}

}
