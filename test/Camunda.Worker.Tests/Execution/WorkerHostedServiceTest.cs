#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class WorkerHostedServiceTest
    {
        private readonly Mock<ICamundaWorker> _workerMock = new Mock<ICamundaWorker>();

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public async Task TestRunWorkerOnStart(int workerCount)
        {
            _workerMock.Setup(w => w.Run(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var options = Options.Create(new CamundaWorkerOptions
            {
                WorkerCount = workerCount
            });

            using (var workerHostedService =
                new WorkerHostedService(_workerMock.Object, options))
            {
                await workerHostedService.StartAsync(CancellationToken.None);
            }

            _workerMock.Verify(w => w.Run(It.IsAny<CancellationToken>()), Times.Exactly(options.Value.WorkerCount));
        }
    }
}
