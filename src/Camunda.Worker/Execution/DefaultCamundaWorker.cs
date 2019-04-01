#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public sealed class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskCamundaClient _externalTaskCamundaClient;
        private readonly IGeneralExternalTaskHandler _handler;
        private readonly ITopicsProvider _topicsProvider;
        private readonly CamundaWorkerOptions _options;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(IExternalTaskCamundaClient externalTaskCamundaClient,
            IGeneralExternalTaskHandler handler,
            ITopicsProvider topicsProvider,
            IOptions<CamundaWorkerOptions> options,
            ILogger<DefaultCamundaWorker> logger = null)
        {
            _externalTaskCamundaClient = Guard.NotNull(externalTaskCamundaClient, nameof(externalTaskCamundaClient));
            _handler = Guard.NotNull(handler, nameof(handler));
            _topicsProvider = Guard.NotNull(topicsProvider, nameof(topicsProvider));
            _options = Guard.NotNull(options, nameof(options)).Value;
            _logger = logger ?? new NullLogger<DefaultCamundaWorker>();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTasks = await SelectExternalTasks(cancellationToken);

                var activeAsyncTasks = externalTasks
                    .Select(CreateContext)
                    .Select(ExecuteInContext)
                    .ToList();

                await Task.WhenAll(activeAsyncTasks);
            }
        }

        private async Task<IEnumerable<ExternalTask>> SelectExternalTasks(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Waiting for external task");

                var fetchAndLockRequest = new FetchAndLockRequest(_options.WorkerId)
                {
                    UsePriority = true,
                    AsyncResponseTimeout = _options.AsyncResponseTimeout,
                    Topics = _topicsProvider.GetTopics()
                };

                var externalTasks = await _externalTaskCamundaClient.FetchAndLock(
                    fetchAndLockRequest, cancellationToken
                );

                return externalTasks;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await Task.Delay(10_000, cancellationToken);
                return Enumerable.Empty<ExternalTask>();
            }
        }

        private IExternalTaskContext CreateContext(ExternalTask externalTask) =>
            new ExternalTaskContext(externalTask, _externalTaskCamundaClient);

        private async Task ExecuteInContext(IExternalTaskContext context)
        {
            using (context)
            {
                try
                {
                    var result = await _handler.Process(context.Task);

                    await result.ExecuteResultAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed execution of task {Id}", context.Task.Id);
                }
            }
        }
    }
}
