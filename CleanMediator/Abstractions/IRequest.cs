namespace CleanMediator.Abstractions
{
    /// <summary>
    /// Represents a request that does not return a response. Typically used for commands or signals where no result is expected.
    /// </summary>
    /// <remarks>
    /// This interface should be implemented by request types that are intended to trigger pipeline behavior without returning a result.
    /// For example: logging, notifications, or command actions.
    /// </remarks>
    public interface IRequest : IRequestBase
    {
    }

    /// <summary>
    /// Represents a request that returns a response of type <typeparamref name="TResponse"/>.
    /// Typically used for queries or commands that expect a result from a handler.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected from the request handler.</typeparam>
    /// <remarks>
    /// This interface should be implemented by request types that expect a value to be returned after being handled. Pipeline behaviour will be triggered as well if any corresponding exist.
    /// </remarks>
    public interface IRequest<out TResponse> : IRequestBase
    {
    }
}
