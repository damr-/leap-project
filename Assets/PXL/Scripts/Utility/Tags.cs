using System.Linq;
using System.Collections.Generic;

namespace PXL.Utility {

	/// <summary>
	/// This class handles the available tags in the application and provides functions to retreive the string of a trag by it's type
	/// </summary>
	public static class Tags {

		/// <summary>
		/// The type of tag
		/// </summary>
		public enum TagType {
			Untagged = 0,
			Object = 1
		}

		/// <summary>
		/// Returns the string for the given <see cref="TagType"/>
		/// </summary>
		public static string GetTagString(TagType tagType) {
			return AvailableTags.First(t => t.Key == tagType).Value;
		}

		/// <summary>
		/// All existing tags as <see cref="TagType"/> and their name in Unity as <see cref="string"/>
		/// </summary>
		private static readonly Dictionary<TagType, string> AvailableTags = new Dictionary<TagType, string>() {
			{TagType.Untagged, "Untagged"},
			{TagType.Object, "Object"}
		};

	}

}