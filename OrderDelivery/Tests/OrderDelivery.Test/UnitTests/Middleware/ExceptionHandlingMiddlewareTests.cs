using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrderDelivery.Api.Middleware;
using System.Text;
using Xunit;

namespace OrderDelivery.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _middleware = new ExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _mockNext.Setup(x => x(context)).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        Assert.Contains("An internal server error occurred", responseBody);
        Assert.Contains("false", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_LogsError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ArgumentException_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new ArgumentException("Invalid argument");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        Assert.Contains("An internal server error occurred", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_InvalidOperationException_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new InvalidOperationException("Invalid operation");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        Assert.Contains("An internal server error occurred", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_TimeoutException_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new TimeoutException("Operation timed out");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        Assert.Contains("An internal server error occurred", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ResponseBodyIsValidJson()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        
        // Verify it's valid JSON
        Assert.StartsWith("{", responseBody);
        Assert.EndsWith("}", responseBody);
        Assert.Contains("success", responseBody);
        Assert.Contains("message", responseBody);
        Assert.Contains("data", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ResponseContainsCorrectStructure()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        
        // Verify the response structure
        Assert.Contains("\"success\":false", responseBody);
        Assert.Contains("\"message\":\"An internal server error occurred\"", responseBody);
        Assert.Contains("\"data\":null", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ResponseHeadersAreSet()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ResponseStreamIsWritten()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.True(responseStream.Length > 0);
        responseStream.Position = 0;
        var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
        Assert.NotEmpty(responseBody);
    }
}
