using UnityEngine;

namespace PXL.Utility {

	/// <summary>
	/// A proxy script to access the referenced particle system.
	/// </summary>
	public class ParticleEffectProxy : MonoBehaviour {

		public ParticleSystem ParticleSystem;

		private void Start() {
			ParticleSystem.AssertNotNull("Missing ParticleSystem reference!");
		}

		public void PlayParticleEffect() {
			ParticleSystem.Play();
		}

	}

}