// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker;

namespace SampleCamundaWorker.Handlers
{
    [HandlerTopic("sayHello")]
    [HandlerVariables("USERNAME")]
    public class SayHelloHandler : IExternalTaskHandler
    {
        public async Task<IDictionary<string, Variable>> Process(ExternalTask externalTask,
            CancellationToken cancellationToken)
        {
            var username = externalTask.Variables["USERNAME"].Value;

            await Task.Delay(1000, cancellationToken);

            return new Dictionary<string, Variable>
            {
                ["MESSAGE"] = new Variable($"Hello, {username}!")
            };
        }
    }
}
