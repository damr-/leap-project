using UnityEngine;

namespace PXL.Utility {

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