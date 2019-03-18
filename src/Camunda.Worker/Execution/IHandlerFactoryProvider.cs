#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


namespace Camunda.Worker.Execution
{
    public interface IHandlerFactoryProvider
    {
        HandlerFactory GetHandlerFactory(ExternalTask externalTask);
    }
}
