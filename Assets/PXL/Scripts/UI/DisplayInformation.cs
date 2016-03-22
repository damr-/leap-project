using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Interaction;
using PXL.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI {

	public class DisplayInformation : InteractionHandSubscriber {
	
		/// <summary>
		/// The Text components of the picks labels for left and right hand
		/// </summary>
		public Text[] PicksTexts;

		/// <summary>
		/// The Text components of the drops labels for left and right hand
		/// </summary>
		public Text[] DropsTexts;

		/// <summary>
		/// The Text components of the distance labels for left and right hand
		/// </summary>
		public Text[] DistanceTexts;

		/// <summary>
		/// The referenced display for the time
		/// </summary>
		public DisplayTime DisplayTime;
		
		/// <summary>
		/// The distance each hand has carried an object
		/// </summary>
		private readonly float[] distances = new float[2];

		protected override void Start() {
			base.Start();

			for (var i = 0; i < 2; i++) {
				PicksTexts[i].AssertNotNull();
				DropsTexts[i].AssertNotNull();
				DistanceTexts[i].AssertNotNull();
			}
			DisplayTime.AssertNotNull("DisplayTime reference missing!");
		}

		protected override void HandleGrabbed(Grabbable grabbable) {
			IncrementTextValue(grabbable, PicksTexts);
			DisplayTime.TryStartTimer();
		}

		protected override void HandleDropped(Grabbable grabbable) {
			IncrementTextValue(grabbable, DropsTexts);
		}

		protected override void HandleMoved(MovementInfo movementInfo) {
			var side = GetHandSideIfValid(movementInfo.Moveable.Grabbable);
			if (side == HandSide.None)
				return;

			var index = (int) side - 1;
			distances[index] += movementInfo.Delta.magnitude;
			DistanceTexts[index].text = distances[index].ToString("0.000");
		}
		
		/// <summary>
		/// Returns the text of the given Text component as an Integer, if possible
		/// </summary>
		private int GetLabelTextAsNumber(Text text) {
			var result = -1;
			if (int.TryParse(text.text, out result))
				return result;
			return -1;
		}

		/// <summary>
		/// Increments the value of the Text component's text by one, if possible
		/// </summary>
		private void IncrementTextValue(Grabbable grabbable, IList<Text> possibleTexts) {
			var side = GetHandSideIfValid(grabbable);

			if (side == HandSide.None)
				return;

			var text = possibleTexts[(int)side-1];

			var value = GetLabelTextAsNumber(text);
			if (value != -1)
				text.text = (value + 1).ToString();
		}

	}
}