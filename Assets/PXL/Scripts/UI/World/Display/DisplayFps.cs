using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

	/// <summary>
	/// This script displays the current frames per second on the object's <see cref="Text"/> component every frame
	/// </summary>
	public class DisplayFps : MonoBehaviour {

		/// <summary>
		/// How long to wait between updates
		/// </summary>
		public float UpdateFrequency = 0.5f;

		/// <summary>
		/// The sum of frames per seconds since the last update
		/// </summary> 
		private float framesPerSecondSum;

		/// <summary>
		/// The amount of frames since the last update
		/// </summary>
		private int framesCount;

		/// <summary>
		/// The last time the frames per seconds have been calculated
		/// </summary>
		private float lastTime;
		
		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		private void Update() {
			framesPerSecondSum += 1f / Time.deltaTime;
			framesCount++;

			if (lastTime + 1 / UpdateFrequency >= Time.time)
				return;

			Text.text = (int)(framesPerSecondSum / framesCount) + " fps";
			framesCount = 0;
			framesPerSecondSum = 0;
			lastTime = Time.time;
		}

	}

}