using UnityEngine;
using Leap;

namespace PXL.Utility {

	public class ConnectionNotice : MonoBehaviour {

		private UnityEngine.UI.Image mImage;
		private UnityEngine.UI.Image Image {
			get { return mImage ?? (mImage = this.TryGetComponent<UnityEngine.UI.Image>()); }
		}

		/** The speed to fade the object alpha from 0 to 1. */
		public float FadeInTime = 1.0f;
		/** The speed to fade the object alpha from 1 to 0. */
		public float FadeOutTime = 1.0f;

		public AnimationCurve FadeCurve;

		/** A delay before beginning the fade-in effect. */
		public int WaitFrames = 10;

		/** The fully on texture tint color. */
		public Color OnColor = Color.white;

		private Controller leapController;
		private float fadedIn;
		private int framesDisconnected;

		private void Start() {
			leapController = new Controller();
			SetAlpha(0.0f);
		}

		private void SetAlpha(float alpha) {
			Image.color = Color.Lerp(Color.clear, OnColor, alpha);
		}

		/** The connection state of the controller. */

		private bool IsConnected() {
			Debug.Log(leapController.IsConnected);
			return leapController.IsConnected;
		}

		/** Whether the controller is embedded in a keyboard or laptop.*/

		private bool IsEmbedded() {
			var devices = leapController.Devices;
			return devices.Count != 0 && devices[0].IsEmbedded;
		}

		private void Update() {

			if (IsConnected())
				framesDisconnected = 0;
			else
				framesDisconnected++;

			if (framesDisconnected < WaitFrames)
				fadedIn -= Time.deltaTime / FadeOutTime;
			else
				fadedIn += Time.deltaTime / FadeInTime;
			fadedIn = Mathf.Clamp(fadedIn, 0.0f, 1.0f);

			SetAlpha(FadeCurve.Evaluate(fadedIn));
		}
	}

}