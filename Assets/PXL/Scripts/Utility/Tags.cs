using System.Linq;
using System.Collections.Generic;

namespace PXL.Utility {

	public static class Tags {

		/// <summary>
		/// The type of a tag
		/// </summary>
		public enum TagType {
			DEFAULT = 0,
			OBJECT = 1,
		}

		/// <summary>
		/// Returns the string for the given <see cref="TagType"/>
		/// </summary>
		public static string GetTagString(TagType tagType) {
			return tags.Where(t => t.Key == tagType).First().Value;
		}
		
		/// <summary>
		/// All existing tags as <see cref="Tags.TagType"/> and their name in Unity as <see cref="string"/>
		/// </summary>
		private static Dictionary<TagType, string> tags = new Dictionary<TagType, string>() {
		{ TagType.DEFAULT, "Default" },
		{ TagType.OBJECT, "Object" }
	};

	}

}