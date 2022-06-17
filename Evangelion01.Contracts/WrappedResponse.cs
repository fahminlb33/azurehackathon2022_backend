namespace Evangelion01.Contracts
{
    public record WrappedResponse(bool Success, string Message, object? Data = null);
    public record WrappedResponse<T>(bool Success, string Message, T? Data = default);
}
