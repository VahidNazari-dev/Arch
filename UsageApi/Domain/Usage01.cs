using Arch.Domian;

namespace UsageApi.Domain;

public class Usage01:AggregateRoot<int>
{
    public Usage01(string title)
    {
        Title = title;
        Deleted = false;
    }

    public string Title { get; private set; }
    public void UpdateTitle(string title) => Title = title;
}
