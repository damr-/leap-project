using System.Linq;
using System.Collections.Generic;

public static class Tags {

	public enum TagType {
		DEFAULT = 0,
		OBJECT = 1,
	}

	public static string getTagString(TagType tagType) {
		return tags.Where(t => t.Key == tagType).FirstOrDefault().Value;
	}

	private static Dictionary<TagType, string> tags = new Dictionary<TagType, string>() {
		{ TagType.DEFAULT, "Default" },
        { TagType.OBJECT, "Object" }
	};

}