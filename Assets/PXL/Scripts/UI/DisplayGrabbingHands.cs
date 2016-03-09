using System.Linq;
using PXL.Interaction;
using System.Collections.Generic;

namespace PXL.UI {

	public class DisplayGrabbingHands : DisplayBase {

		private readonly HashSet<string> grabbingHandNames = new HashSet<string>();

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