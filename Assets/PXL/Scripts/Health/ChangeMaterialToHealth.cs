using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Health {

	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(Health))]
	public class ChangeMaterialToHealth : MonoBehaviour {

		[Serializable]
		public class HealthMaterial {
			/// <summary>
			/// Below what amount of healthpoints this material becomes activated
			/// </summary>
			public float MinHealth;
			public Material Material;
		}

		public List<HealthMaterial> HealthMaterials = new List<HealthMaterial>();

		/// <summary>
		/// The <see cref="MeshRenderer"/> of this object
		/// </summary>
		private MeshRenderer MeshRenderer {
			get { return mMeshRenderer ?? (mMeshRenderer = this.TryGetComponent<MeshRenderer>()); }
		}
		private MeshRenderer mMeshRenderer;

		/// <summary>
		/// The <see cref="Health"/> of this object
		/// </summary>
		private Health Health {
			get { return mHealth ?? (mHealth = this.TryGetComponent<Health>()); }
		}
		private Health mHealth;

		private void OnEnable() {
			Health.CurrentHealth.Subscribe(HandleHealthChange);
		}

		/// <summary>
		/// Called when the amount of health points of this object change
		/// </summary>
		private void HandleHealthChange(float newHealth) {
			MeshRenderer.material = HealthMaterials.First(h => newHealth <= h.MinHealth).Material;
		}

	}

}