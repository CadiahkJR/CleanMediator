namespace CleanMediator.Abstractions;

/// <summary>
/// Defines a middleware pipeline behavior for non-generic requests that do not return a value.
/// This allows additional logic to be executed before or after the actual request handler is invoked.
/// </summary>
/// <typeparam name="TRequest">The type of the request. Must implement <see cref="IRequest"/>.</typeparam>
public interface IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles the incoming request, optionally performing actions before and/or after the next delegate is invoked.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">A delegate to invoke the next behavior in the pipeline or the request handler.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(
        TRequest request,
        Func<Task> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines a middleware pipeline behavior for generic requests that return a response.
/// This allows additional logic to be executed before or after the actual request handler is invoked,
/// such as logging, validation, performance measurement, or exception handling.
/// </summary>
/// <typeparam name="TRequest">The type of the request. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the incoming request, optionally performing actions before and/or after the next delegate is invoked.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">A delegate to invoke the next behavior in the pipeline or the request handler.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation with a result of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken);
}
