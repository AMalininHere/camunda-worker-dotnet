// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker
{
    public class DefaultHandlerFactoryProvider : IHandlerFactoryProvider
    {
        private readonly IReadOnlyDictionary<string, HandlerDescriptor> _descriptors;

        public DefaultHandlerFactoryProvider(IEnumerable<HandlerDescriptor> descriptors)
        {
            _descriptors = descriptors
                .ToDictionary(descriptor => descriptor.TopicName);
        }

        public Func<IServiceProvider, IExternalTaskHandler> GetHandlerFactory(string topicName)
        {
            if (topicName == null)
            {
                throw new ArgumentNullException(nameof(topicName));
            }

            return _descriptors.TryGetValue(topicName, out var descriptor) ? descriptor.Factory : null;
        }
    }
}
