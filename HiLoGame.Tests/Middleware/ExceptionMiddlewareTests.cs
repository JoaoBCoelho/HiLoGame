using HiLoGame.API.Middlewares;
using HiLoGame.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;

namespace HiLoGame.Tests.Middleware
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_SystemException_ShouldReturnInternalServerError()
        {
            // Arrange
            const int expectedStatusCode = (int)HttpStatusCode.InternalServerError;

            var middleware = new ExceptionMiddleware((innerHttpContext) =>
            {
                throw new Exception("Throw new System.Exception");
            });

            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context);

            //Assert
            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ShouldReturnBadRequest()
        {
            //Arrange
            const int expectedStatusCode = (int)HttpStatusCode.BadRequest;

            var middleware = new ExceptionMiddleware((innerHttpContext) =>
            {
                throw new ValidationException("Throw new ValidationException");
            });

            var context = new DefaultHttpContext();

            //Act
            await middleware.InvokeAsync(context);

            //Assert
            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_NoException_ShouldRunCorrectly()
        {
            //Arrange
            const int expectedStatusCode = (int)HttpStatusCode.OK;

            var middleware = new ExceptionMiddleware((innerHttpContext) => Task.CompletedTask);

            var context = new DefaultHttpContext();

            //Act
            await middleware.InvokeAsync(context);

            //Assert
            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        }
    }
}
