using CleanMediator.Abstractions;
using CleanMediator.Dispatching;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CleanMediator.Tests
{
    public class DispatcherTests
    {
        [Fact]
        public async Task Dispatch_GenericRequest_ReturnsExpectedResponse()
        {
            var services = new ServiceCollection();
            services.AddScoped<IRequestDispatcher, RequestDispatcher>();
            services.AddScoped<IRequestHandler<SampleQuery, string>, SampleQueryHandler>();

            var provider = services.BuildServiceProvider();
            var dispatcher = provider.GetRequiredService<IRequestDispatcher>();
            var result = await dispatcher.SendAsync(new SampleQuery());

            result.ShouldBe("CleanMediator is awesome");
        }

        [Fact]
        public async Task Dispatch_NonGenericRequest_CallsHandler()
        {
            var services = new ServiceCollection();
            services.AddScoped<IRequestDispatcher, RequestDispatcher>();
            services.AddScoped<IRequestHandler<SimpleCommand>, SimpleCommandHandler>();

            var provider = services.BuildServiceProvider();
            var dispatcher = provider.GetRequiredService<IRequestDispatcher>();

            await dispatcher.SendAsync(new SimpleCommand());

            SimpleCommandHandler.Executed.ShouldBeTrue();
        }

        [Fact]
        public async Task Dispatch_Throws_WhenHandlerNotRegistered()
        {
            var services = new ServiceCollection();
            services.AddScoped<IRequestDispatcher, RequestDispatcher>();

            var provider = services.BuildServiceProvider();
            var dispatcher = provider.GetRequiredService<IRequestDispatcher>();

            var ex = await Should.ThrowAsync<InvalidOperationException>(() => dispatcher.SendAsync(new SampleQuery()));
            ex.Message.ShouldContain("No handler found");
        }

        #region Sample Types for Request and Handler
        private class SampleQuery : IRequest<string> { }

        private class SampleQueryHandler : IRequestHandler<SampleQuery, string>
        {
            public Task<string> HandleAsync(SampleQuery request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult("CleanMediator is awesome");
            }
        }

        private class SimpleCommand : IRequest { }

        private class SimpleCommandHandler : IRequestHandler<SimpleCommand>
        {
            public static bool Executed = false;

            public Task HandleAsync(SimpleCommand request, CancellationToken cancellationToken = default)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }
        #endregion
    }
}