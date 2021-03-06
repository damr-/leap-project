﻿using UniRx;
using System.Linq;
using UnityEngine;
using PXL.Interaction;
using PXL.Utility;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

	/// <summary>
	/// This script provides functionality to draw the fluency of a motion on a <see cref="RawImage"/> 
	/// as soon as the motion has finished and <see cref="TrackData"/> is provided.
	/// 
	/// It draws the individual data from the <see cref="TrackData"/> and interpolates linearly between these data sets. 
	/// </summary>
	[RequireComponent(typeof(RawImage))]
	public class DisplayFluency : MonoBehaviour {

		/// <summary>
		/// The FluencyObserver of this scene
		/// </summary>
		public FluencyObserver FluencyObserver;

		/// <summary>
		/// The Image where the data will be drawn
		/// </summary>
		protected RawImage RawImage {
			get { return mRawImage ?? (mRawImage = this.TryGetComponent<RawImage>()); }
		}
		private RawImage mRawImage;

		/// <summary>
		/// The Canvas Rect Transform this Display belongs to
		/// </summary>
		public RectTransform CanvasTransform;

		private void Start() {
			FluencyObserver.AssertNotNull("Missing FluencyObserver!");
			CanvasTransform.AssertNotNull("Missing Canvas RectTransform!");

			FluencyObserver.FinishedObserving.Subscribe(HandleObjectDropped);
		}

		/// <summary>
		/// Called when an object has been dropped and the fluency should be displayed
		/// </summary>
		private void HandleObjectDropped(TrackData trackData) {
			var texture = CreateTexture();
			FillTexture(texture, trackData);
			RawImage.texture = texture;
		}

		/// <summary>
		/// Creates a texture of the given size with black background
		/// </summary>
		private Texture2D CreateTexture() {
			var width = (int)CanvasTransform.rect.width;
			var height = (int)CanvasTransform.rect.height;
			var texture = new Texture2D(width, height);
			return texture;
		}

		/// <summary>
		/// Fills the <see cref="Texture2D"/> with the given <see cref="TrackData"/>
		/// </summary>
		private void FillTexture(Texture2D texture, TrackData trackData) {
			var speedData = trackData.Speed;
			var timeData = trackData.Time;

			var startTime = timeData.ElementAt(0);
			var stopTime = timeData.ElementAt(timeData.Count - 1);

			var maxSpeed = speedData.Max();

			var dataIndex = 0;

			var lastPositionPos = 0;
			var lastTimePos = 0;

			for (var i = 0; i < texture.width; i++) {
				var timePos = (int)timeData.ElementAt(dataIndex).Remap(startTime, stopTime, 0, texture.width);
				var positionPos = (int)speedData.ElementAt(dataIndex).Remap(0, maxSpeed, 0, texture.height);

				//Set pixel at fixed position
				if (i == timePos) {
					PaintPixelArea(texture, timePos, positionPos, Color.black);
					lastTimePos = timePos;
					lastPositionPos = positionPos;
					dataIndex++;
				}
				//interpolate between last and next fixed position
				else {
					var alpha = ((float)i).Remap(lastTimePos, timePos, 0, 1);
					var currentPositionPos = (int)Mathf.Lerp(lastPositionPos, positionPos, alpha);
					PaintPixelArea(texture, i, currentPositionPos, Color.red);
				}
			}

			texture.Apply();
		}

		/// <summary>
		/// Applies the color at the given position and its surrounding 8 pixels to the given texture
		/// </summary>
		private void PaintPixelArea(Texture2D texture, int x, int y, Color color) {
			for (var i = y - 1; i < y + 1; i++) {
				for (var h = x - 1; h < x + 1; h++) {
					texture.SetPixel(h, i, color);
				}
			}
		}

	}

}