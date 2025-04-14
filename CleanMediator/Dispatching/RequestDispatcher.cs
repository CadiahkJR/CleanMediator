using CleanMediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CleanMediator.Dispatching;

/// <summary>
/// The default implementation of <see cref="IRequestDispatcher"/> responsible for resolving request handlers
/// and invoking pipeline behaviors before and after the request is handled.
/// </summary>
public class RequestDispatcher(IServiceProvider serviceProvider) : IRequestDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// Sends a non-generic request to its handler and executes any associated pipeline behaviors.
    /// This is used for requests that do not produce a response.
    /// </summary>
    /// <param name="request">The request to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();

        #region Resolve Handler
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler found for {requestType.Name}");


        Func<Task> handlerDelegate = () =>
        {
            var method = handlerType.GetMethod("HandleAsync")!;
            return (Task)method.Invoke(handler, [request, cancellationToken])!;
        };
        #endregion

        #region Apply Pipeline Behaviors
        var behaviorType = typeof(IPipelineBehavior<>).MakeGenericType(requestType);
        var behaviors = _serviceProvider.GetServices(behaviorType).Reverse().Cast<object>();

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () =>
            {
                var method = behavior.GetType().GetMethod("HandleAsync")!;
                return (Task)method.Invoke(behavior, [request, cancellationToken, next])!;
            };
        }

        #endregion
        await handlerDelegate();
    }

    /// <summary>
    /// Sends a request that expects a response to its handler and executes any associated pipeline behaviors.
    /// </summary>
    /// <typeparam name="TResponse">The type of the expected response.</typeparam>
    /// <param name="request">The request to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response from the request handler.</returns>
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        #region Resolve Handler
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler found for {requestType.Name} -> {responseType.Name}");


        Func<Task<TResponse>> handlerDelegate = async () =>
        {
            var method = handlerType.GetMethod("HandleAsync")!;
            return await (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        };
        #endregion

        #region Apply Pipeline Behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType).Reverse().Cast<object>();

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = async () =>
            {
                var method = behavior.GetType().GetMethod("HandleAsync")!;
                return await (Task<TResponse>)method.Invoke(behavior, [request, next, cancellationToken])!;
            };
        }
        #endregion

        return await handlerDelegate();
    }
}
