#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public sealed class CompleteResult : IExecutionResult
    {
        public CompleteResult(IDictionary<string, Variable> variables,
            IDictionary<string, Variable> localVariables = null)
        {
            Variables = variables ?? new Dictionary<string, Variable>();
            LocalVariables = localVariables ?? new Dictionary<string, Variable>();
        }

        public IDictionary<string, Variable> Variables { get; }

        public IDictionary<string, Variable> LocalVariables { get; }

        public Task ExecuteResultAsync(IExternalTaskContext context)
        {
            return context.CompleteAsync(Variables, LocalVariables);
        }
    }
}
