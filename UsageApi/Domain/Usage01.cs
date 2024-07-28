using Arch.Domain;

namespace UsageApi.Domain;

public class Usage01:AggregateRoot<int>
{
    public Usage01(string title,Usage01Type type)
    {
        Title = title;
        Deleted = false;
        Type=type;
    }

    public string Title { get; private set; }
    public Usage01Type Type { get; private set; }
    public void UpdateTitle(string title) => Title = title;
}
