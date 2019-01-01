// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Camunda.Worker.Core
{
    public class CompositeExternalTaskHandlerTest
    {
        [Fact]
        public async Task TestExecute()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            var providerMock = new Mock<IServiceProvider>();

            scopeFactoryMock.Setup(factory => factory.CreateScope()).Returns(scopeMock.Object);
            scopeMock.SetupGet(scope => scope.ServiceProvider).Returns(providerMock.Object);

            var handlerFactoryProviderMock = new Mock<IHandlerFactoryProvider>();
            var handlerMock = new Mock<IExternalTaskHandler>();
            handlerFactoryProviderMock.Setup(factory => factory.GetHandlerFactory("testTopic"))
                .Returns((string topic) => provider => handlerMock.Object);

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CompleteResult(new Dictionary<string, Variable>
                {
                    ["DONE"] = new Variable(true)
                }));

            var executor = new CompositeExternalTaskHandler(
                scopeFactoryMock.Object,
                handlerFactoryProviderMock.Object,
                new NullLogger<CompositeExternalTaskHandler>()
            );

            var result = await executor.Process(new ExternalTask
            {
                Id = "1",
                TopicName = "testTopic",
                WorkerId = "testWorker",
                Variables = new Dictionary<string, Variable>()
            }, CancellationToken.None);

            var executionResult = Assert.IsAssignableFrom<CompleteResult>(result);
            Assert.True(executionResult.Variables.TryGetValue("DONE", out var resultVariable));
            Assert.True(Assert.IsType<bool>(resultVariable.Value));
        }
    }
}
