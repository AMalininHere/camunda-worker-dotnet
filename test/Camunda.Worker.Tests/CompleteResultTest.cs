// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CompleteResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            _contextMock
                .Setup(context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ))
                .Returns(Task.CompletedTask);

            var result = new CompleteResult(new Dictionary<string, Variable>());

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
