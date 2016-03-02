using System.Linq;
using System.Collections.Generic;

public static class Tags {

	public enum TagType {
		Default = 0,
		CUBE = 1,
	}

	public static string getTagString(TagType tagType) {
		return tags.Where(t => t.type == tagType).FirstOrDefault().name;
	}

	private static List<Tag> tags = new List<Tag>() {
		new Tag (TagType.CUBE, "Cube")
	};

}

public struct Tag {
	public Tags.TagType type;
	public string name;

	public Tag(Tags.TagType type, string name) {
		this.type = type;
		this.name = name;
	}
}