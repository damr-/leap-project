using System.Linq;
using PXL.Objects.Spawner;
using PXL.Utility;
using UnityEngine;

namespace PXL.UI.Admin {

	public class ObjectShapeChanger : IndexedPropertyChanger {
	
		/// <summary>
		/// Optional preview image reference
		/// </summary>
		public ObjectShapePreview ObjectShapePreview;

		private SetObjectShapeOnSpawn SetObjectShapeOnSpawn {
			get {
				if (mSetObjectShapeOnSpawn != null)
					return mSetObjectShapeOnSpawn;

				mSetObjectShapeOnSpawn = ObjectSpawner.GetComponent<SetObjectShapeOnSpawn>();
				if (mSetObjectShapeOnSpawn == null)
					throw new MissingReferenceException(ObjectSpawner.gameObject.name + " is missing a SetObjectShapeOnSpawn component!");

				return mSetObjectShapeOnSpawn;
			}
		}
		private SetObjectShapeOnSpawn mSetObjectShapeOnSpawn;

		protected override void Start() {
			base.Start();
			if(ObjectShapePreview != null)
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

		protected override bool IsValidIndex(int index) {
			return index >= 0 && index < SetObjectShapeOnSpawn.AvailableShapes.Count;
		}

	}

}
