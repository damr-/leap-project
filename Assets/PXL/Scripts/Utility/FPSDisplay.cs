using UnityEngine;
using UnityEngine.UI;

namespace PXL.Utility {

	public class FpsDisplay : MonoBehaviour {

		/// <summary>
		/// How long to wait between updates
		/// </summary>
		public float UpdateDelay = 0.5f;

		/// <summary>
		/// The text component of the child
		/// </summary>
		public Text Text;

		/// <summary>
		/// If the FPS are currently visible
		/// </summary>
		private bool CounterEnabled { get { return Text.enabled; } }

		private float framesSum;
		private int counter;
		private float lastTime;

		private void Start() {
			Text.AssertNotNull("Missing Text reference!");
		}

		private void Update() {
			if (!CounterEnabled)
				return;

			framesSum += 1f / Time.deltaTime;
			counter++;

			if (lastTime + UpdateDelay >= Time.time)
				return;

			Text.text = (int)(framesSum / counter) + " fps";
			counter = 0;
			framesSum = 0;
			lastTime = Time.time;
		}
	}

}