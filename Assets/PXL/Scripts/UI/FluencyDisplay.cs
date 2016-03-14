using UniRx;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine.UI;

namespace PXL.UI {

	[RequireComponent(typeof(RawImage))]
	public class FluencyDisplay : MonoBehaviour {

		/// <summary>
		/// The FluencyObserver of this scene
		/// </summary>
		public FluencyObserver FluencyObserver;

		/// <summary>
		/// The Image where the data will be drawn
		/// </summary>
		protected RawImage RawImage { get { return mRawImage ?? (mRawImage = this.TryGetComponent<RawImage>()); } }
		private RawImage mRawImage;

		/// <summary>
		/// Sets up the subscription
		/// </summary>
		private void Start() {
			FluencyObserver.FinishedObserving.Subscribe(HandleObjectDropped);
		}

		/// <summary>
		/// Called when an object has been dropped and the fluency should be displayed
		/// </summary>
		private void HandleObjectDropped(TrackData trackData) {
			var texture = CreateTexture();//Color.black);

			FillTexture(texture, trackData);

			RawImage.texture = texture;
		}

		/// <summary>
		/// Creates a texture of the given size with black background
		/// </summary>
		private Texture2D CreateTexture(/*Color backGroundColor*/) {
			const int width = 2048;
			const int height = 1000;
            var texture = new Texture2D(width, height);
			//for (var y = 0; y < texture.height; y++) {
			//	for (var x = 0; x < texture.width; x++) {
			//		texture.SetPixel(x, y, backGroundColor);
			//	}
			//}
			//texture.Apply();
			return texture;
		}

		/// <summary>
		/// Fills the texture with the given <see cref="TrackData"/>
		/// </summary>
		private void FillTexture(Texture2D texture, TrackData trackData) {
			var speedData = trackData.Speed;
			var timeData = trackData.Time;

			var startTime = timeData.First();
			var stopTime = timeData.Last();

			var maxSpeed = speedData.Max();

			var width = texture.width;
			var height = texture.height;

			var dataIndex = 0;

			var lastPositionPos = 0;
			var lastTimePos = 0;

			for (var i = 0; i < width; i++) {
				var timePos = (int)timeData.ElementAt(dataIndex).Remap(startTime, stopTime, 0, width);

				//Set pixel at fixed position
				if (i == timePos) {
					var positionPos = (int)speedData.ElementAt(dataIndex).Remap(0, maxSpeed, 0, height);
					texture.SetPixel(timePos, positionPos, Color.green);
					lastTimePos = timePos;
					lastPositionPos = positionPos;
					dataIndex++;
				}
				//interpolate between last and next fixed position
				else {
					var nextTimePos = (int)timeData.ElementAt(dataIndex).Remap(startTime, stopTime, 0, width);
					var nextPositionPos = (int)speedData.ElementAt(dataIndex).Remap(0, maxSpeed, 0, height);

					var currentPositionPos = (int)Mathf.Lerp(lastPositionPos, nextPositionPos, ((float)i).Remap(lastTimePos, nextTimePos, 0, 1));

					texture.SetPixel(i, currentPositionPos, Color.red);
				}
			}


			texture.Apply();
		}
	}

}