// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static ICamundaWorkerBuilder AddCamundaWorker(this IServiceCollection services,
            Action<CamundaWorkerOptions> configureDelegate)
        {
            services.AddOptions<CamundaWorkerOptions>()
                .Configure(configureDelegate);
            services.AddHttpClient<IExternalTaskCamundaClient, ExternalTaskCamundaClient>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<CamundaWorkerOptions>>().Value;
                    client.BaseAddress = options.BaseUri;
                });

            services.TryAddTransient<ITopicsProvider, StaticTopicsProvider>();
            services.TryAddTransient<ICamundaWorker, DefaultCamundaWorker>();
            services.AddHostedService<WorkerHostedService>();

            return services.AddCamundaWorkerCore();
        }

        public static ICamundaWorkerBuilder AddCamundaWorkerCore(this IServiceCollection services)
        {
            services.TryAddTransient<IHandlerFactoryProvider, TopicBasedFactoryProvider>();
            services.TryAddTransient<IGeneralExternalTaskHandler, GeneralExternalTaskHandler>();

            return new CamundaWorkerBuilder(services);
        }
    }
}
