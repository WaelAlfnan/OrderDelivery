using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using OrderDelivery.Api.Filters;
using Xunit;

namespace OrderDelivery.UnitTests.Filters;

public class GlobalExceptionFilterTests
{
    private readonly Mock<ILogger<GlobalExceptionFilter>> _mockLogger;
    private readonly GlobalExceptionFilter _filter;

    public GlobalExceptionFilterTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionFilter>>();
        _filter = new GlobalExceptionFilter(_mockLogger.Object);
    }

    [Fact]
    public void OnException_GeneralException_SetsCorrectResponse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        Assert.Equal(500, objectResult.StatusCode);
        
        var response = objectResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public void OnException_ArgumentException_SetsCorrectResponse()
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public void OnException_InvalidOperationException_SetsCorrectResponse()
    {
        // Arrange
        var exception = new InvalidOperationException("Invalid operation");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public void OnException_TimeoutException_SetsCorrectResponse()
    {
        // Arrange
        var exception = new TimeoutException("Operation timed out");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public void OnException_Exception_LogsError()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unhandled exception occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void OnException_Exception_ResponseContainsExceptionMessage()
    {
        // Arrange
        var exception = new Exception("Test exception message");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        var response = objectResult.Value;
        Assert.NotNull(response);
    }

    [Fact]
    public void OnException_Exception_ResponseHasCorrectStructure()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        var response = objectResult.Value;
        Assert.NotNull(response);
        
        // Verify the response has the expected properties
        var responseType = response.GetType();
        var successProperty = responseType.GetProperty("Success");
        var messageProperty = responseType.GetProperty("Message");
        var dataProperty = responseType.GetProperty("Data");
        var errorsProperty = responseType.GetProperty("Errors");
        
        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);
        Assert.NotNull(dataProperty);
        Assert.NotNull(errorsProperty);
    }

    [Fact]
    public void OnException_Exception_ResponseSuccessIsFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        var response = objectResult.Value;
        Assert.NotNull(response);
        
        var successProperty = response.GetType().GetProperty("Success");
        var successValue = successProperty?.GetValue(response);
        Assert.False((bool)successValue!);
    }

    [Fact]
    public void OnException_Exception_ResponseMessageIsCorrect()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        var response = objectResult.Value;
        Assert.NotNull(response);
        
        var messageProperty = response.GetType().GetProperty("Message");
        var messageValue = messageProperty?.GetValue(response);
        Assert.Equal("An internal server error occurred", messageValue);
    }

    [Fact]
    public void OnException_Exception_ResponseDataIsNull()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        var response = objectResult.Value;
        Assert.NotNull(response);
        
        var dataProperty = response.GetType().GetProperty("Data");
        var dataValue = dataProperty?.GetValue(response);
        Assert.Null(dataValue);
    }

    [Fact]
    public void OnException_Exception_ResponseErrorsContainsExceptionMessage()
    {
        // Arrange
        var exception = new Exception("Test exception message");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        Assert.IsType<ObjectResult>(context.Result);
        
        var objectResult = (ObjectResult)context.Result;
        var response = objectResult.Value;
        Assert.NotNull(response);
        
        var errorsProperty = response.GetType().GetProperty("Errors");
        var errorsValue = errorsProperty?.GetValue(response);
        Assert.NotNull(errorsValue);
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }
}
