using Arch.Domain;

namespace UsageApi.Domain;

public class Usage01Type: Enumeration
{
	public static Usage01Type Type01 = new Usage01Type(1, "One");
	public static Usage01Type Type02 = new Usage01Type(2, "Two");
	private Usage01Type(int id, string name):base(id,name)
	{

	}
}
