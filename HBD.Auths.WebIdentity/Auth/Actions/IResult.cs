using Microsoft.AspNetCore.Identity;

namespace HBD.Auths.WebIdentity.Auth.Actions;

public interface IResult<out TResult>
{
    TResult? Value { get; }
    bool Succeeded { get; }
    IEnumerable<IdentityError>? Errors { get; }
}

internal class Result<TResult>:IResult<TResult>
{
    internal Result(TResult? value)
    {
        Value = value;
        Succeeded = true;
    }
    
    internal Result(IEnumerable<IdentityError> errors )
    {
        Errors = errors;
        Succeeded = false;
    }
    
    public TResult? Value { get; }
    public bool Succeeded { get; }
    public IEnumerable<IdentityError>? Errors { get; }
}

public static class Result
{
    public static IResult<TResult> Success<TResult>(TResult result) => new Result<TResult>(result);
    public static IResult<TResult> Fails<TResult>(IEnumerable<IdentityError> errors) => new Result<TResult>(errors);
}