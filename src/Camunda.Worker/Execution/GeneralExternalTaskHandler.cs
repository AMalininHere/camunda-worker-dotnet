// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class GeneralExternalTaskHandler : IGeneralExternalTaskHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHandlerFactoryProvider _handlerFactoryProvider;
        private readonly ILogger<GeneralExternalTaskHandler> _logger;

        public GeneralExternalTaskHandler(IServiceScopeFactory scopeFactory,
            IHandlerFactoryProvider handlerFactoryProvider,
            ILogger<GeneralExternalTaskHandler> logger = null)
        {
            _scopeFactory = scopeFactory;
            _handlerFactoryProvider = handlerFactoryProvider;
            _logger = logger ?? new NullLogger<GeneralExternalTaskHandler>();
        }

        public async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            if (externalTask == null)
            {
                throw new ArgumentNullException(nameof(externalTask));
            }

            var handlerFactory = _handlerFactoryProvider.GetHandlerFactory(externalTask);

            using (var scope = _scopeFactory.CreateScope())
            {
                var handler = handlerFactory(scope.ServiceProvider);

                _logger.LogInformation("Started processing of task {TaskId}", externalTask.Id);

                IExecutionResult result;
                try
                {
                    result = await handler.Process(externalTask);
                }
                catch (Exception e)
                {
                    result = new FailureResult(e);
                }

                _logger.LogInformation("Finished processing of task {TaskId}", externalTask.Id);

                return result;
            }
        }
    }
}
