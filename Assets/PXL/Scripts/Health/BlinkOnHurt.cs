using System;
using PXL.Utility;
using UnityEngine;
using UniRx;

namespace PXL.Health {

	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(Health))]
	public class BlinkOnHurt : MonoBehaviour {

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

		/// <summary>
		/// The material to apply when hurt
		/// </summary>
		public Material HurtMaterial;

		/// <summary>
		/// How long the hurtmaterial stays when this object is hurt
		/// </summary>
		[Range(0.01f, 10f)]
		public float BlinkDuration = 0.25f;

		/// <summary>
		/// Whether the material is currently blinking.
		/// </summary>
		private bool isBlinking;

		/// <summary>
		/// The default material of this object, stored every time before blinkng
		/// </summary>
		private Material defaultMaterial;

		/// <summary>
		/// The subscription for the blink timer
		/// </summary>
		private IDisposable blinkDisposable = Disposable.Empty;
		
		/// <summary>
		/// Sets up the Health subscription
		/// </summary>
		private void OnEnable() {
			HurtMaterial.AssertNotNull("Hurt material missing!");
			Health.Hurt.Subscribe(_ => HandleHurt());
		}

		/// <summary>
		/// Called when this object is hurt
		/// </summary>
		private void HandleHurt() {
			if(!isBlinking)
				defaultMaterial = MeshRenderer.material;
            MeshRenderer.material = HurtMaterial;
			isBlinking = true;

			blinkDisposable = Observable.Timer(TimeSpan.FromSeconds(BlinkDuration)).Subscribe(_ => {
				MeshRenderer.material = defaultMaterial;
				isBlinking = false;
			});
		}

		private void OnDisable() {
			blinkDisposable.Dispose();
        }

	}

}