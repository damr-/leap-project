using UnityEngine;
using Leap;

namespace PXL.Utility {

	public class ConnectionNotice : MonoBehaviour {

		private UnityEngine.UI.Image m_image = null;
		private UnityEngine.UI.Image image {
			get {
				if (m_image == null)
					m_image = this.TryGetComponent<UnityEngine.UI.Image>();
				return m_image;
			}
		}

		/** The speed to fade the object alpha from 0 to 1. */
		public float fadeInTime = 1.0f;
		/** The speed to fade the object alpha from 1 to 0. */
		public float fadeOutTime = 1.0f;

		public AnimationCurve fadeCurve;

		/** A delay before beginning the fade-in effect. */
		public int waitFrames = 10;

		/** The fully on texture tint color. */
		public Color onColor = Color.white;

		private Controller leap_controller_;
		private float fadedIn = 0.0f;
		private int frames_disconnected_ = 0;

		void Start() {
			leap_controller_ = new Controller();
			SetAlpha(0.0f);
		}

		void SetAlpha(float alpha) {
			image.color = Color.Lerp(Color.clear, onColor, alpha);
		}

		/** The connection state of the controller. */
		bool IsConnected() {
			Debug.Log(leap_controller_.IsConnected);
			return leap_controller_.IsConnected;
		}

		/** Whether the controller is embedded in a keyboard or laptop.*/
		bool IsEmbedded() {
			DeviceList devices = leap_controller_.Devices;
			if (devices.Count == 0)
				return false;
			return devices[0].IsEmbedded;
		}

		void Update() {

			if (IsConnected())
				frames_disconnected_ = 0;
			else
				frames_disconnected_++;

			if (frames_disconnected_ < waitFrames)
				fadedIn -= Time.deltaTime / fadeOutTime;
			else
				fadedIn += Time.deltaTime / fadeInTime;
			fadedIn = Mathf.Clamp(fadedIn, 0.0f, 1.0f);

			SetAlpha(fadeCurve.Evaluate(fadedIn));
		}
	}

}