using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Objects.Spawner {

	/// <summary>
	/// This component sets the shape (mesh) of all the spawner's spawned objects.
	/// 
	/// If the mesh is null, a random one will be chosen every time an object is created.
	/// </summary>
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

			var defaultShape = AvailableShapes.FirstOrDefault(s => s.Object == ObjectSpawner.DefaultObjectPrefab);

			if (defaultShape.Object == null) {
				Debug.LogWarning("The Default object prefab of " + ObjectSpawner.name + " is not included in the AvailableShapes!");
				CurrentObjectShape.Value = AvailableShapes[0];
			}
			else {
				CurrentObjectShape.Value = defaultShape;
			}

			ObjectSpawner.SpawnInitiated.Subscribe(_ => RandomiseIfNeeded());
		}

		/// <summary>
		/// Returns a random shape
		/// </summary>
		public ObjectShape GetRandomShape() {
			return AvailableShapes.GetRandomElement(1);
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