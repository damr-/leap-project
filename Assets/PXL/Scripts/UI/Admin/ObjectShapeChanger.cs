using UnityEngine;
using System.Linq;
using System;
using UniRx;
using PXL.Utility;

namespace PXL.UI.Admin {

	public class ObjectShapeChanger : IndexedPropertyChanger {

		[Serializable]
		public struct ObjectShape {
			public GameObject Obj;
			public string Name;
			public Sprite Texture;
		}

		/// <summary>
		/// All selectable objects
		/// </summary>
		public ObjectShape[] AvailableObjects;

		/// <summary>
		/// Whether a random shape has to be chosen every time an object is spawned
		/// </summary>
		protected bool ChooseRandomShape;

		protected override void Start() {
			base.Start();
			ObjectSpawners.ForEach(o => o.SpawnInitiated.Subscribe(_ => RandomiseIfNeeded()));
		}

		protected override void ChangeProperty(int index) {
			if (!IsValidIndex(index))
				return;

			var newShape = AvailableObjects.ElementAt(index);

			ObjectSpawners.ForEach(o => o.SetObjectPrefab(newShape.Obj));
			Preview.overrideSprite = newShape.Texture;
			PropertyText.text = newShape.Name;

			ChooseRandomShape = index == 0;
			CurrentPropertyIndex = index;
		}

		protected override bool IsValidIndex(int index) {
			return index >= 0 && index < AvailableObjects.Length;
		}

		/// <summary>
		/// Change the currently used shape to a random one if necessary
		/// </summary>
		protected virtual void RandomiseIfNeeded() {
			if (ChooseRandomShape)
				ObjectSpawners.ForEach(o => o.SetObjectPrefab(GetRandomShape()));
		}

		/// <summary>
		/// Returns a random shape
		/// </summary>
		public GameObject GetRandomShape() {
			return AvailableObjects.GetRandomElement(1).Obj;
		}
	}

}
