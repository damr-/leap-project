using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PXL.Utility;
using PXL.Interaction;
using System.Collections.Generic;

namespace PXL.UI {
	[RequireComponent(typeof(Text))]
	public class DisplayGrabbingHands : MonoBehaviour {
		private Text text;

		private void Start() {
			text = this.TryGetComponent<Text>();
		}

		private void Update() {
			text.text = "<none>";
			List<string> grabbingHandNames = GrabManager.GetGrabbingHands().Select(h => h.gameObject.name).ToList();
			foreach (string name in grabbingHandNames)
				text.text += name + "\n";
		}
	}

}