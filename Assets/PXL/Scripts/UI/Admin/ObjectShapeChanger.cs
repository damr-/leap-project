using System.Linq;
using PXL.Objects.Spawner;

namespace PXL.UI.Admin {

	/// <summary>
	/// An <see cref="IndexedPropertyChanger"/> which cycles through the available shapes of an <see cref="ObjectSpawner"/>
	/// </summary>
	public class ObjectShapeChanger : IndexedPropertyChanger {

		/// <summary>
		/// Optional preview image reference
		/// </summary>
		public ObjectShapePreview ObjectShapePreview;

		/// <summary>
		/// The <see cref="SetObjectShapeOnSpawn"/> component of the referenced ObjectSpawner
		/// </summary>
		private SetObjectShapeOnSpawn SetObjectShapeOnSpawn {
			get { return ObjectSpawner.GetComponent<SetObjectShapeOnSpawn>(); }
		}

		/// <summary>
		/// Sets up the ShapeChanger with the given objects spawner
		/// </summary>
		public override void SetObjectSpawner(ObjectSpawner objectSpawner) {
			base.SetObjectSpawner(objectSpawner);

			if (ObjectShapePreview != null)
				ObjectShapePreview.Setup(ObjectSpawner);
			ChangeProperty(SetObjectShapeOnSpawn.AvailableShapes.IndexOf(SetObjectShapeOnSpawn.CurrentObjectShape));
		}

		/// <summary>
		/// Changes the current property to the one with the given index
		/// </summary>
		protected override void ChangeProperty(int index) {
			if (!IsValidIndex(index))
				return;

			var newShape = SetObjectShapeOnSpawn.AvailableShapes.ElementAt(index);

			SetObjectShapeOnSpawn.SetObjectShape(newShape);
			if (PropertyText != null)
				PropertyText.text = newShape.Name;
			CurrentPropertyIndex = index;
		}

		/// <summary>
		/// Returns whether the index is within 0 and the length of AvailableShapes
		/// </summary>
		protected override bool IsValidIndex(int index) {
			return index >= 0 && index < SetObjectShapeOnSpawn.AvailableShapes.Count;
		}

	}

}
