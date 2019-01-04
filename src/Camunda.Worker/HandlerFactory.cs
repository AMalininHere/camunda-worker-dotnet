// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Execution;

namespace Camunda.Worker
{
    public delegate IExternalTaskHandler HandlerFactory(IServiceProvider provider);
}
