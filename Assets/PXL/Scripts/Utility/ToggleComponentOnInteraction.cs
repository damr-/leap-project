using System.Collections.Generic;
using PXL.Interaction;
using UnityEngine;

namespace PXL.Utility {

	public class ToggleComponentOnInteraction : MonoBehaviour {

		public class ToggleSetting {
			public HandModel[] Hands;
			public Component[] Components;
			public bool Enable;

			private InteractionHand[] interactionHands;

			public ToggleSetting() {
				for (var i = 0; i < Hands.Length; i++) {
					interactionHands[i] = Hands[i].GetComponent<InteractionHand>();
				}
			}
		}

		public List<ToggleSetting> Settings = new List<ToggleSetting>();



	}

}