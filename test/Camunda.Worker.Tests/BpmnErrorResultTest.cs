using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class BpmnErrorResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            _contextMock
                .Setup(context => context.ReportBpmnErrorAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, Variable>>()
                ))
                .Returns(Task.CompletedTask);

            var result = new BpmnErrorResult("TEST_CODE", "Test message");

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.ReportBpmnErrorAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestExecuteResultWithFailedCompletion()
        {
            _contextMock
                .Setup(context => context.ReportBpmnErrorAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, Variable>>()
                ))
                .ThrowsAsync(new ClientException(new ErrorResponse
                {
                    Type = "an error type",
                    Message = "an error message"
                }, HttpStatusCode.InternalServerError));

            Expression<Func<IExternalTaskContext, Task>> failureExpression = context => context.ReportFailureAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            );

            _contextMock
                .Setup(failureExpression)
                .Returns(Task.CompletedTask);

            var result = new BpmnErrorResult("TEST_CODE", "Test message");

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.ReportBpmnErrorAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.Verify(failureExpression, Times.Once());
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
