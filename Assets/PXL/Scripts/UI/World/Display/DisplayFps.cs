using UnityEngine;

namespace PXL.UI.World.Display {

	public class DisplayFps : DisplayTextBase {

		/// <summary>
		/// How long to wait between updates
		/// </summary>
		public float UpdateFrequency = 0.5f;

		private float framesSum;
		private int counter;
		private float lastTime;

		private void Update() {
			framesSum += 1f / Time.deltaTime;
			counter++;

			if (lastTime + 1 / UpdateFrequency >= Time.time)
				return;

			Text.text = ((int)(framesSum / counter)).ToString();
			counter = 0;
			framesSum = 0;
			lastTime = Time.time;
		}

	}

}