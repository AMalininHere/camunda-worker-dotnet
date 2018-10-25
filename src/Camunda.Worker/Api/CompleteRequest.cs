// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace Camunda.Worker.Api
{
    public class CompleteRequest
    {
        public string WorkerId { get; set; }
        public IDictionary<string, Variable> Variables { get; set; }
    }
}
