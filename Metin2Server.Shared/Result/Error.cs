namespace Metin2Server.Shared.Result;

public record Error(string Code, int StatusCode, IList<string> Messages)
{
    public static readonly Error None = new(string.Empty, 500, []);

    public static readonly Error NullValue = new(
        "Error.NullValue",
        400,
        ["The specified result value is null."]);

    public static readonly Error ConditionNotMet = new(
        "Error.ConditionNotMet", 
        422,
        ["The specified condition was not met."]);
}