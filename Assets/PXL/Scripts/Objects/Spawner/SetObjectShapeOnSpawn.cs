using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Spawner {

	[RequireComponent(typeof(ObjectSpawner))]
	public class SetObjectShapeOnSpawn : MonoBehaviour {

		/// <summary>
		/// The ObjectSpawner of this object
		/// </summary>
		private ObjectSpawner ObjectSpawner {
			get { return mObjectSpawner ?? (mObjectSpawner = this.TryGetComponent<ObjectSpawner>()); }
		}
		private ObjectSpawner mObjectSpawner;

		[Serializable]
		public struct ObjectShape {
			public GameObject Object;
			public string Name;
			public Sprite Texture;

			public ObjectShape(string name, GameObject gameObject, Sprite texture) {
				Name = name;
				Object = gameObject;
				Texture = texture;
			}
		}

		/// <summary>
		/// The available shapes for objects spawned by this spawner
		/// </summary>
		public List<ObjectShape> AvailableShapes = new List<ObjectShape>();

		/// <summary>
		/// Whether a random shape has to be chosen every time an object is spawned
		/// </summary>
		protected bool ChooseRandomShape;

		/// <summary>
		/// The currently active prefab which will be spawned
		/// </summary>
		public ObservableProperty<ObjectShape> CurrentObjectShape = new ObservableProperty<ObjectShape>();

		private void Start() {
			if (AvailableShapes.Count == 0)
				throw new MissingReferenceException("No shapes added to the spawner!");

			CurrentObjectShape.Value = AvailableShapes.First(s => s.Object == ObjectSpawner.DefaultObjectPrefab);
			ObjectSpawner.SpawnInitiated.Subscribe(_ => RandomiseIfNeeded());
		}

		/// <summary>
		/// Returns a random shape
		/// </summary>
		public ObjectShape GetRandomShape() {
			return AvailableShapes.ToArray().GetRandomElement(1);
		}

		/// <summary>
		/// Sets the object shape which will be spawned
		/// </summary>
		public void SetObjectShape(ObjectShape newShape) {
			CurrentObjectShape.Value = newShape;
			ChooseRandomShape = newShape.Object == null;
			ObjectSpawner.SetObjectPrefab(newShape.Object);
		}

		/// <summary>
		/// Change the currently used shape to a random one if necessary
		/// </summary>
		protected virtual void RandomiseIfNeeded() {
			if (ChooseRandomShape)
				ObjectSpawner.SetObjectPrefab(GetRandomShape().Object);
		}

	}

}