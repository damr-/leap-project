using System.Linq;
using PXL.Objects.Spawner;
using UnityEngine;

namespace PXL.UI.Admin {

	public class ObjectColorChanger : IndexedPropertyChanger {

		/// <summary>
		/// Optional preview for the color
		/// </summary>
		public ObjectColorPreview ObjectColorPreview;

		private SetObjectColorOnSpawn SetObjectColorOnSpawn {
			get {
				if (mSetObjectColorOnSpawn != null)
					return mSetObjectColorOnSpawn;

				mSetObjectColorOnSpawn = ObjectSpawner.GetComponent<SetObjectColorOnSpawn>();
				if (mSetObjectColorOnSpawn == null)
					throw new MissingReferenceException(ObjectSpawner.gameObject.name + " is missing a SetObjectColorOnSpawn component!");

				return mSetObjectColorOnSpawn;
			}
		}
		private SetObjectColorOnSpawn mSetObjectColorOnSpawn;

		protected override void Start() {
			base.Start();

			if(ObjectColorPreview != null)
				ObjectColorPreview.Setup(ObjectSpawner);
			ChangeProperty(SetObjectColorOnSpawn.AvailableColors.IndexOf(SetObjectColorOnSpawn.DefaultColor));
		}

		/// <summary>
		/// Changes the current property to the one with the given index
		/// </summary>
		protected override void ChangeProperty(int index) {
			if (!IsValidIndex(index))
				return;

			var newObjectColor = SetObjectColorOnSpawn.AvailableColors.ElementAt(index);

			SetObjectColorOnSpawn.SetColor(newObjectColor.Color);

			if (PropertyText != null)
				PropertyText.text = newObjectColor.Name;

			CurrentPropertyIndex = index;
		}

		protected override bool IsValidIndex(int index) {
			return index >= 0 && index < SetObjectColorOnSpawn.AvailableColors.Count;
		}
	}

}