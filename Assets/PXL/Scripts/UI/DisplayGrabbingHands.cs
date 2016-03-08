using System.Linq;
using PXL.Interaction;
using System.Collections.Generic;

namespace PXL.UI {

	public class DisplayGrabbingHands : DisplayBase {

		private HashSet<string> grabbingHandNames = new HashSet<string>();

		private void Update() {
			text.text = "";
			grabbingHandNames.Clear();
            GrabbingHandsManager.GetGrabbingHands()
			   .Select(h => h.gameObject.name)
			   .ToList()
			   .ForEach(name => grabbingHandNames.Add(name));

			foreach (string name in grabbingHandNames)
				text.text += name + "\n";

			if (text.text.Length == 0)
				text.text = "<none>";
		}
	}

}