using System.Linq;
using PXL.Interaction;
using System.Collections.Generic;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

	/// <summary>
	/// This script displays the hands which are currently grabbing an object on the given <see cref="Text"/> component.
	/// </summary>
	public class DisplayTextGrabbingHands : MonoBehaviour {

		/// <summary>
		/// All the names of grabbing hands
		/// </summary>
		private readonly HashSet<string> grabbingHandNames = new HashSet<string>();

		/// <summary>
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		private void Update() {
			Text.text = "";
			grabbingHandNames.Clear();
			GrabbingHandsManager.GetGrabbingHands()
				.Select(h => h.gameObject.name)
				.ToList()
				.ForEach(s => grabbingHandNames.Add(s));

			foreach (var s in grabbingHandNames)
				Text.text += s + "\n";

			if (Text.text.Length == 0)
				Text.text = "<none>";
		}

	}

}