using System.Linq;
using PXL.Objects.Spawner;

namespace PXL.UI.Admin {

	/// <summary>
	/// This script provides the functionality
	/// </summary>
	public class ObjectColorChanger : IndexedPropertyChanger {

		/// <summary>
		/// Optional preview for the color
		/// </summary>
		public ObjectColorPreview ObjectColorPreview;

		/// <summary>
		/// The <see cref="SetObjectColorOnSpawn"/> component of the referenced <see cref="ObjectSpawner"/>
		/// </summary>
		private SetObjectColorOnSpawn SetObjectColorOnSpawn {
			get { return ObjectSpawner.GetComponent<SetObjectColorOnSpawn>(); }
		}

		public override void SetObjectSpawner(ObjectSpawner objectSpawner) {
			base.SetObjectSpawner(objectSpawner);

			if (ObjectColorPreview != null)
				ObjectColorPreview.Setup(ObjectSpawner);
			ChangeProperty(SetObjectColorOnSpawn.AvailableColors.IndexOf(SetObjectColorOnSpawn.CurrentColor));
		}

		/// <summary>
		/// Changes the current property to the one with the given index
		/// </summary>
		protected override void ChangeProperty(int index) {
			if (!IsValidIndex(index))
				return;

			var newObjectColor = SetObjectColorOnSpawn.AvailableColors.ElementAt(index);

			SetObjectColorOnSpawn.SetColor(newObjectColor);

			if (PropertyText != null)
				PropertyText.text = newObjectColor.Name;

			CurrentPropertyIndex = index;
		}

		/// <summary>
		/// Returns whether the given index is a valid one for the available colors list
		/// </summary>
		protected override bool IsValidIndex(int index) {
			return index >= 0 && index < SetObjectColorOnSpawn.AvailableColors.Count;
		}

	}

}