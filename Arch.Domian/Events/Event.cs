

namespace Arch.Domain;

public class Event
{
    private string _eventName;
    public virtual string EventName
    {
        get
        {
            if (string.IsNullOrEmpty(_eventName))
            {
                _eventName = GetType().FullName;
            }

            return _eventName;
        }
        set
        {
            _eventName = value;
        }
    }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int Version { get; set; }

    public object EventMetadata { get; set; }

    public virtual bool MustDispatch { get; set; }

   
}
