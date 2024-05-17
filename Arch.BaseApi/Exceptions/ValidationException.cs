

namespace Arch.BaseApi; 
public class ValidationException : Exception
{
    public string ResponseMessageKey { get; }
    public string? SecondResponseMessageKey { get; }
    public IEnumerable<ResponseError> Errors { get; set; }
    public ValidationException(string responseMessageKey) : base(responseMessageKey)
    {
        ResponseMessageKey = responseMessageKey;
    }
    public ValidationException(string responseMessageKey, params ResponseError[] errors) : this(responseMessageKey)
    {
        ResponseMessageKey = responseMessageKey;
        Errors = errors;
    }
    public ValidationException(string firstResponseMessageKey, string secondResponseMessageKey, params ResponseError[] errors) : this(firstResponseMessageKey, errors)
    {
        ResponseMessageKey = firstResponseMessageKey;
        SecondResponseMessageKey = secondResponseMessageKey;
        Errors = errors;
    }
}
