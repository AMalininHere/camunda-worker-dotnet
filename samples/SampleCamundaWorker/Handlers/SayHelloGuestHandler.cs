using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker;

namespace SampleCamundaWorker.Handlers
{
    [HandlerTopics("sayHelloGuest")]
    public class SayHelloGuestHandler : ExternalTaskHandler
    {
        public override Task<IExecutionResult> HandleAsync(ExternalTask externalTask)
        {
            return Task.FromResult<IExecutionResult>(new CompleteResult
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["MESSAGE"] = Variable.String("Hello, Guest!")
                }
            });
        }
    }
}
