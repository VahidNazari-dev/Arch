

namespace Arch.BaseApi;

public class BaseResponse
{
    public BaseResponse(ValueTask result)
    {
        Result = result;
    }

    public ValueTask Result { get; }
}
