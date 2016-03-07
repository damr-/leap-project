using UnityEngine;
using UnityEngine.UI;

namespace PXL.Utility {

	public class FPSDisplay : MonoBehaviour {
	
		/// <summary>
		/// The key to toggle the FPS display
		/// </summary>
		public KeyCode toggleKey = KeyCode.V;
		
		/// <summary>
		/// How long to wait between updates
		/// </summary>
		public float updateDelay = 0.5f;
		
		/// <summary>
		/// The text component of the child
		/// </summary>
		private Text text;
		
		/// <summary>
		/// The image component of this panel
		/// </summary>
		private Image image;
		
		/// <summary>
		/// If the FPS are currently visible
		/// </summary>
		private bool CounterEnabled { get { return image.enabled && text.enabled; } }

		private float framesSum = 0;
		private int counter = 0;
		private float lastTime = 0;

		private void Start() {
			text = GetComponentInChildren<Text>();
			image = this.TryGetComponent<Image>();
		}

		private void Update() {
			if (Input.GetKeyDown(toggleKey)) {
				text.enabled = !text.enabled;
				image.enabled = !image.enabled;
			}

			if (!CounterEnabled)
				return;

			framesSum += 1f / Time.deltaTime;
			counter++;

			if (lastTime + updateDelay < Time.time) {
				text.text = (int)(framesSum / counter) + " fps";
				counter = 0;
				framesSum = 0;
				lastTime = Time.time;
			}
		}
	}

}