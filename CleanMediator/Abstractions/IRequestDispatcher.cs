namespace CleanMediator.Abstractions;

/// <summary>
/// Dispatches requests to their respective handlers and applies any registered pipeline behaviors.
/// </summary>
public interface IRequestDispatcher
{
    /// <summary>
    /// Sends a non-generic request that does not expect a response, such as a command or signal.
    /// Pipeline behaviors are invoked in the order they are registered.
    /// </summary>
    /// <param name="request">The request to send. Must implement <see cref="IRequest"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendAsync(IRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request that expects a response and returns the result from its handler.
    /// Pipeline behaviors are invoked in the order they are registered.
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned by the request.</typeparam>
    /// <param name="request">The request to send. Must implement <see cref="IRequest{TResponse}"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, with the result of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
