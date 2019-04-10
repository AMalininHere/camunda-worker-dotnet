#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public interface IExternalTaskSelector
    {
        Task<IEnumerable<ExternalTask>> SelectAsync(IEnumerable<FetchAndLockRequest.Topic> topics,
            CancellationToken cancellationToken = default);
    }
}
