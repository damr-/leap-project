namespace PXL.Objects.Areas {

	public class BoundariesArea : ObjectArea {
	
		protected override void HandleValidObjectType(ObjectBehaviour objectBehaviour) {
			objectBehaviour.DestroyObject();
		}
	}

}