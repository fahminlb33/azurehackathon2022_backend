namespace Evangelion01.Contracts
{
    public record WrappedResponse(bool Success, string Message, object? Data = null);
}
