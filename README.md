# CleanMediator

[![NuGet](https://img.shields.io/nuget/v/CleanMediator.svg)](https://www.nuget.org/packages/CleanMediator)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

**CleanMediator** is a lightweight mediator and pipeline behavior library tailored for Clean Architecture projects. It provides structured request/response handling with optional middleware-style pipeline behaviors for pre- and post-processing logic such as logging, validation, caching, etc.

---

## ✨ Features

- Minimal setup, zero external dependencies
- Supports both commands (`IRequest`) and queries (`IRequest<T>`)
- Pipeline behaviors for clean cross-cutting concerns
- Automatic handler and behavior discovery
- Built-in dependency injection support
- Fully unit-testable
- .NET 8+ compatible

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package CleanMediator
```

---

## 🚀 Getting Started

### 1. Register CleanMediator

```csharp
builder.Services.AddCleanMediator();
```

This will automatically register:
- All `IRequestHandler<TRequest>` and `IRequestHandler<TRequest, TResponse>` implementations
- All `IPipelineBehavior<TRequest>` and `IPipelineBehavior<TRequest, TResponse>` behaviors

---

### 2. Create a Request (Query)

```csharp
public class GetUserByIdQuery : IRequest<UserResponse>
{
    public int Id { get; set; }
}
```

### 3. Create a Handler

```csharp
public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    public Task<UserResponse> HandleAsync(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserResponse
        {
            Id = request.Id,
            Name = "Jane Doe"
        });
    }
}
```

### 4. Dispatch from Code (Controller)

```csharp
[Route("api/[controller]")]
    [ApiController]
    public class UsersController(IRequestDispatcher dispatcher) : ControllerBase
    {
        private readonly IRequestDispatcher _dispatcher = dispatcher;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var result = await _dispatcher.SendAsync(new GetUserByIdQuery{ Id = id });
            return Ok(result);
        }
    }
```

---

## 🔁 Commands (No Response)

```csharp
public class DeactivateUserCommand : IRequest
{
    public int UserId { get; set; }
}

public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand>
{
    public Task HandleAsync(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"User {request.UserId} deactivated");
        return Task.CompletedTask;
    }
}

[HttpPut("{id}")]
public async Task<IActionResult> DeactivateUserAsync(int id)
{
    await _dispatcher.SendAsync(new DeactivateUserCommand { UserId = id });
    return Ok("User Deactivated Successfully.");
}
```

---

## 🧩 Pipeline Behaviors

### 1. Logging Behavior (Generic)

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
        return response;
    }
}
```

### 2. Validation Behavior (Non-Generic)

```csharp
public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public async Task HandleAsync(TRequest request, Func<Task> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Validating {typeof(TRequest).Name}");
        await next();
    }
}
```
---

## 🧪 Unit Testing

Use `xUnit` + `Shouldly` for clean tests:

```csharp
[Fact]
public async Task Dispatch_GenericRequest_ReturnsExpectedResponse()
{
    var services = new ServiceCollection();
    services.AddScoped<IRequestDispatcher, RequestDispatcher>();
    services.AddScoped<IRequestHandler<GetUserByIdQuery, UserResponse>, GetUserByIdHandler>();

    var provider = services.BuildServiceProvider();
    var dispatcher = provider.GetRequiredService<IRequestDispatcher>();

    var result = await dispatcher.SendAsync(new GetUserByIdQuery { Id = 1 });
    result.Name.ShouldBe("Jane Doe");
}
```

---

## 📂 Folder Structure Suggestion

```text
src/
  CleanMediator/           --> NuGet package source
test/
  CleanMediator.Tests/     --> Unit tests with xUnit and Shouldly
```

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).

---

## 🤝 Contributing

Contributions are welcome! Please fork, fix, and submit a PR.
Open issues if you have questions or suggestions.

---

## 🌐 Links

- [NuGet Package](https://www.nuget.org/packages/CleanMediator)
- [GitHub Issues](https://github.com/CadiahkJR/CleanMediator/issues)
- [License](LICENSE)