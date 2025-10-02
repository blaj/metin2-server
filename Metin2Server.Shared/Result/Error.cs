using Microsoft.AspNetCore.Http;

namespace Blaj.AquafySharedLibrary.Result;

public record Error(string Code, int StatusCode, IList<string> Messages)
{
    public static readonly Error None = new(string.Empty, StatusCodes.Status500InternalServerError, []);

    public static readonly Error NullValue = new(
        "Error.NullValue",
        StatusCodes.Status400BadRequest,
        ["The specified result value is null."]);

    public static readonly Error ConditionNotMet = new(
        "Error.ConditionNotMet", 
        StatusCodes.Status422UnprocessableEntity,
        ["The specified condition was not met."]);
}