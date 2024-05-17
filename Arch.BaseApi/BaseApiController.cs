

using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Arch.BaseApi;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : Controller
{
    protected IActionResult ApiOk()
    {
        return new ApiResult(HttpStatusCode.OK, new BaseResponse(ValueTask.CompletedTask));
    }
    protected IActionResult ApiOk(object data)
    {
        if (data.GetType().IsPrimitive || data is string)
        {
            return new ApiResult(HttpStatusCode.OK, new BaseResponse(ValueTask.CompletedTask));
        }
        return new ApiResult(HttpStatusCode.OK, data);
    }
    protected IActionResult ApiOk(object data, int count)
    {
        if (data.GetType().IsPrimitive || data is string)
        {
            throw new ArgumentException("Primitive types are not supported");
        }
        return new ApiResult(HttpStatusCode.OK, data, count);
    }
    protected IActionResult BadRequestInternal(string error)
    {
        ResponseError apiErrorEntry = new ResponseError("message", error);
        return BadRequestInternal(apiErrorEntry);
    }

    protected IActionResult BadRequestInternal(ResponseError error)
    {
        return new ApiResult(HttpStatusCode.BadRequest, new ResponseError[] { error });
    }
}
