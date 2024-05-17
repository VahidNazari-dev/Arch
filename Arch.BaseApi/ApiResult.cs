

using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Net;
using System.Collections;
using System.Resources;

namespace Arch.BaseApi;

public class ApiResult : ActionResult
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public string? Message { get; set; }
    public IEnumerable<ResponseError> Errors { get; set; }
    public object Data { get; set; }
    public int? Count { get; set; }
    public ApiResult(HttpStatusCode statusCode, string message, IEnumerable<ResponseError> errors)
    {
        StatusCode = statusCode;
        Count = 0;
        Errors = errors;
        Message = message;

    }
    public ApiResult(HttpStatusCode statusCode, string firstMessage, string secondMessage, IEnumerable<ResponseError> errors)
    {
        StatusCode = statusCode;
        Count = 0;
        Errors = errors;
        Message = StringFormat(firstMessage, secondMessage);
    }
    public ApiResult(HttpStatusCode statusCode, ResponseError[] errors)
    {
        StatusCode = statusCode;
        Errors = errors;
        Count = 0;
    }
    /// <summary>
    /// Use In ApiOk() Method
    /// </summary>
    public ApiResult(HttpStatusCode code, object data)
    {

        StatusCode = code;
        Data = data;
        Message = "عملیات با موفقیت انجام شد";
        ICollection collection = Data as ICollection;
        if (collection != null)
            Count = collection.Count;
        else
            Count = 1;
    }
    /// <summary>
    /// Use In ApiOk() Method
    /// </summary>
    public ApiResult(HttpStatusCode code, object data, int count)
    {
        StatusCode = code;
        Data = data;
        Message = "عملیات با موفقیت انجام شد";
        Count = count;
    }
    public override void ExecuteResult(ActionContext context)
    {
        SetStatusCode(context);
        CreateResult().ExecuteResult(context);
    }
    public override Task ExecuteResultAsync(ActionContext context)
    {
        SetStatusCode(context);
        return CreateResult().ExecuteResultAsync(context);
    }

    #region PrivateMethod
    private ObjectResult CreateResult()
    {
        return new ObjectResult(this);
    }

    private void SetStatusCode(ActionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)StatusCode;
    }
    private string StringFormat(string param, params object[] args) => string.Format(param, args);

    #endregion
}
