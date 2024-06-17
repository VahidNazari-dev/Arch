

namespace Arch.Domian;

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
}
